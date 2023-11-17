
using ChessGame.Scripts.DataTypes;
using ChessGame.Scripts.Helpers;
using Godot;
using System.Collections.Generic;
using System.Linq;

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

        public static List<BoardPos> GetAllAttackerPositions(PieceInfo[,] board, BoardPos posAttacked, List<BoardPos>[,] moveCache, ChessColor ignoreColor)
        {
            List<BoardPos> attackerPositions = new List<BoardPos>();
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    PieceInfo tileInfo = BoardDataHandler.GetPieceInfoAtPos(board, new BoardPos(rank, file));

                    // Ignore if there is no piece
                    if (tileInfo.PieceId == ChessPieceId.Empty)
                    {
                        continue;
                    }

                    // Ignore piece if it is the wrong color
                    if (tileInfo.Color == ignoreColor)
                    {
                        continue;
                    }

                    // Don't look at the piece that is being attacked, for obvious reasons
                    if (posAttacked == new BoardPos(rank, file))
                    {
                        continue;
                    }

                    if (moveCache[rank, file].FindAll(x => x.Rank == posAttacked.Rank && x.File == posAttacked.File).Count > 0)
                    {
                        attackerPositions.Add(new BoardPos(rank, file));
                    }
                }
            }

            return attackerPositions;
        }

        public static List<BoardPos> GetAllBlockingPiecePositions(PieceInfo[,] board, List<BoardPos>[,] moveCache, BoardPos kingPos, BoardPos attackerPosition, ChessColor attackerColor)
        {
            List<BoardPos> blockingPositions = new List<BoardPos>();

            PieceInfo attacker = BoardDataHandler.GetPieceInfoAtPos(board, attackerPosition);

            Vector2I line = MoveHelpers.GetLineMoveIsOn(attackerPosition.Rank, attackerPosition.File, kingPos.Rank, kingPos.File);

            foreach (BoardPos pos in MoveHelpers.GetSpacesOnLine(attackerPosition, line, board))
            {
                blockingPositions = blockingPositions.Concat(GetAllAttackerPositions(board, pos, moveCache, attackerColor)).ToList();
            }
            
            // Don't add the king to these positions
            blockingPositions.RemoveAll(x => x.Rank == kingPos.Rank && x.File == kingPos.File);
            return blockingPositions;
        }
    }
}
