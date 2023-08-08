using ChessGame.Scripts.ChessBoard;
using ChessGame.Scripts.ChessBoard.Boards;
using ChessGame.Scripts.DataTypes;
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
        private LogicalBoard _logicBoard { get; set; }

        private List<PieceInfo> _whitePieces = new List<PieceInfo>();
        private List<PieceInfo> _blackPieces = new List<PieceInfo>();

        private ChessColor _playerColor;
        private static string _startingFenString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

        private TimerService _timerService;
        private TurnService _turnService;
        private GameInfoService _gameInfoService;
        private MoveInfoService _moveInfoService;

        [Signal]
        public delegate void ColorIsInCheckUpdateEventHandler(ChessColor color, bool inCheck);

        public override void _Ready()
        {
            _gameInfoService = ServiceHelpers.GetGameInfoService();
            _timerService = ServiceHelpers.GetTimerService();
            _turnService = ServiceHelpers.GetTurnService();
            _moveInfoService = ServiceHelpers.GetMoveInfoService();

            _gBoard = GetNode<GraphicalBoard>("/root/Main/Boards/GraphicalBoard");
            _logicBoard = GetNode<LogicalBoard>("/root/Main/Boards/LogicalBoard");

            _playerColor = _gameInfoService.PlayerSideColor;
        }

        public void CreateDefaultBoard()
        {
            CreateBoard(_startingFenString);
        }

        public void CreateBoard(string fenString)
        {
            PieceInfo[,] fenBoard = GetBoardFromFEN(fenString);
            UpdateBoard(fenBoard);
            UpdateMoveCache();
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
            List<BoardPos> capableMoves = _moveInfoService.GetMovesAtPos(pos);

            if (movingPieceInfo.Color != _turnService.GetCurrentTurnColor())
            {
                return false;
            }

            var result = capableMoves.SingleOrDefault((pos) => pos == targetPos);
            if (result != null)
            {
                _logicBoard.MovePiece(pos, targetPos);
                _gBoard.MovePiece(pos, movingPieceInfo, targetPos);

                _turnService.SwitchTurn();

                UpdateMoveCache();
                SendFENUpdate();

                bool whiteInCheck = WhiteCheckCheck();
                bool blackInCheck = BlackCheckCheck();

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
            List<BoardPos> moves = _moveInfoService.GetMovesAtPos(pos);
            return moves;
        }

        public Vector2 GetTileCenter(BoardPos pos)
        {
            return GraphicalBoard.CalculateTileCenter(pos);
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

        private void UpdateMoveCache()
        {
            List<BoardPos>[,] boardPosMatrix = new List<BoardPos>[8, 8];

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    BoardPos piecePos = new BoardPos(rank, file);
                    PieceInfo pieceInfo = _logicBoard.GetPieceInfoAtPos(piecePos);

                    if (pieceInfo.PieceId != ChessPieceId.Empty)
                    {
                        MoveFinder mf = new MoveFinder(_logicBoard, pieceInfo, piecePos, pieceInfo.Color);
                        boardPosMatrix[rank, file] = mf.GetCapableMoves(piecePos);
                    }
                }
            }

            _moveInfoService.MoveCache = boardPosMatrix;
        }
        private bool WhiteCheckCheck()
        {
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    var pos = new BoardPos(rank, file);

                    PieceInfo piece = _logicBoard.GetPieceInfoAtPos(pos);
                    if (piece.PieceId == ChessPieceId.Empty || piece.Color == ChessColor.White)
                    {
                        continue;
                    }
                    else
                    {
                        var moves = _moveInfoService.GetMovesAtPos(pos);
                        foreach (var move in moves)
                        {
                            PieceInfo pieceAtTarget = _logicBoard.GetPieceInfoAtPos(move);

                            if (pieceAtTarget.PieceId == ChessPieceId.King && pieceAtTarget.Color == ChessColor.White)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool BlackCheckCheck()
        {
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    var pos = new BoardPos(rank, file);

                    PieceInfo piece = _logicBoard.GetPieceInfoAtPos(pos);
                    if (piece.PieceId == ChessPieceId.Empty || piece.Color == ChessColor.Black)
                    {
                        continue;
                    }
                    else
                    {
                        var moves = _moveInfoService.GetMovesAtPos(pos);
                        foreach (var move in moves)
                        {
                            PieceInfo pieceAtTarget = _logicBoard.GetPieceInfoAtPos(move);

                            if (pieceAtTarget.PieceId == ChessPieceId.King && pieceAtTarget.Color == ChessColor.Black)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
