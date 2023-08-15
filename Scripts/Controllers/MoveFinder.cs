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
    public class MoveFinder
    {
        private ILogicalBoard _board;
        private PieceInfo _piece;
        private BoardPos _piecePos;

        private GameInfoService _gameInfoService;

        public MoveFinder(ILogicalBoard board, PieceInfo piece, BoardPos piecePos)
        {
            _gameInfoService = ServiceFactory.GetGameInfoService();

            _board = board;
            _piece = piece;
            _piecePos = piecePos;
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

        public List<BoardPos> GetCapableMoves(BoardPos piecePos)
        {
            int startingRank = piecePos.Rank;
            int startingFile = piecePos.File;

            ChessColor opposingColor = MiscHelpers.InvertColor(_piece.Color);

            List<BoardPos> theoreticalMoves = GetMovesAssumingEmptyBoard();
            List<BoardPos> capableMoves = new List<BoardPos>();
            Dictionary<Vector2I, Vector2I> blockedDict = MoveHelpers.CreateBlockedDict(startingRank, startingFile, _board);

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

                PieceInfo pieceAtTarget = _board.GetPieceInfoAtPos(pos);

                // Same color Check
                if (pieceAtTarget.Color == _piece.Color && pieceAtTarget.PieceId != ChessPieceId.Empty)
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

                if (_piece.PieceId == ChessPieceId.Pawn)
                {
                    if (ChessConstants.DiagonalDirections.Contains(line))
                    {
                        // Pawn Diagonal Attack Check
                        if ((pieceAtTarget.PieceId != ChessPieceId.Empty) && pieceAtTarget.Color != _piece.Color)
                        {
                            capableMoves.Add(move);
                            continue;
                        }
                        // Remove diagonal attack if there is no enemy piece
                        else
                        {
                            bool enPassantThisFile = _gameInfoService.FileInEnPassantPosition(opposingColor, move.File);

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
                if (absStartDistance < absBlockDistance || _piece.PieceId == ChessPieceId.Knight)
                {
                    // we'll deal with castling later in the function
                    if (_piece.PieceId == ChessPieceId.King && MoveHelpers.IsCastleMove(_piecePos, move))
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
                if (_piece.PieceId == ChessPieceId.King && MoveHelpers.IsCastleMove(_piecePos, move))
                {
                    if (_piece.Color == ChessColor.White && _gameInfoService.WhiteAnyCastlePossible())
                    {
                        CastleSide? castle = MoveHelpers.GetCastlingDirection(move);

                        if (castle == CastleSide.KingSide && _gameInfoService.CanWKingCastle)
                        {
                            capableMoves.Add(move);
                        }

                        if (castle == CastleSide.QueenSide && _gameInfoService.CanWQueenCastle)
                        {
                            capableMoves.Add(move);
                        }
                    }

                    if (_piece.Color == ChessColor.Black && _gameInfoService.BlackAnyCastlePossible())
                    {
                        CastleSide? castle = MoveHelpers.GetCastlingDirection(move);

                        if (castle == CastleSide.KingSide && _gameInfoService.CanBKingCastle)
                        {
                            capableMoves.Add(move);
                        }

                        if (castle == CastleSide.QueenSide && _gameInfoService.CanBQueenCastle)
                        {
                            capableMoves.Add(move);
                        }
                    }
                }

                // Piece capture check, make sure that pawn can only capture diagonally
                if ((pieceAtTarget.Color != _piece.Color) && (pos.Rank == block.X) && (pos.File == block.Y) && _piece.PieceId != ChessPieceId.Pawn)
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

            // Player starts at bottom
            // Ai starts at the top

            // Top is rank 0, bottom is rank 7

            switch (pieceId)
            {
                case ChessPieceId.Pawn:
                    return MoveCreators.GetPawnMoves(startingRank, startingFile, _piece.Color);
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
