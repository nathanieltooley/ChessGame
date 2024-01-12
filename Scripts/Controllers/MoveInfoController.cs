using ChessGame.Scripts.Boards;
using ChessGame.Scripts.DataTypes;
using ChessGame.Scripts.Helpers;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessGame.Scripts.Controllers
{
    class BlockInfo
    {
        public BoardPos pos;
        public PieceInfo prevBoardInfo;
    }

    public class MoveInfoController
    {

        // Watches are positions that a piece is attacking
        private List<BlockInfo>[,] _watches = new List<BlockInfo>[8, 8];

        // Pieces that have a block or
        private List<BoardPos> _piecesInPlay = new List<BoardPos>();

        private PieceInfo[,] _logicalBoard;
        private GameInfoService _gameInfoService;

        private List<BoardPos>[,] _possibleMoves = new List<BoardPos>[8, 8];

        public MoveInfoController(PieceInfo[,] board, GameInfoService gameInfoService)
        {
            _logicalBoard = board;
            _gameInfoService = gameInfoService;

            // Init Blocks and Watches
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    _watches[i, j] = new List<BlockInfo>();
                }
            }

            FullMoveCalcs();
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

        public static void CloneMoveInfo(MoveInfoController info1, MoveInfoController info2)
        {
            info1._logicalBoard = info2._logicalBoard;
            info1._possibleMoves = info2._possibleMoves;
        }

        private List<BoardPos> CalculateMovesAtPos(BoardPos piecePos, PieceInfo piece)
        {
            int startingRank = piecePos.Rank;
            int startingFile = piecePos.File;

            ChessColor opposingColor = MiscHelpers.InvertColor(piece.Color);

            List<BoardPos> theoreticalMoves = GetMovesAssumingEmptyBoard(piece, piecePos);
            List<BoardPos> capableMoves = new List<BoardPos>();
            Dictionary<Vector2I, Vector2I> blockedDict = MoveHelpers.CreateBlockedDict(startingRank, startingFile, _logicalBoard);

            _watches[startingRank, startingFile] = new List<BlockInfo>();

            foreach (var move in theoreticalMoves)
            {

                // Out of bounds Check
                if (move.Rank > 7 || move.Rank < 0 || move.File > 7 || move.File < 0)
                {
                    continue;
                }

                PieceInfo pieceAtTarget = BoardDataHandler.GetPieceInfoAtPos(_logicalBoard, move);

                // Same color Check
                if (pieceAtTarget.Color == piece.Color && pieceAtTarget.PieceId != ChessPieceId.Empty)
                {
                    AddPosToWatchList(piecePos, move);
                    continue;
                }

                // r0 f7

                // these distances are in vector form rather than a magnitude
                Vector2I distanceFromStartingTile = MoveHelpers.GetDistanceFromStart(startingRank, startingFile, move.Rank, move.File);
                Vector2I line = MoveHelpers.GetLineMoveIsOn(startingRank, startingFile, move.Rank, move.File);

                Vector2I block;
                blockedDict.TryGetValue(line, out block);

                Vector2I blockDistance = MoveHelpers.GetDistanceFromStart(startingRank, startingFile, block.X, block.Y);

                Vector2I absStartDistance = distanceFromStartingTile.Abs();
                Vector2I absBlockDistance = blockDistance.Abs();

                BoardPos blockPos = new BoardPos(startingRank + absBlockDistance.X, startingFile + absStartDistance.Y);

                if (piece.PieceId == ChessPieceId.Pawn)
                {
                    if (ChessConstants.DiagonalDirections.Contains(line))
                    {
                        // Pawn Diagonal Attack Check
                        if (pieceAtTarget.PieceId != ChessPieceId.Empty && pieceAtTarget.Color != piece.Color)
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
                if (absStartDistance < absBlockDistance || piece.PieceId == ChessPieceId.Knight)
                {
                    // we'll deal with castling later
                    if (piece.PieceId == ChessPieceId.King && MoveHelpers.IsCastleMove(piecePos, move))
                    {
                        continue;
                    }
                    else
                    {
                        capableMoves.Add(move);
                    }

                }
                else if (absStartDistance > absBlockDistance)
                {
                    // Add block
                    AddPosToWatchList(piecePos, blockPos);
                    continue;
                }

                // Castling Check
                if (piece.PieceId == ChessPieceId.King && MoveHelpers.IsCastleMove(piecePos, move))
                {
                    if (piece.Color == ChessColor.White && _gameInfoService.WhiteAnyCastlePossible())
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

                    if (piece.Color == ChessColor.Black && _gameInfoService.BlackAnyCastlePossible())
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
                if (pieceAtTarget.Color != piece.Color && move.Rank == block.X && move.File == block.Y && piece.PieceId != ChessPieceId.Pawn)
                {
                    capableMoves.Add(move);
                }
            }

            foreach (var move in capableMoves)
            {
                AddPosToWatchList(piecePos, move);
            }

            // We don't have to check for watches here since we're already checking for capable moves
            if (capableMoves.Count > 0 || _watches[piecePos.Rank, piecePos.File].Count() > 0) 
            {
                _piecesInPlay.Add(piecePos);
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

        private void FullMoveCalculation(int rank, int file)
        {
            var pos = new BoardPos(rank, file);
            var pieceInfo = BoardDataHandler.GetPieceInfoAtPos(_logicalBoard, pos);

            if (pieceInfo.PieceId != ChessPieceId.Empty)
            {
                _possibleMoves[rank, file] = CalculateMovesAtPos(pos, pieceInfo);
            }
        }

        private void FullMoveCalcs()
        {
            for (int rank = 0; rank < 8; rank++) 
            {
                for (int file = 0; file < 8; file++)
                {
                    FullMoveCalculation(rank, file);
                }
            }
        }

        private void SmartMoveCalcs()
        {
            // Copy the List so that we can iterate _piecesInPlay even if it changes during this function
            BoardPos[] piecesInPlayCopy = new BoardPos[_piecesInPlay.Count];
            _piecesInPlay.CopyTo(piecesInPlayCopy);

            foreach (var piecePos in piecesInPlayCopy)
            {
                var pieceInfo = BoardDataHandler.GetPieceInfoAtPos(_logicalBoard, piecePos);
                if (pieceInfo.PieceId == ChessPieceId.Empty)
                {
                    _piecesInPlay.Remove(piecePos);
                }

                foreach (var posOfInterest in _watches[piecePos.Rank, piecePos.File])
                {
                    // Match the current piece info with the info saved last move
                    // if the piece is different or not there any more, recalculate moves for this piece
                    var currentPieceInfo = BoardDataHandler.GetPieceInfoAtPos(_logicalBoard, posOfInterest.pos);
                    if (currentPieceInfo != posOfInterest.prevBoardInfo)
                    {
                        FullMoveCalculation(piecePos.Rank, piecePos.File);
                        break;
                    }
                }
            }
        }

        private void AddPosToWatchList(BoardPos piecePos, BoardPos watchPos)
        {
            var watches = _watches[piecePos.Rank, piecePos.File];
            if (watches.Find((x) => x.pos == watchPos) is null)
            {
                _watches[piecePos.Rank, piecePos.File].Add(new BlockInfo { pos = watchPos, prevBoardInfo = BoardDataHandler.GetPieceInfoAtPos(_logicalBoard, watchPos) });
            }
        }

        public void UpdateBoard(PieceInfo[,] board)
        {
            var diffs = BoardDataHandler.GetBoardDiffs(_logicalBoard, board);
            _logicalBoard = board;

            foreach (var diff in diffs)
            {
                FullMoveCalculation(diff.Rank, diff.File);
            }

            SmartMoveCalcs();
        }

        public List<BoardPos>[,] GetCalculatedMoves()
        {
            return _possibleMoves;
        }
    }
}
