using ChessGame.Scripts.Boards;
using ChessGame.Scripts.ChessBoard;
using ChessGame.Scripts.DataTypes;
using ChessGame.Scripts.Factories;
using System;
using System.Collections.Generic;
namespace ChessGame.Scripts.Controllers
{
    public class MoveController
    {
        private PieceInfo[,] _board;

        public List<BoardPos>[,] MoveCache { get; set; }

        public MoveController(PieceInfo[,] logicalBoard)
        {
            _board = logicalBoard;
        }

        public List<BoardPos> GetMovesAtPos(BoardPos pos)
        {
            return MoveCache[pos.Rank, pos.File];
        }

        public void UpdateMoveCache()
        {
            List<BoardPos>[,] boardPosMatrix = new List<BoardPos>[8, 8];

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    BoardPos piecePos = new BoardPos(rank, file);
                    PieceInfo pieceInfo = BoardDataHandler.GetPieceInfoAtPos(_board, piecePos);

                    if (pieceInfo.PieceId != ChessPieceId.Empty)
                    {
                        boardPosMatrix[rank, file] = MoveFinder.GetCapableMoves(piecePos, pieceInfo, _board, ServiceFactory.GetGameInfoService());
                    }
                }
            }

            MoveCache = boardPosMatrix;
        }

        public bool IsTileUnderAttack(BoardPos boardPos, ChessColor attackerColor)
        {
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    var pos = new BoardPos(rank, file);

                    PieceInfo piece = BoardDataHandler.GetPieceInfoAtPos(_board, pos);
                    if (piece.PieceId == ChessPieceId.Empty || piece.Color != attackerColor)
                    {
                        continue;
                    }
                    else
                    {
                        var moves = GetMovesAtPos(pos);
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

        public bool CheckCheck(BoardPos kingPos, ChessColor kingColor)
        {
            return IsTileUnderAttack(kingPos, kingColor);
        }

        public bool CheckMateCheck(BoardPos kingPos, ChessColor attackerColor, List<BoardPos> moves)
        {
            if (!IsTileUnderAttack(kingPos, attackerColor))
            {
                return false;
            }

            foreach (var move in moves)
            {
                if (!IsTileUnderAttack(move, attackerColor))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
