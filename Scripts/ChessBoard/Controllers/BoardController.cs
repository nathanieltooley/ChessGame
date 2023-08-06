using ChessGame.Scripts.ChessBoard.Boards;
using ChessGame.Scripts.DataTypes;
using ChessGame.Scripts.Helpers;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessGame.Scripts.ChessBoard.Controllers
{
    public class BoardController
    {
        private GraphicalBoard _gBoard { get; set; }
        private LogicalBoard _logicBoard { get; set; }

        private Func<ChessColor> _getCurrentTurn;

        private List<PieceInfo> _whitePieces = new List<PieceInfo>();
        private List<PieceInfo> _blackPieces = new List<PieceInfo>();

        private ChessColor _playerColor;
        private static string _startingFenString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
        private Action<string> _emitBoardStateUpdate;

        public BoardController(ChessColor playerColor, Node2D rootPieceNode, Action<string> boardStateUpdate, Func<ChessColor> getCurrentTurn)
        {
            _playerColor = playerColor;

            _logicBoard = new LogicalBoard();
            _gBoard = new GraphicalBoard(rootPieceNode);
            _emitBoardStateUpdate = boardStateUpdate;
            _getCurrentTurn = getCurrentTurn;

        }

        public void CreateDefaultBoard()
        {
            CreateBoard(_startingFenString);
        }

        public void CreateBoard(string fenString)
        {
            PieceInfo[,] fenBoard = GetBoardFromFEN(fenString);
            UpdateBoard(fenBoard);
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
            bool success;
            var movingPieceInfo = _logicBoard.GetPieceInfoAtPos(pos);

            _logicBoard.MovePiece(pos, targetPos, movingPieceInfo.Color == _playerColor, out success);

            if (success)
            {
                _gBoard.MovePiece(pos, movingPieceInfo, targetPos);
                SendFENUpdate();
            }

            return success;
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

        public List<BoardPos> GetMovesForPiece(BoardPos pos, bool isPlayerMove) 
        {
            var pieceInfo = _logicBoard.GetPieceInfoAtPos(pos);
            return _logicBoard.GetMovesForPiece(pos, isPlayerMove);
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
            _emitBoardStateUpdate(FEN.Encrypt(_logicBoard.GetBoard()));
        }

        
    }
}
