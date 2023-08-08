using ChessGame.Scripts.Controllers;
using ChessGame.Scripts.DataTypes;
using Godot;
using System.Collections.Generic;

namespace ChessGame.Scripts.ChessBoard
{
    public partial class LogicalBoard : Node
    {
        private PieceInfo[,] _board = new PieceInfo[8,8];

        public override void _Ready()
        {
            // Init Board
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    _board[rank, file] = new PieceInfo { Color = ChessColor.White, PieceId = ChessPieceId.Empty };
                }
            }
        }

        public PieceInfo GetPieceInfoAtPos(BoardPos boardPos)
        {
            return _board[boardPos.Rank, boardPos.File];
        }

        public void AddPiece(BoardPos boardPos, PieceInfo piece)
        {
            _board[boardPos.Rank, boardPos.File] = piece;
        }

        public void RemovePiece(BoardPos boardPos)
        {
            _board[boardPos.Rank, boardPos.File] = PieceInfo.GetEmptyPiece();
        }

        public void MovePiece(BoardPos startingPos, BoardPos targetPos)
        {
            PieceInfo startingPieceInfo = GetPieceInfoAtPos(startingPos);

            AddPiece(targetPos, startingPieceInfo);
            RemovePiece(startingPos);
        }   

        public void ClearBoard()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    _board[i, j] = PieceInfo.GetEmptyPiece();
                }
            }
        }

        public PieceInfo[,] GetBoard()
        {
            return _board;
        }
    }
}
