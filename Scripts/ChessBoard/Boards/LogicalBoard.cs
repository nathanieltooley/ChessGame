using ChessGame.Scripts.ChessBoard.Controllers;
using ChessGame.Scripts.DataTypes;
using Godot;
using System.Collections.Generic;

namespace ChessGame.Scripts.ChessBoard
{
    public partial class LogicalBoard : GodotObject
    {
        private PieceInfo[,] _board = new PieceInfo[8,8];

        public LogicalBoard() 
        {

            // Init Board
            for (int rank =  0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    _board[rank, file] = new PieceInfo { Color = ChessColor.White, PieceId = ChessPieceId.Empty };
                }
            }
        }

        public PieceInfo GetPieceInfoAtPos(BoardPos boardPos)
        {
            return _board[boardPos.Rank, boardPos.File];
        }

        public void AddPiece(BoardPos boardPos, PieceInfo piece)
        {
            _board[boardPos.Rank, boardPos.File] = piece;
        }

        public void RemovePiece(BoardPos boardPos)
        {
            _board[boardPos.Rank, boardPos.File] = PieceInfo.GetEmptyPiece();
        }

        public void MovePiece(BoardPos startingPos, BoardPos targetPos, bool isPlayerMove, out bool success)
        {
            PieceInfo startingPieceInfo = GetPieceInfoAtPos(startingPos);
            PieceInfo targetPieceInfo = GetPieceInfoAtPos(targetPos);

            List<BoardPos> moves = GetMovesForPiece(startingPos, isPlayerMove);

            if (MoveFinder.IsMovePossible(targetPos, moves))
            {
                AddPiece(targetPos, startingPieceInfo);
                RemovePiece(startingPos);

                startingPieceInfo.CachedMoves.Clear();

                success = true;
            } else
            {
                success = false;
            }
        }

        

        public void ClearBoard()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    _board[i, j] = PieceInfo.GetEmptyPiece();
                }
            }
        }

        public List<BoardPos> GetMovesForPiece(BoardPos pos, bool isPlayerMove)
        {
            PieceInfo piece = GetPieceInfoAtPos(pos);

            if (piece.CachedMoves == null || piece.CachedMoves.Count == 0)
            {


                MoveFinder mf = new MoveFinder(this, piece, pos, isPlayerMove, isPlayerMove ? ChessColor.White : ChessColor.Black);
                List<BoardPos> moves = mf.GetCapableMoves(pos.Rank, pos.File, mf.GetMovesAssumingEmptyBoard());
                return moves;
            } else
            {
                return piece.CachedMoves;
            }
        }

        public PieceInfo[,] GetBoard()
        {
            return _board;
        }

        private void CacheMoves(BoardPos boardPos, List<BoardPos> moves)
        {
            PieceInfo pieceAtPos = GetPieceInfoAtPos(boardPos);

            if (pieceAtPos != null)
            {
                pieceAtPos.CachedMoves = moves;
            }
        }
    }
}
