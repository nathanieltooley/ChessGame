using ChessGame.Scripts.Boards;
using ChessGame.Scripts.DataTypes;
using ChessGame.Scripts.Factories;
using ChessGame.Scripts.Helpers;
using Godot;
using System.Collections.Generic;
namespace ChessGame.Scripts.Controllers
{
    public static class MoveController
    {
        public static List<BoardPos>[,] CreateMoveCache(PieceInfo[,] board)
        {
            List<BoardPos>[,] boardPosMatrix = new List<BoardPos>[8, 8];

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    BoardPos piecePos = new BoardPos(rank, file);
                    PieceInfo pieceInfo = BoardDataHandler.GetPieceInfoAtPos(board, piecePos);

                    if (pieceInfo.PieceId != ChessPieceId.Empty)
                    {
                        boardPosMatrix[rank, file] = MoveFinder.GetCapableMoves(piecePos, pieceInfo, board, ServiceFactory.GetGameInfoService());
                    }
                }
            }

            return boardPosMatrix;
        }

        public static bool IsTileUnderAttack(PieceInfo[,] board, BoardPos boardPos, ChessColor attackerColor, List<BoardPos>[,] moveCache)
        {
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    var pos = new BoardPos(rank, file);

                    PieceInfo piece = BoardDataHandler.GetPieceInfoAtPos(board, pos);
                    if (piece.PieceId == ChessPieceId.Empty || piece.Color != attackerColor)
                    {
                        continue;
                    }
                    else
                    {
                        var moves = moveCache[pos.Rank, pos.File];
                        foreach (var move in moves)
                        {
                            if (move == boardPos)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public static bool CheckCheck(PieceInfo[,] board, BoardPos kingPos, ChessColor attackerColor, List<BoardPos>[,] moveCache)
        {
            return IsTileUnderAttack(board, kingPos, attackerColor, moveCache);
        }

        public static bool CheckMateCheck(PieceInfo[,] board, BoardPos kingPos, ChessColor attackerColor, List<BoardPos> kingsMoves, List<BoardPos>[,] moveCache)
        {
            List<BoardPos> attackerPositions = new List<BoardPos>();
            // Check to see if king's position is under attack
            if (!IsTileUnderAttack(board, kingPos, attackerColor, moveCache))
            {
                return false;
            }

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    PieceInfo tileInfo = BoardDataHandler.GetPieceInfoAtPos(board, new BoardPos(rank, file));

                    if (tileInfo.PieceId == ChessPieceId.Empty)
                    {
                        continue;
                    }

                    if (moveCache[rank, file].FindAll(x => x.Rank == kingPos.Rank && x.File == kingPos.File).Count > 0) {
                        attackerPositions.Add(new BoardPos(rank, file));
                    }
                }
            }

            // Check all of the king's moves to see if those are under attack as well
            foreach (var move in kingsMoves)
            {
                if (!IsTileUnderAttack(board, move, attackerColor, moveCache))
                {
                    return false;
                }
            }

            // Check if any of the tiles between the attacker and the king are able to be blocked by friendly pieces

            foreach (var attackerPos in attackerPositions)
            {
                PieceInfo attacker = BoardDataHandler.GetPieceInfoAtPos(board, attackerPos);
                
                // Skip knight since he can jump over pieces
                if (attacker.PieceId == ChessPieceId.Knight)
                {
                    continue;
                }

                Vector2I line = MoveHelpers.GetLineMoveIsOn(attackerPos.Rank, attackerPos.File, kingPos.Rank, kingPos.File);

                foreach (BoardPos pos in MoveHelpers.GetSpacesOnLine(attackerPos, line, board))
                {
                    if (IsTileUnderAttack(board, pos, MiscHelpers.InvertColor(attackerColor), moveCache))
                    {
                        return false;
                    }
                }
            }

            

            return true;
        }
    }
}
