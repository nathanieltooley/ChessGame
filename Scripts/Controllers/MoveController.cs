using ChessGame.Scripts.Boards;
using ChessGame.Scripts.ChessBoard;
using ChessGame.Scripts.DataTypes;
using ChessGame.Scripts.Factories;
using ChessGame.Scripts.Helpers;
using System;
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

        public static bool CheckMateCheck(PieceInfo[,] board, BoardPos kingPos, ChessColor attackerColor, List<BoardPos> moves, List<BoardPos>[,] moveCache)
        {
            if (!IsTileUnderAttack(board, kingPos, attackerColor, moveCache))
            {
                return false;
            }

            foreach (var move in moves)
            {
                if (!IsTileUnderAttack(board, move, attackerColor, moveCache))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
