
using ChessGame.Scripts.DataTypes;

namespace ChessGame.Scripts.Boards
{
    public static class BoardSearching
    {
        public static BoardPos GetKingPos(PieceInfo[,] board, ChessColor kingColor)
        {
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    var boardPos = new BoardPos(rank, file);
                    var pieceAtTile = BoardDataHandler.GetPieceInfoAtPos(board, boardPos);

                    if (pieceAtTile.PieceId == ChessPieceId.King && pieceAtTile.Color == kingColor)
                    {
                        return boardPos;
                    }
                }
            }

            return null;
        }
    }
}
