using ChessGame.Scripts.Boards;
using ChessGame.Scripts.ChessBoard.Boards;
using ChessGame.Scripts.DataTypes;
using ChessGame.Scripts.Factories;
using ChessGame.Scripts.Helpers;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace ChessGame.Scripts.Controllers
{
    public partial class BoardController : Node
    {
        private GraphicalBoard _gBoard { get; set; }
        private PieceInfo[,] _board { get; set; }
        private PlayerMovementController _playerMovementController { get; set; }

        private List<BoardPos>[,] _moveCache;

        private List<PieceInfo> _whitePieces = new List<PieceInfo>();
        private List<PieceInfo> _blackPieces = new List<PieceInfo>();

        private ChessColor _playerColor;
        private static string _startingFenString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

        private TimerService _timerService;
        private TurnService _turnService;
        private GameInfoService _gameInfoService;

        [Signal]
        public delegate void ColorIsInCheckUpdateEventHandler(ChessColor color, bool inCheck);
        [Signal]
        public delegate void ColorIsInCheckmateUpdateEventHandler(ChessColor color, bool inCheckmate);
        [Signal]
        public delegate void GameOverEventHandler(ChessColor winner);

        public override void _Ready()
        {
            _gameInfoService = ServiceFactory.GetGameInfoService();
            _timerService = ServiceFactory.GetTimerService();
            _turnService = ServiceFactory.GetTurnService();

            _gBoard = BoardFactory.GetGraphicalBoard();
            _board = BoardDataHandler.CreateNewBoard();
            _playerMovementController = ControllerFactory.GetPlayerMovementController();

            _playerColor = _gameInfoService.PlayerSideColor;
        }

        public override void _Process(double delta)
        {
            if (_playerMovementController.IsDragging)
            {
                _playerMovementController.PieceBeingDragged.Position = GetViewport().GetMousePosition();
            }
        }

        public override void _Input(InputEvent @event)
        {
            _playerMovementController.InputHandler(@event);
        }

        public void CreateDefaultBoard()
        {
            CreateBoard(_startingFenString);
        }

        public void CreateBoard(string fenString)
        {
            PieceInfo[,] fenBoard = GetBoardFromFEN(fenString);
            UpdateBoard(fenBoard);
            _moveCache = MoveHelpers.CreateMoveCache(_board);
        }

        public void AddPiece(BoardPos pos, PieceInfo piece)
        {
            _gBoard.AddPiece(pos, piece);
            BoardDataHandler.AddPiece(_board, pos, piece);

            SendFENUpdate();
        }

        public void RemovePiece(BoardPos pos)
        {
            _gBoard.RemovePiece(pos);
            BoardDataHandler.RemovePiece(_board, pos);

            SendFENUpdate();
        }

        public bool MovePiece(BoardPos pos, BoardPos targetPos)
        {
            var movingPieceInfo = BoardDataHandler.GetPieceInfoAtPos(_board, pos);
            var pieceColor = movingPieceInfo.Color;
            var whiteKingPos = BoardSearching.GetKingPos(_board, ChessColor.White);
            var blackKingPos = BoardSearching.GetKingPos(_board, ChessColor.Black);
            var opposingColor = MiscHelpers.InvertColor(pieceColor);

            var whiteCheckMate = false;
            var blackCheckMate = false;

            var invalidateCastle = false;

            List<BoardPos> capableMoves = _moveCache[pos.Rank, pos.File];

            if (pieceColor != _turnService.GetCurrentTurnColor())
            {
                return false;
            }

            // King in Check Check
            if (_gameInfoService.ColorInCheck(pieceColor) && movingPieceInfo.PieceId != ChessPieceId.King)
            {

                BoardPos kingPos = pieceColor == ChessColor.White ? whiteKingPos : blackKingPos;
                var attackers = BoardSearching.GetAllAttackerPositions(_board, kingPos, _moveCache);

                foreach (var attacker in attackers)
                {
                    
                    var blockers = BoardSearching.GetAllBlockingPiecePositions(_board, _moveCache, kingPos, attacker);
                    
                    // Capturing the attacking piece can also be considered "blocking"
                    if (_moveCache[pos.Rank, pos.File].Any((mPos) => mPos == attacker))
                    {
                        continue;
                    }

                    // If this piece has no possible moves that can block check, it fails this move regardless
                    if (!blockers.Any((blockerPos) => blockerPos == pos))
                    {
                        return false;
                    }
                } 
            }

            var result = capableMoves.SingleOrDefault((pos) => pos == targetPos);
            if (result != null)
            {
                PieceInfo[,] futureBoard = (PieceInfo[,])_board.Clone();
                BoardDataHandler.MovePiece(futureBoard, pos, targetPos);

                List<BoardPos>[,] _futureBoardMoveCache = MoveHelpers.CreateMoveCache(futureBoard);

                bool movedIntoCheck = false;
                EndgameHandler endHandFuture = new EndgameHandler(_board, _futureBoardMoveCache);

                BoardPos futureBlackKingPos = BoardSearching.GetKingPos(futureBoard, ChessColor.Black);
                BoardPos futureWhiteKingPos = BoardSearching.GetKingPos(futureBoard, ChessColor.White);
                

                if (pieceColor == ChessColor.White)
                {
                    movedIntoCheck = endHandFuture.CheckCheck(futureWhiteKingPos, ChessColor.Black);
                }
                else
                {
                    movedIntoCheck = endHandFuture.CheckCheck(futureBlackKingPos, ChessColor.White);
                }

                if (movedIntoCheck)
                {
                    return false;
                }

                PawnChecks(pos, targetPos, movingPieceInfo, opposingColor);

                BoardDataHandler.MovePiece(_board, pos, targetPos);
                _gBoard.MovePiece(pos, movingPieceInfo, targetPos);

                if (movingPieceInfo.PieceId == ChessPieceId.King)
                {
                    // Can't move a king into check
                    if (MoveHelpers.IsTileUnderAttack(_board, targetPos, opposingColor, _moveCache))
                    {
                        return false;
                    }

                    // Castling
                    if (MoveHelpers.IsCastleMove(pos, targetPos))
                    {
                        // move rook to other side of king
                        CastleSide cSide = (CastleSide)MoveHelpers.GetCastlingDirection(targetPos);
                        BoardPos rookPos = MoveHelpers.GetPosOfRook(movingPieceInfo.Color, cSide);
                        PieceInfo rookInfo = GetPieceInfoAtPos(rookPos);
                        BoardPos newRookPos = MoveHelpers.GetPosOfRookAfterCastling(movingPieceInfo.Color, cSide);

                        BoardDataHandler.MovePiece(_board, rookPos, newRookPos);
                        _gBoard.MovePiece(rookPos, rookInfo, newRookPos);

                        invalidateCastle = true;
                    }
                    else
                    {
                        // Other type of move (invalidate castling)
                        if (movingPieceInfo.Color == ChessColor.White)
                        {
                            invalidateCastle = true;
                        }
                    }

                    if (movingPieceInfo.Color == ChessColor.White)
                    {
                        whiteKingPos = targetPos;
                    } else
                    {
                        blackKingPos = targetPos;
                    }
                }

                RookCastleInvalidation(pos, movingPieceInfo);

                _turnService.SwitchTurn();

                _moveCache = MoveHelpers.CreateMoveCache(_board);
                SendFENUpdate();

                EndgameHandler endHandCurrent = new EndgameHandler(_board, _moveCache);

                bool whiteInCheck = endHandCurrent.CheckCheck(whiteKingPos, ChessColor.Black);
                bool blackInCheck = endHandCurrent.CheckCheck(blackKingPos, ChessColor.White);

                whiteCheckMate = endHandCurrent.CheckMateCheck(whiteKingPos, ChessColor.Black, _moveCache[whiteKingPos.Rank, whiteKingPos.File]);
                EmitSignal(SignalName.ColorIsInCheckmateUpdate, (int)ChessColor.White, whiteCheckMate);
                blackCheckMate = endHandCurrent.CheckMateCheck(blackKingPos, ChessColor.White, _moveCache[blackKingPos.Rank, blackKingPos.File]);
                EmitSignal(SignalName.ColorIsInCheckmateUpdate, (int)ChessColor.Black, blackCheckMate);

                if (whiteCheckMate)
                {
                    EmitSignal(SignalName.GameOver, (int)ChessColor.Black);
                }
                else if (blackCheckMate)
                {
                    EmitSignal(SignalName.GameOver, (int)ChessColor.White);
                }

                if (invalidateCastle)
                {
                    if (movingPieceInfo.Color == ChessColor.White)
                    {
                        _gameInfoService.CanWKingCastle = false;
                        _gameInfoService.CanWQueenCastle = false;
                    }

                    if (movingPieceInfo.Color == ChessColor.Black)
                    {
                        _gameInfoService.CanBKingCastle = false;
                        _gameInfoService.CanBQueenCastle = false;
                    }
                }

                EmitSignal(SignalName.ColorIsInCheckUpdate, (int)ChessColor.White, whiteInCheck);
                EmitSignal(SignalName.ColorIsInCheckUpdate, (int)ChessColor.Black, blackInCheck);

                return true;
            }

            return false;
        }

        private void RookCastleInvalidation(BoardPos pos, PieceInfo movingPieceInfo)
        {
            if (movingPieceInfo.PieceId == ChessPieceId.Rook)
            {
                if (pos.File == 0)
                {
                    if (movingPieceInfo.Color == ChessColor.White)
                    {
                        _gameInfoService.CanWQueenCastle = false;
                    }
                    else
                    {
                        _gameInfoService.CanBQueenCastle = false;
                    }
                }
                else if (pos.File == 7)
                {
                    if (movingPieceInfo.Color == ChessColor.White)
                    {
                        _gameInfoService.CanWKingCastle = false;
                    }
                    else
                    {
                        _gameInfoService.CanBKingCastle = false;
                    }
                }
            }
        }

        private void PawnChecks(BoardPos pos, BoardPos targetPos, PieceInfo movingPieceInfo, ChessColor opposingColor)
        {
            if (movingPieceInfo.PieceId == ChessPieceId.Pawn)
            {
                // Double pawn move
                if (MoveHelpers.GetDistanceFromStart(pos.Rank, pos.File, targetPos.Rank, targetPos.File).Abs().X == 2)
                {
                    _gameInfoService.ToggleEnpassant(movingPieceInfo.Color, pos.File);
                }
                else if (MoveHelpers.IsEnpassant(pos, targetPos, GetPieceInfoAtPos(targetPos)))
                {
                    BoardPos enPassantedPawnPos = MoveHelpers.GetPosOfDoubleMovePawn(opposingColor, targetPos.File);
                    RemovePiece(enPassantedPawnPos);
                }
            }
        }

        public PieceInfo GetPieceInfoAtPos(BoardPos pos)
        {
            return BoardDataHandler.GetPieceInfoAtPos(_board, pos);
        }

        public List<VisualChessPiece> GetVisualPieces()
        {
            List<VisualChessPiece> pieces = new List<VisualChessPiece>();

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    var piece = _gBoard.GetPiece(new BoardPos(rank, file));

                    if (piece != null)
                    {
                        pieces.Add(piece);
                    }
                }
            }

            return pieces;
        }

        public List<BoardPos> GetMovesForPiece(BoardPos pos)
        {
            List<BoardPos> moves = _moveCache[pos.Rank, pos.File];
            return moves;
        }

        public Vector2 GetTileCenter(BoardPos pos)
        {
            return GridMathHelpers.CalculateTileCenter(pos, _gameInfoService.ViewInverted());
        }

        public void Test()
        {
            AddPiece(new BoardPos(4, 4), new PieceInfo { Color = ChessColor.White, PieceId = ChessPieceId.Queen });
            // AddPiece(new BoardPos(4, 4), new PieceInfo { Color = ChessColor.White, PieceId = ChessPieceId.Knight });

            //LogHelpers.DebugLog();

        }

        private PieceInfo[,] GetBoardFromFEN(string fenString)
        {
            PieceInfo[,] newBoard = FEN.Decrypt(fenString, _whitePieces, _blackPieces);
            return newBoard;
        }

        private void UpdateBoard(PieceInfo[,] board)
        {
            _gBoard.ClearBoard();
            BoardDataHandler.ClearBoard(_board);

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    AddPiece(new BoardPos(rank, file), board[rank, file]);
                }
            }

            SendFENUpdate();
        }

        private void SendFENUpdate()
        {
            _gameInfoService.EmitFenStringSignal(FEN.Encrypt(_board));
        }
    }
}
