using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Scripts.ChessBoard.Controllers
{
    public static class MoveVerificator
    {
        public static bool IsMovePossible(BoardTile startingTile, BoardTile targetTile, LogicalBoard board)
        {
            return false;
        }

        public static List<Vector2I> GetCapableMoves(int startingRank, int startingFile, BoardTile startingTile, bool isPlayer, List<Vector2I> theoreticalMoves, LogicalBoard board)
        {
            // copy list
            List<Vector2I> capableMoves = new List<Vector2I>();
            Dictionary<Vector2I, Vector2I> blockedDict = new Dictionary<Vector2I, Vector2I>();

            ChessPieceId piece = startingTile.PieceId;

            int tilesLeft;
            int tilesRight;
            int tilesUp;
            int tilesDown;

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

            foreach (var move in theoreticalMoves)
            {
                int rank;
                int file;

                move.Deconstruct(out rank, out file);

                BoardTile targetTile = board.GetTile(rank, file);

                // Out of bounds Check
                if ((rank > 7 || rank < 0) || (file > 7 || file < 0))
                {
                    capableMoves.Add(move);
                    continue;
                }

                // Same color Check
                if (targetTile.PieceColor == startingTile.PieceColor)
                {
                    capableMoves.Add(move);
                    continue;
                }

                switch (piece)
                {
                    case ChessPieceId.Bishop:
                    case ChessPieceId.Rook:
                    case ChessPieceId.Queen:

                        Vector2I distanceFromStartingTile = GetDistanceFromStart(startingRank, startingFile, rank, file);
                        Vector2I line = GetLineMoveIsOn(startingRank, startingFile, rank, file);

                        Vector2I block;
                        blockedDict.TryGetValue(distanceFromStartingTile, out block);

                        Vector2I blockDistance = GetDistanceFromStart(startingRank, startingFile, block.X, block.Y);

                        if (distanceFromStartingTile > blockDistance)
                        {
                            capableMoves.Add(move);
                        }

                        break;
                }
            }

            return capableMoves;
        }

        public static List<Vector2I> GetMovesAssumingEmptyBoard(ChessPieceId pieceId, int startingRank, int startingFile, bool isPlayer)
        {
            List<Vector2I> moves = new List<Vector2I>();

            // Player starts at bottom
            // Ai starts at the top

            // Top is rank 0, bottom is rank 7

            switch (pieceId)
            {
                case ChessPieceId.Pawn:
                    PawnMoves(startingRank, startingFile, isPlayer, moves);
                    break;
                case ChessPieceId.Rook:
                    RookMoves(startingRank, startingFile, isPlayer, moves);
                    break;
                case ChessPieceId.Knight:
                    KnightMoves(startingRank, startingFile, isPlayer, moves);
                    break;
                case ChessPieceId.Bishop:
                    BishopMoves(startingRank, startingFile, isPlayer, moves);
                    break;
                case ChessPieceId.Queen:
                    QueenMoves(startingRank, startingFile, isPlayer, moves);
                    break;
                case ChessPieceId.King:
                    KingMoves(startingRank, startingFile, isPlayer, moves);
                    break;
            }

            return moves;
        }

        // Foward for player is -rank
        // Foward for AI is +rank
        private static int MoveUpBoard(int amount)
        {
            return amount * -1;
        }

        // Back for player is +rank
        // Back for ai is -rank
        private static int MoveDownBoard(int amount)
        {
            return amount;
        }

        // Right for player is +rank
        // Right for AI is -rank
        private static int MoveRight(int amount)
        {
            return amount;
        }

        // Left for player is -rank
        // Left for ai is +rank
        private static int MoveLeft(int amount)
        {
            return amount * -1;
        }

        private static void PawnMoves(int startingRank, int startingFile, bool isPlayer, List<Vector2I> moves)
        {
            if (startingRank == 1 && !isPlayer)
            {
                moves.Add(new Vector2I(startingRank + MoveDownBoard(1), startingFile));
                moves.Add(new Vector2I(startingRank + MoveDownBoard(2), startingFile));
            } else if (!isPlayer)
            {
                moves.Add(new Vector2I(startingRank + MoveDownBoard(1), startingFile));
            }

            if (startingRank == 6 && isPlayer)
            {
                moves.Add(new Vector2I(startingRank + MoveUpBoard(1), startingFile));
                moves.Add(new Vector2I(startingRank + MoveUpBoard(2), startingFile));
            } else if (isPlayer)
            {
                moves.Add(new Vector2I(startingRank + MoveUpBoard(1), startingFile));
            }
        }

        private static void RookMoves(int startingRank, int startingFile, bool isPlayer, List<Vector2I> moves)
        {
            for (int i = 0; i < 8; i++)
            {
                // Up
                moves.Add(new Vector2I(startingRank + MoveUpBoard(i), startingFile));

                // Down
                moves.Add(new Vector2I(startingRank + MoveDownBoard(i), startingFile));

                // Left
                moves.Add(new Vector2I(startingRank, startingFile + MoveLeft(i)));

                // Right
                moves.Add(new Vector2I(startingRank, startingFile + MoveRight(i)));
            }
        }

        private static void KnightMoves(int startingRank, int startingFile, bool isPlayer, List<Vector2I> moves)
        {
            // Up
            moves.Add(new Vector2I(startingRank + MoveUpBoard(3), startingFile + MoveLeft(1)));
            moves.Add(new Vector2I(startingRank + MoveUpBoard(3), startingFile + MoveRight(1)));

            // Down
            moves.Add(new Vector2I(startingRank + MoveDownBoard(3), startingFile + MoveLeft(1)));
            moves.Add(new Vector2I(startingRank + MoveDownBoard(3), startingFile + MoveRight(1)));

            // Right
            moves.Add(new Vector2I(startingRank + MoveUpBoard(1), startingFile + MoveRight(3)));
            moves.Add(new Vector2I(startingRank + MoveDownBoard(1), startingFile + MoveRight(3)));

            // Left
            moves.Add(new Vector2I(startingRank + MoveUpBoard(1), startingFile + MoveLeft(3)));
            moves.Add(new Vector2I(startingRank + MoveDownBoard(1), startingFile + MoveLeft(3)));
        }

        private static void BishopMoves(int startingRank, int startingFile, bool isPlayer, List<Vector2I> moves)
        {
            for (int i = 0; i < 8; i++)
            {
                // Diagonal NW
                moves.Add(new Vector2I(startingRank + MoveUpBoard(i), startingFile + MoveLeft(i)));

                // Diagonal NE
                moves.Add(new Vector2I(startingRank + MoveUpBoard(i), startingFile + MoveRight(i)));

                // Diagonal SW
                moves.Add(new Vector2I(startingRank + MoveDownBoard(i), startingFile + MoveLeft(i)));

                // Diagonal SW
                moves.Add(new Vector2I(startingRank + MoveDownBoard(i), startingFile + MoveRight(i)));
            }
        }

        private static void QueenMoves(int startingRank, int startingFile, bool isPlayer, List<Vector2I> moves)
        {
            BishopMoves(startingRank, startingFile, isPlayer, moves);
            RookMoves(startingRank, startingFile, isPlayer, moves);
        }

        private static void KingMoves(int startingRank, int startingFile, bool isPlayer, List<Vector2I> moves)
        {

            // Add castling at some point

            // Up
            moves.Add(new Vector2I(startingRank + MoveUpBoard(1), startingFile));
            // Down
            moves.Add(new Vector2I(startingRank + MoveDownBoard(1), startingFile));
            // Left
            moves.Add(new Vector2I(startingRank, startingFile + MoveLeft(1)));
            // Right
            moves.Add(new Vector2I(startingRank, startingFile + MoveRight(1)));

            // NW
            moves.Add(new Vector2I(startingRank + MoveUpBoard(1), startingFile + MoveLeft(1)));
            // NE
            moves.Add(new Vector2I(startingRank + MoveUpBoard(1), startingFile + MoveRight(1)));
            // SW
            moves.Add(new Vector2I(startingRank + MoveDownBoard(1), startingFile + MoveLeft(1)));
            // SE
            moves.Add(new Vector2I(startingRank + MoveDownBoard(1), startingFile + MoveRight(1)));

        }

        private static Vector2I GetPointWhereLineIsBlocked(Vector2I diagonal, int startingRank, int startingFile, LogicalBoard board)
        {
            int i = 1;

            int tempRank;
            int tempFile;

            while (true)
            {
                tempRank = startingRank + (i * diagonal.X);
                tempFile = startingFile + (i * diagonal.Y);

                bool rankOOB = (tempRank < 0 || tempRank > 7);
                bool fileOOB = (tempFile < 0 || tempFile > 7);

                if ((rankOOB || fileOOB) || board.GetTile(tempRank, tempFile).PieceId != ChessPieceId.Empty)
                {
                    return new Vector2I(tempRank, tempFile);
                }

                i++;
            }
        }

        private static Vector2I GetDistanceFromStart(int startingRank, int startingFile, int targetRank, int targetFile)
        {
            Vector2I distanceFromStart = new Vector2I(targetRank - startingRank, targetFile - startingFile);
            return distanceFromStart;
        }

        private static Vector2I GetLineMoveIsOn(int startingRank, int startingFile, int targetRank, int targetFile)
        {
            Vector2I distance = GetDistanceFromStart(startingRank, startingFile, targetRank, targetFile);

            int rankMag = Math.Abs(distance.X);
            int fileMag = Math.Abs(distance.Y);

            return (distance / new Vector2I(rankMag, fileMag));
        }
    }
}
