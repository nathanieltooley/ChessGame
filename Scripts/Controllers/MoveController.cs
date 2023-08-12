using ChessGame.Scripts.ChessBoard;
using ChessGame.Scripts.DataTypes;
using System;
using System.Collections.Generic;
namespace ChessGame.Scripts.Controllers
{
    public class MoveController
    {
        private ILogicalBoard _logicBoard;

        public List<BoardPos>[,] MoveCache { get; set; }

        public MoveController(ILogicalBoard logicalBoard)
        {
            _logicBoard = logicalBoard;
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
                    PieceInfo pieceInfo = _logicBoard.GetPieceInfoAtPos(piecePos);

                    if (pieceInfo.PieceId != ChessPieceId.Empty)
                    {
                        MoveFinder mf = new MoveFinder(_logicBoard, pieceInfo, piecePos);
                        boardPosMatrix[rank, file] = mf.GetCapableMoves(piecePos);
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

                    PieceInfo piece = _logicBoard.GetPieceInfoAtPos(pos);
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
    }
}
