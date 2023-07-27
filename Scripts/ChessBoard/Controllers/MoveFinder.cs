using ChessGame.Scripts.DataTypes;
using Godot;
using System;
using System.Collections.Generic;

namespace ChessGame.Scripts.ChessBoard.Controllers
{
    public class MoveFinder
    {

        private LogicalBoard _board;
        private PieceInfo _piece;
        private BoardPos _piecePos;
        private bool _isPlayer;

        private List<BoardPos> _capableMoves;

        public MoveFinder()
        {
            
        }

        public MoveFinder(LogicalBoard board, PieceInfo piece, BoardPos piecePos, bool isPlayer)
        {
            _board = board;
            _piece = piece;
            _piecePos = piecePos;
            _isPlayer = isPlayer;
        }

        public static bool IsMovePossible(BoardPos targetPos, List<BoardPos> capableMoves)
        {
            foreach (var move in capableMoves)
            {
                if (move == targetPos)
                {
                    return true;
                }
            }

            return false;
        }

        public List<BoardPos> GetCapableMoves(int startingRank, int startingFile, List<BoardPos> theoreticalMoves)
        {
            // copy list
            List<BoardPos> capableMoves = new List<BoardPos>();
            Dictionary<Vector2I, Vector2I> blockedDict = new Dictionary<Vector2I, Vector2I>();

            int tilesLeft;
            int tilesRight;
            int tilesUp;
            int tilesDown;

            Vector2I blockOnNWD = GetPointWhereLineIsBlocked(ChessConstants.NorthWestDiagonal, startingRank, startingFile, _board);
            Vector2I blockOnNED = GetPointWhereLineIsBlocked(ChessConstants.NorthEastDiagonal, startingRank, startingFile, _board);
            Vector2I blockOnSWD = GetPointWhereLineIsBlocked(ChessConstants.SouthWestDiagonal, startingRank, startingFile, _board);
            Vector2I blockOnSED = GetPointWhereLineIsBlocked(ChessConstants.SouthEastDiagonal, startingRank, startingFile, _board);

            Vector2I blockLeft = GetPointWhereLineIsBlocked(ChessConstants.LeftFile, startingRank, startingFile, _board);
            Vector2I blockRight = GetPointWhereLineIsBlocked(ChessConstants.RightFile, startingRank, startingFile, _board);
            Vector2I blockUp = GetPointWhereLineIsBlocked(ChessConstants.UpRank, startingRank, startingFile, _board);
            Vector2I blockDown = GetPointWhereLineIsBlocked(ChessConstants.DownRank, startingRank, startingFile, _board);

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
                int rank = move.Rank;
                int file = move.File;

                BoardPos pos = new BoardPos(rank, file);

                // Out of bounds Check
                if ((rank > 7 || rank < 0) || (file > 7 || file < 0))
                {
                    continue;
                }

                PieceInfo pieceAtTarget = _board.GetPieceInfoAtPos(pos);

                // Same color Check
                if (pieceAtTarget.Color == _piece.Color && pieceAtTarget.PieceId != ChessPieceId.Empty)
                { 
                    continue;
                }

                Vector2I distanceFromStartingTile = GetDistanceFromStart(startingRank, startingFile, rank, file);
                Vector2I line = GetLineMoveIsOn(startingRank, startingFile, rank, file);

                Vector2I block;
                blockedDict.TryGetValue(line, out block);

                Vector2I blockDistance = GetDistanceFromStart(startingRank, startingFile, block.X, block.Y);

                if (distanceFromStartingTile > blockDistance)
                {
                    capableMoves.Add(move);
                }
            }

            return capableMoves;
        }

        public List<BoardPos> GetMovesAssumingEmptyBoard()
        {
            ChessPieceId pieceId = _piece.PieceId;

            int startingRank = _piecePos.Rank;
            int startingFile = _piecePos.File;

            List<BoardPos> moves = new List<BoardPos>();

            // Player starts at bottom
            // Ai starts at the top

            // Top is rank 0, bottom is rank 7

            switch (pieceId)
            {
                case ChessPieceId.Pawn:
                    PawnMoves(startingRank, startingFile, moves);
                    break;
                case ChessPieceId.Rook:
                    RookMoves(startingRank, startingFile, moves);
                    break;
                case ChessPieceId.Knight:
                    KnightMoves(startingRank, startingFile, moves);
                    break;
                case ChessPieceId.Bishop:
                    BishopMoves(startingRank, startingFile, moves);
                    break;
                case ChessPieceId.Queen:
                    QueenMoves(startingRank, startingFile, moves);
                    break;
                case ChessPieceId.King:
                    KingMoves(startingRank, startingFile, moves);
                    break;
            }

            return moves;
        }

        // Foward for player is -rank
        // Foward for AI is +rank
        private int MoveUpBoard(int amount)
        {
            return amount * -1;
        }

        // Back for player is +rank
        // Back for ai is -rank
        private int MoveDownBoard(int amount)
        {
            return amount;
        }

        // Right for player is +rank
        // Right for AI is -rank
        private int MoveRight(int amount)
        {
            return amount;
        }

        // Left for player is -rank
        // Left for ai is +rank
        private int MoveLeft(int amount)
        {
            return amount * -1;
        }

