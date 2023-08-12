using ChessGame.Scripts.ChessBoard;
using ChessGame.Scripts.ChessBoard.Boards;
using ChessGame.Scripts.DataTypes;
using ChessGame.Scripts.Factories;
using ChessGame.Scripts.Helpers;
using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace ChessGame.Scripts.Controllers
{
    public partial class BoardController : Node
    {
        private GraphicalBoard _gBoard { get; set; }
        private ILogicalBoard _logicBoard { get; set; }
        private PlayerMovementController _playerMovementController { get; set; }
        private MoveController _moveController { get; set; }

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
            _logicBoard = BoardFactory.GetLogicalBoard();
            _playerMovementController = ControllerFactory.GetPlayerMovementController();
            _moveController = ControllerFactory.GetMoveController(_logicBoard);

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
            _moveController.UpdateMoveCache();
        }

        public void AddPiece(BoardPos pos, PieceInfo piece)
        {
            _gBoard.AddPiece(pos, piece);
            _logicBoard.AddPiece(pos, piece);

            SendFENUpdate();
        }

        public void RemovePiece(BoardPos pos)
        {
            _gBoard.RemovePiece(pos);
            _logicBoard.RemovePiece(pos);

            SendFENUpdate();
        }

        public bool MovePiece(BoardPos pos, BoardPos targetPos)
        {
            var movingPieceInfo = _logicBoard.GetPieceInfoAtPos(pos);
            var pieceColor = movingPieceInfo.Color;
            var whiteKingPos = GetKingPos(ChessColor.White);
            var blackKingPos = GetKingPos(ChessColor.Black);
            var attackingColor = pieceColor == ChessColor.White ? ChessColor.Black : ChessColor.White;

            var whiteCheckMate = false;
            var blackCheckMate = false;

            List<BoardPos> capableMoves = _moveController.GetMovesAtPos(pos);

            if (pieceColor != _turnService.GetCurrentTurnColor())
            {
                return false;
            }

            // King in Check Check
            if (_gameInfoService.ColorInCheck(pieceColor) && movingPieceInfo.PieceId != ChessPieceId.King)
            {
                return false;
            }

            
            // Can't move a king into check
            if (movingPieceInfo.PieceId == ChessPieceId.King && _moveController.IsTileUnderAttack(targetPos, attackingColor))
            {
                return false;
            }

            var result = capableMoves.SingleOrDefault((pos) => pos == targetPos);
            if (result != null)
            {
                _logicBoard.MovePiece(pos, targetPos);
                _gBoard.MovePiece(pos, movingPieceInfo, targetPos);

                _turnService.SwitchTurn();

                _moveController.UpdateMoveCache();
                SendFENUpdate(); 

                bool whiteInCheck = _moveController.CheckCheck(whiteKingPos, ChessColor.Black);
                bool blackInCheck = _moveController.CheckCheck(blackKingPos, ChessColor.White);

                whiteCheckMate = CheckMateCheck(whiteKingPos, ChessColor.Black, _moveController.GetMovesAtPos(whiteKingPos));
                EmitSignal(SignalName.ColorIsInCheckmateUpdate, (int)ChessColor.White, whiteCheckMate);
                blackCheckMate = CheckMateCheck(blackKingPos, ChessColor.White, _moveController.GetMovesAtPos(blackKingPos));
                EmitSignal(SignalName.ColorIsInCheckmateUpdate, (int)ChessColor.Black, blackCheckMate);

                if (whiteCheckMate)
                {
                    EmitSignal(SignalName.GameOver, (int)ChessColor.Black);
                } else if (blackCheckMate)
                {
                    EmitSignal(SignalName.GameOver, (int)ChessColor.White);
                }

                EmitSignal(SignalName.ColorIsInCheckUpdate, (int)ChessColor.White, whiteInCheck);
                EmitSignal(SignalName.ColorIsInCheckUpdate, (int)ChessColor.Black, blackInCheck);

                return true;
            }

            return false;
        }

        public PieceInfo GetPieceInfoAtPos(BoardPos pos)
        {
            return _logicBoard.GetPieceInfoAtPos(pos);
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
            List<BoardPos> moves = _moveController.GetMovesAtPos(pos);
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
            _logicBoard.ClearBoard();

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
            _gameInfoService.EmitFenStringSignal(FEN.Encrypt(_logicBoard.GetBoard()));
        }

        private BoardPos GetKingPos(ChessColor kingColor)
        {
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    var boardPos = new BoardPos(rank, file);
                    var pieceAtTile = _logicBoard.GetPieceInfoAtPos(boardPos);

                    if (pieceAtTile.PieceId == ChessPieceId.King && pieceAtTile.Color == kingColor)
                    {
                        return boardPos;
                    }
                }
            }

            return null;
        }

        private bool CheckMateCheck(BoardPos kingPos, ChessColor attackerColor, List<BoardPos> moves)
        {
            if (!_moveController.IsTileUnderAttack(kingPos, attackerColor))
            {
                return false;
            }

            foreach (var move in moves)
            {
                if (!_moveController.IsTileUnderAttack(move, attackerColor))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
