using ChessGame.Scripts.ChessBoard;
using ChessGame.Scripts.DataTypes;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Scripts.Helpers
{
    public static class MoveHelpers
    {
        public static Dictionary<Vector2I, Vector2I> CreateBlockedDict(int startingRank, int startingFile, ILogicalBoard board)
        {
            Dictionary<Vector2I, Vector2I> blockedDict = new Dictionary<Vector2I, Vector2I>();

            Vector2I blockOnNWD = GetPointWhereLineIsBlocked(ChessConstants.NorthWestDiagonal, startingRank, startingFile, board);
            Vector2I blockOnNED = GetPointWhereLineIsBlocked(ChessConstants.NorthEastDiagonal, startingRank, startingFile, board);
            Vector2I blockOnSWD = GetPointWhereLineIsBlocked(ChessConstants.SouthWestDiagonal, startingRank, startingFile, board);
            Vector2I blockOnSED = GetPointWhereLineIsBlocked(ChessConstants.SouthEastDiagonal, startingRank, startingFile, board);

            Vector2I blockLeft = GetPointWhereLineIsBlocked(ChessConstants.LeftFile, startingRank, startingFile, board);
            Vector2I blockRight = GetPointWhereLineIsBlocked(ChessConstants.RightFile, startingRank, startingFile, board);
            Vector2I blockUp = GetPointWhereLineIsBlocked(ChessConstants.UpRank, startingRank, startingFile, board);
            Vector2I blockDown = GetPointWhereLineIsBlocked(ChessConstants.DownRank, startingRank, startingFile, board);

            blockedDict.Add(ChessConstants.NorthWestDiagonal, blockOnNWD);
            blockedDict.Add(ChessConstants.NorthEastDiagonal, blockOnNED);
            blockedDict.Add(ChessConstants.SouthWestDiagonal, blockOnSWD);
            blockedDict.Add(ChessConstants.SouthEastDiagonal, blockOnSED);

            blockedDict.Add(ChessConstants.LeftFile, blockLeft);
            blockedDict.Add(ChessConstants.RightFile, blockRight);
            blockedDict.Add(ChessConstants.UpRank, blockUp);
            blockedDict.Add(ChessConstants.DownRank, blockDown);

            return blockedDict;
        }

        public static Vector2I GetPointWhereLineIsBlocked(Vector2I diagonal, int startingRank, int startingFile, ILogicalBoard board)
        {
            int i = 1;

            int tempRank;
            int tempFile;

            while (true)
            {
                tempRank = startingRank + i * diagonal.X;
                tempFile = startingFile + i * diagonal.Y;

                bool rankOOB = tempRank < 0 || tempRank > 7;
                bool fileOOB = tempFile < 0 || tempFile > 7;

                if (rankOOB || fileOOB || board.GetPieceInfoAtPos(new BoardPos(tempRank, tempFile)).PieceId != ChessPieceId.Empty)
                {
                    return new Vector2I(tempRank, tempFile);
                }

                i++;
            }
        }

        public static Vector2I GetDistanceFromStart(int startingRank, int startingFile, int targetRank, int targetFile)
        {
            Vector2I distanceFromStart = new Vector2I(targetRank - startingRank, targetFile - startingFile);
            return distanceFromStart;
        }

        public static Vector2I GetLineMoveIsOn(int startingRank, int startingFile, int targetRank, int targetFile)
        {
            Vector2I distance = GetDistanceFromStart(startingRank, startingFile, targetRank, targetFile);

            int rankMag = Math.Abs(distance.X);
            int fileMag = Math.Abs(distance.Y);

            Vector2I resultVec = new Vector2I();

            resultVec.X = rankMag == 0 ? 0 : distance.X / rankMag;
            resultVec.Y = fileMag == 0 ? 0 : distance.Y / fileMag;

            return resultVec;
        }

        public static CastleSide? GetCastlingDirection(BoardPos movePos)
        {
            if (movePos.File == 6)
            {
                return CastleSide.KingSide;
            } else if (movePos.File == 2)
            {
                return CastleSide.QueenSide;
            }

            return null;
        }

        public static bool IsCastleMove(BoardPos piecePos, BoardPos movePos)
        {
            int fileDiff = Math.Abs(movePos.File - piecePos.File);

            if (fileDiff == 2)
            {
                return true;
            }

            return false;
        }

        public static BoardPos CastleMoveRook(CastleSide side, ChessColor color) 
        {
            if (color == ChessColor.White)
            {
                if (side == CastleSide.KingSide)
                {
                    return new BoardPos(7, 5);
                } else
                {
                    return new BoardPos(7, 3);
                }
            } else
            {
                if (side == CastleSide.KingSide)
                {
                    return new BoardPos(0, 5);
                } else
                {
                    return new BoardPos(0, 3);
                }
            }
        }

        public static BoardPos GetPosOfRook(ChessColor color, CastleSide cSide)
        {
            if (color == ChessColor.White)
            {
                if (cSide == CastleSide.KingSide)
                {
                    return new BoardPos(7, 7);
                } else
                {
                    return new BoardPos(7, 0);
                }
            } else
            {
                if (cSide == CastleSide.KingSide)
                {
                    return new BoardPos(0, 7);
                } else
                {
                    return new BoardPos(0, 0);
                }
            }
        }

        public static BoardPos GetPosOfRookAfterCastling(ChessColor color, CastleSide side)
        {
            if (color == ChessColor.White)
            {
                if (side == CastleSide.KingSide)
                {
                    return new BoardPos(7, 5);
                } else
                {
                    return new BoardPos(7, 3);
                }
            } else
            {
                if (side == CastleSide.KingSide)
                {
                    return new BoardPos(0, 5);
                } else
                {
                    return new BoardPos(0, 3);
                }
            }
        }
    }
}
