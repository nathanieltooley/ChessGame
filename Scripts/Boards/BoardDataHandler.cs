using ChessGame.Scripts.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Scripts.Boards
{
    public static class BoardDataHandler
    {

        public static PieceInfo[,] CreateNewBoard()
        {
            PieceInfo[,] board = new PieceInfo[8, 8];

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    board[rank, file] = new PieceInfo { Color = ChessColor.White, PieceId = ChessPieceId.Empty };
                }
            }

            return board;
        }

        public static PieceInfo GetPieceInfoAtPos(PieceInfo[,] board, BoardPos boardPos)
        {
            return board[boardPos.Rank, boardPos.File];
        }

        public static void AddPiece(PieceInfo[,] board, BoardPos boardPos, PieceInfo piece)
        {
            board[boardPos.Rank, boardPos.File] = piece;
        }

        public static void RemovePiece(PieceInfo[,] board, BoardPos boardPos)
        {
            board[boardPos.Rank, boardPos.File] = PieceInfo.GetEmptyPiece();
        }

        public static void MovePiece(PieceInfo[,] board, BoardPos startingPos, BoardPos targetPos)
        {
            PieceInfo startingPieceInfo = GetPieceInfoAtPos(board, startingPos);

            AddPiece(board, targetPos, startingPieceInfo);
            RemovePiece(board, startingPos);
        }

        public static void ClearBoard(PieceInfo[,] board)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j] = PieceInfo.GetEmptyPiece();
                }
            }
        }
    }
}