        private void PawnMoves(int startingRank, int startingFile, List<BoardPos> moves)
        {
            if (startingRank == 1 && !_isPlayer)
            {
                moves.Add(new BoardPos(startingRank + MoveDownBoard(1), startingFile));
                moves.Add(new BoardPos(startingRank + MoveDownBoard(2), startingFile));
            } else if (!_isPlayer)
            {
                moves.Add(new BoardPos(startingRank + MoveDownBoard(1), startingFile));
            }

            if (startingRank == 6 && _isPlayer)
            {
                moves.Add(new BoardPos(startingRank + MoveUpBoard(1), startingFile));
                moves.Add(new BoardPos(startingRank + MoveUpBoard(2), startingFile));
            } else if (_isPlayer)
            {
                moves.Add(new BoardPos(startingRank + MoveUpBoard(1), startingFile));
            }
        }

        private void RookMoves(int startingRank, int startingFile, List<BoardPos> moves)
        {
            for (int i = 0; i < 8; i++)
            {
                // Up
                moves.Add(new BoardPos(startingRank + MoveUpBoard(i), startingFile));

                // Down
                moves.Add(new BoardPos(startingRank + MoveDownBoard(i), startingFile));

                // Left
                moves.Add(new BoardPos(startingRank, startingFile + MoveLeft(i)));

                // Right
                moves.Add(new BoardPos(startingRank, startingFile + MoveRight(i)));
            }
        }

        private void KnightMoves(int startingRank, int startingFile, List<BoardPos> moves)
        {
            // Up
            moves.Add(new BoardPos(startingRank + MoveUpBoard(3), startingFile + MoveLeft(1)));
            moves.Add(new BoardPos(startingRank + MoveUpBoard(3), startingFile + MoveRight(1)));

            // Down
            moves.Add(new BoardPos(startingRank + MoveDownBoard(3), startingFile + MoveLeft(1)));
            moves.Add(new BoardPos(startingRank + MoveDownBoard(3), startingFile + MoveRight(1)));

            // Right
            moves.Add(new BoardPos(startingRank + MoveUpBoard(1), startingFile + MoveRight(3)));
            moves.Add(new BoardPos(startingRank + MoveDownBoard(1), startingFile + MoveRight(3)));

            // Left
            moves.Add(new BoardPos(startingRank + MoveUpBoard(1), startingFile + MoveLeft(3)));
            moves.Add(new BoardPos(startingRank + MoveDownBoard(1), startingFile + MoveLeft(3)));
        }

        private void BishopMoves(int startingRank, int startingFile, List<BoardPos> moves)
        {
            for (int i = 0; i < 8; i++)
            {
                // Diagonal NW
                moves.Add(new BoardPos(startingRank + MoveUpBoard(i), startingFile + MoveLeft(i)));

                // Diagonal NE
                moves.Add(new BoardPos(startingRank + MoveUpBoard(i), startingFile + MoveRight(i)));

                // Diagonal SW
                moves.Add(new BoardPos(startingRank + MoveDownBoard(i), startingFile + MoveLeft(i)));

                // Diagonal SW
                moves.Add(new BoardPos(startingRank + MoveDownBoard(i), startingFile + MoveRight(i)));
            }
        }

        private void QueenMoves(int startingRank, int startingFile, List<BoardPos> moves)
        {
            BishopMoves(startingRank, startingFile, moves);
            RookMoves(startingRank, startingFile, moves);
        }

        private void KingMoves(int startingRank, int startingFile, List<BoardPos> moves)
        {

            // Add castling at some point

            // Up
            moves.Add(new BoardPos(startingRank + MoveUpBoard(1), startingFile));
            // Down
            moves.Add(new BoardPos(startingRank + MoveDownBoard(1), startingFile));
            // Left
            moves.Add(new BoardPos(startingRank, startingFile + MoveLeft(1)));
            // Right    
            moves.Add(new BoardPos(startingRank, startingFile + MoveRight(1)));

            // NW
            moves.Add(new BoardPos(startingRank + MoveUpBoard(1), startingFile + MoveLeft(1)));
            // NE
            moves.Add(new BoardPos(startingRank + MoveUpBoard(1), startingFile + MoveRight(1)));
            // SW
            moves.Add(new BoardPos(startingRank + MoveDownBoard(1), startingFile + MoveLeft(1)));
            // SE
            moves.Add(new BoardPos(startingRank + MoveDownBoard(1), startingFile + MoveRight(1)));

        }

        private Vector2I GetPointWhereLineIsBlocked(Vector2I diagonal, int startingRank, int startingFile, LogicalBoard board)
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

                if ((rankOOB || fileOOB) || board.GetPieceInfoAtPos(new BoardPos(tempRank, tempFile)).PieceId != ChessPieceId.Empty)
                {
                    return new Vector2I(tempRank, tempFile);
                }

                i++;
            }
        }

        private Vector2I GetDistanceFromStart(int startingRank, int startingFile, int targetRank, int targetFile)
        {
            Vector2I distanceFromStart = new Vector2I(targetRank - startingRank, targetFile - startingFile);
            return distanceFromStart;
        }

        private Vector2I GetLineMoveIsOn(int startingRank, int startingFile, int targetRank, int targetFile)
        {
            Vector2I distance = GetDistanceFromStart(startingRank, startingFile, targetRank, targetFile);

            int rankMag = Math.Abs(distance.X);
            int fileMag = Math.Abs(distance.Y);

            Vector2I resultVec = new Vector2I();

            resultVec.X = rankMag == 0 ? 0 : (distance.X / rankMag);
            resultVec.Y = fileMag == 0 ? 0 : (distance.Y / fileMag);

            return resultVec;
        }
    }
}
