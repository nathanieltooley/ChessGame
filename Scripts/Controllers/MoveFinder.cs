using ChessGame.Scripts.Boards;
using ChessGame.Scripts.ChessBoard;
using ChessGame.Scripts.DataTypes;
using ChessGame.Scripts.Factories;
using ChessGame.Scripts.Helpers;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessGame.Scripts.Controllers
{
    public static class MoveFinder
    {

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

        public static List<BoardPos> GetCapableMoves(BoardPos piecePos, PieceInfo piece, PieceInfo[,] board, GameInfoService gameInfoService)
        {
            int startingRank = piecePos.Rank;
            int startingFile = piecePos.File;

            ChessColor opposingColor = MiscHelpers.InvertColor(piece.Color);

            List<BoardPos> theoreticalMoves = GetMovesAssumingEmptyBoard(piece, piecePos);
            List<BoardPos> capableMoves = new List<BoardPos>();
            Dictionary<Vector2I, Vector2I> blockedDict = MoveHelpers.CreateBlockedDict(startingRank, startingFile, board);

            foreach (var move in theoreticalMoves)
            {
                int rank = move.Rank;
                int file = move.File;

                BoardPos pos = new BoardPos(rank, file);

                // Out of bounds Check
                if (rank > 7 || rank < 0 || file > 7 || file < 0)
                {
                    continue;
                }

                PieceInfo pieceAtTarget = BoardDataHandler.GetPieceInfoAtPos(board, pos);

                // Same color Check
                if (pieceAtTarget.Color == piece.Color && pieceAtTarget.PieceId != ChessPieceId.Empty)
                {
                    continue;
                }

                // r0 f7

                // these distances are in vector form rather than a magnitude
                Vector2I distanceFromStartingTile = MoveHelpers.GetDistanceFromStart(startingRank, startingFile, rank, file);
                Vector2I line = MoveHelpers.GetLineMoveIsOn(startingRank, startingFile, rank, file);

                Vector2I block;
                blockedDict.TryGetValue(line, out block);

                Vector2I blockDistance = MoveHelpers.GetDistanceFromStart(startingRank, startingFile, block.X, block.Y);

                Vector2I absStartDistance = distanceFromStartingTile.Abs();
                Vector2I absBlockDistance = blockDistance.Abs();

                if (piece.PieceId == ChessPieceId.Pawn)
                {
                    if (ChessConstants.DiagonalDirections.Contains(line))
                    {
                        // Pawn Diagonal Attack Check
                        if ((pieceAtTarget.PieceId != ChessPieceId.Empty) && pieceAtTarget.Color != piece.Color)
                        {
                            capableMoves.Add(move);
                            continue;
                        }
                        // Remove diagonal attack if there is no enemy piece
                        else
                        {
                            bool enPassantThisFile = gameInfoService.FileInEnPassantPosition(opposingColor, move.File);

                            if (enPassantThisFile && MoveHelpers.BehindEnpassantableRank(opposingColor, move.Rank))
                            {
                                capableMoves.Add(move);
                                continue;
                            }

                            continue;
                        }
                    }
                }

                // if this move is closer to the piece than the block
                // or if knight
                if (absStartDistance < absBlockDistance || piece.PieceId == ChessPieceId.Knight)
                {
                    // we'll deal with castling later in the function
                    if (piece.PieceId == ChessPieceId.King && MoveHelpers.IsCastleMove(piecePos, move))
                    {
                        continue;
                    } else
                    {
                        capableMoves.Add(move);
                    }
                    
                } else if (absStartDistance > absBlockDistance)
                {
                    continue;
                }

                // Castling Check
                if (piece.PieceId == ChessPieceId.King && MoveHelpers.IsCastleMove(piecePos, move))
                {
                    if (piece.Color == ChessColor.White && gameInfoService.WhiteAnyCastlePossible())
                    {
                        CastleSide? castle = MoveHelpers.GetCastlingDirection(move);

                        if (castle == CastleSide.KingSide && gameInfoService.CanWKingCastle)
                        {
                            capableMoves.Add(move);
                        }

                        if (castle == CastleSide.QueenSide && gameInfoService.CanWQueenCastle)
                        {
                            capableMoves.Add(move);
                        }
                    }

                    if (piece.Color == ChessColor.Black && gameInfoService.BlackAnyCastlePossible())
                    {
                        CastleSide? castle = MoveHelpers.GetCastlingDirection(move);

                        if (castle == CastleSide.KingSide && gameInfoService.CanBKingCastle)
                        {
                            capableMoves.Add(move);
                        }

                        if (castle == CastleSide.QueenSide && gameInfoService.CanBQueenCastle)
                        {
                            capableMoves.Add(move);
                        }
                    }
                }

                // Piece capture check, make sure that pawn can only capture diagonally
                if ((pieceAtTarget.Color != piece.Color) && (pos.Rank == block.X) && (pos.File == block.Y) && piece.PieceId != ChessPieceId.Pawn)
                {
                    capableMoves.Add(move);
                }
            }

            return capableMoves;
        }

        private static List<BoardPos> GetMovesAssumingEmptyBoard(PieceInfo piece, BoardPos piecePos)
        {
            ChessPieceId pieceId = piece.PieceId;

            int startingRank = piecePos.Rank;
            int startingFile = piecePos.File;

            // Player starts at bottom
            // Ai starts at the top

            // Top is rank 0, bottom is rank 7

            switch (pieceId)
            {
                case ChessPieceId.Pawn:
                    return MoveCreators.GetPawnMoves(startingRank, startingFile, piece.Color);
                case ChessPieceId.Rook:
                    return MoveCreators.GetRookMoves(startingRank, startingFile);
                case ChessPieceId.Knight:
                    return MoveCreators.GetKnightMoves(startingRank, startingFile);
                case ChessPieceId.Bishop:
                    return MoveCreators.GetBishopMoves(startingRank, startingFile);
                case ChessPieceId.Queen:
                    return MoveCreators.GetQueenMoves(startingRank, startingFile);
                case ChessPieceId.King:
                    return MoveCreators.GetKingMoves(startingRank, startingFile);
            }

            return null;
        }
    }
}
