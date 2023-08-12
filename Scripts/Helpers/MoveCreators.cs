using System.Collections.Generic;

namespace ChessGame.Scripts.Helpers
{
    public static class MoveCreators
    {
        public static List<BoardPos> GetPawnMoves(int startingRank, int startingFile, ChessColor color)
        {
            List<BoardPos > moves = new List<BoardPos>();

            if (color == ChessColor.Black)
            {
                if (startingRank == 1)
                {
                    AddValidMove(startingRank + MoveDownBoard(1), startingFile, moves);
                    AddValidMove(startingRank + MoveDownBoard(2), startingFile, moves);
                }
                else
                {
                    AddValidMove(startingRank + MoveDownBoard(1), startingFile, moves);
                }

                AddValidMove(startingRank + MoveDownBoard(1), startingFile + MoveLeft(1), moves);
                AddValidMove(startingRank + MoveDownBoard(1), startingFile + MoveRight(1), moves);
            }

            if (color == ChessColor.White)
            {
                if (startingRank == 6)
                {
                    AddValidMove(startingRank + MoveUpBoard(1), startingFile, moves);
                    AddValidMove(startingRank + MoveUpBoard(2), startingFile, moves);
                }
                else
                {
                    AddValidMove(startingRank + MoveUpBoard(1), startingFile, moves);
                }

                AddValidMove(startingRank + MoveUpBoard(1), startingFile + MoveLeft(1), moves);
                AddValidMove(startingRank + MoveUpBoard(1), startingFile + MoveRight(1), moves);
            }

            return moves;
        }

        public static List<BoardPos> GetRookMoves(int startingRank, int startingFile)
        {
            List<BoardPos> moves = new List<BoardPos>();

            for (int i = 1; i < 9; i++)
            {
                // Up
                AddValidMove(startingRank + MoveUpBoard(i), startingFile, moves);

                // Down
                AddValidMove(startingRank + MoveDownBoard(i), startingFile, moves);

                // Left
                AddValidMove(startingRank, startingFile + MoveLeft(i), moves);

                // Right
                AddValidMove(startingRank, startingFile + MoveRight(i), moves);
            }

            return moves;
        }

        public static List<BoardPos> GetKnightMoves(int startingRank, int startingFile)
        {
            List<BoardPos> moves = new List<BoardPos>();

            // Up
            AddValidMove(startingRank + MoveUpBoard(2), startingFile + MoveLeft(1), moves);
            AddValidMove(startingRank + MoveUpBoard(2), startingFile + MoveRight(1), moves);

            // Down
            AddValidMove(startingRank + MoveDownBoard(2), startingFile + MoveLeft(1), moves);
            AddValidMove(startingRank + MoveDownBoard(2), startingFile + MoveRight(1), moves);

            // Right
            AddValidMove(startingRank + MoveUpBoard(1), startingFile + MoveRight(2), moves);
            AddValidMove(startingRank + MoveDownBoard(1), startingFile + MoveRight(2), moves);

            // Left
            AddValidMove(startingRank + MoveUpBoard(1), startingFile + MoveLeft(2), moves);
            AddValidMove(startingRank + MoveDownBoard(1), startingFile + MoveLeft(2), moves);

            return moves;
        }

        public static List<BoardPos> GetBishopMoves(int startingRank, int startingFile)
        {
            List<BoardPos> moves = new List<BoardPos>();

            for (int i = 1; i < 9; i++)
            {
                // Diagonal NW
                AddValidMove(startingRank + MoveUpBoard(i), startingFile + MoveLeft(i), moves);

                // Diagonal NE
                AddValidMove(startingRank + MoveUpBoard(i), startingFile + MoveRight(i), moves);

                // Diagonal SW
                AddValidMove(startingRank + MoveDownBoard(i), startingFile + MoveLeft(i), moves);

                // Diagonal SW
                AddValidMove(startingRank + MoveDownBoard(i), startingFile + MoveRight(i), moves);
            }

            return moves;
        }

        public static List<BoardPos> GetQueenMoves(int startingRank, int startingFile)
        {
            List<BoardPos> moves = new List<BoardPos>();

            moves.AddRange(GetBishopMoves(startingRank, startingFile));
            moves.AddRange(GetRookMoves(startingRank, startingFile));

            return moves;
        }

        public static List<BoardPos> GetKingMoves(int startingRank, int startingFile)
        {
            List<BoardPos> moves = new List<BoardPos>();

            // Up
            AddValidMove(startingRank + MoveUpBoard(1), startingFile, moves);
            // Down
            AddValidMove(startingRank + MoveDownBoard(1), startingFile, moves);
            // Left
            AddValidMove(startingRank, startingFile + MoveLeft(1), moves);
            // Right    
            AddValidMove(startingRank, startingFile + MoveRight(1), moves);

            // NW
            AddValidMove(startingRank + MoveUpBoard(1), startingFile + MoveLeft(1), moves);
            // NE
            AddValidMove(startingRank + MoveUpBoard(1), startingFile + MoveRight(1), moves);
            // SW
            AddValidMove(startingRank + MoveDownBoard(1), startingFile + MoveLeft(1), moves);
            // SE
            AddValidMove(startingRank + MoveDownBoard(1), startingFile + MoveRight(1), moves);

            // Castling Moves
            AddValidMove(startingRank, startingFile + MoveRight(2), moves);
            AddValidMove(startingRank, startingFile + MoveLeft(2), moves);

            return moves;
        }

        public static int MoveUpBoard(int amount)
        {
            return amount * -1;
        }

        public static int MoveDownBoard(int amount)
        {
            return amount;
        }

        public static int MoveRight(int amount)
        {
            return amount;
        }

        public static int MoveLeft(int amount)
        {
            return amount * -1;
        }

        private static void AddValidMove(int rank, int file, List<BoardPos> moves)
        {
            if (rank < 0 || rank > 7 || file < 0 || file > 7)
            {
                return;
            }
            else
            {
                moves.Add(new BoardPos(rank, file));
            }
        }
    }
}
