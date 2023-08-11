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

        private ChessColor _pieceMoverColor;
        private GameInfoService _gameInfoService;
        private bool _isPlayer;

        public MoveFinder(ILogicalBoard board, PieceInfo piece, BoardPos piecePos, ChessColor pieceMoverColor)
        {
            _gameInfoService = ServiceFactory.GetGameInfoService();

            _board = board;
            _piece = piece;
            _piecePos = piecePos;
            _isPlayer = _gameInfoService.IsPlayerColor(pieceMoverColor);
            _pieceMoverColor = pieceMoverColor;
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
                        if ((pieceAtTarget.PieceId != ChessPieceId.Empty) && pieceAtTarget.Color != _pieceMoverColor)
                        {
                            capableMoves.Add(move);
                            continue;
                        }
                        // Remove diagonal attack if there is no enemy piece
                        else
                        {
                            continue;
                        }
                    }
                }

                // if this move is closer to the piece than the block
                // or if knight
                if (absStartDistance < absBlockDistance || _piece.PieceId == ChessPieceId.Knight)
                {
                    capableMoves.Add(move);
                }

                // Piece capture check, make sure that pawn can only capture diagonally
                if ((pieceAtTarget.Color != _pieceMoverColor) && (pos.Rank == block.X) && (pos.File == block.Y) && _piece.PieceId != ChessPieceId.Pawn)
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
                    return MoveCreators.GetPawnMoves(startingRank, startingFile, _isPlayer);
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
