using ChessGame.Scripts.Boards;
using ChessGame.Scripts.DataTypes;
using ChessGame.Scripts.Helpers;
using Godot;
using System;
using System.Collections.Generic;

namespace ChessGame.Scripts.Controllers
{
    public class EndgameHandler
    {
        // Pieces that can block a king from check this turn
        public List<BoardPos> BlockingPieces { get; set; }

        private PieceInfo[,] _board;
        private List<BoardPos>[,] _moveCache;

        public EndgameHandler(PieceInfo[,] board, List<BoardPos>[,] moveCache)
        {
            _board = board;
            _moveCache = moveCache;
        }

        public bool CheckCheck(BoardPos kingPos, ChessColor attackerColor)
        {
            return MoveHelpers.IsTileUnderAttack(_board, kingPos, attackerColor, _moveCache);
        }

        public bool CheckMateCheck(BoardPos kingPos, ChessColor attackerColor, List<BoardPos> kingsMoves)
        {
            List<BoardPos> attackerPositions = new List<BoardPos>();
            // Check to see if king's position is under attack
            if (!MoveHelpers.IsTileUnderAttack(_board, kingPos, attackerColor, _moveCache))
            {
                return false;
            }

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    PieceInfo tileInfo = BoardDataHandler.GetPieceInfoAtPos(_board, new BoardPos(rank, file));

                    if (tileInfo.PieceId == ChessPieceId.Empty)
                    {
                        continue;
                    }

                    if (_moveCache[rank, file].FindAll(x => x.Rank == kingPos.Rank && x.File == kingPos.File).Count > 0)
                    {
                        attackerPositions.Add(new BoardPos(rank, file));
                    }
                }
            }

            // Check all of the king's moves to see if those are under attack as well
            foreach (var move in kingsMoves)
            {
                if (!MoveHelpers.IsTileUnderAttack(_board, move, attackerColor, _moveCache))
                {
                    return false;
                }
            }

            // Check if any of the tiles between the attacker and the king are able to be blocked by friendly pieces

            foreach (var attackerPos in attackerPositions)
            {
                PieceInfo attacker = BoardDataHandler.GetPieceInfoAtPos(_board, attackerPos);

                // Skip knight since he can jump over pieces
                if (attacker.PieceId == ChessPieceId.Knight)
                {
                    continue;
                }

                Vector2I line = MoveHelpers.GetLineMoveIsOn(attackerPos.Rank, attackerPos.File, kingPos.Rank, kingPos.File);

                foreach (BoardPos pos in MoveHelpers.GetSpacesOnLine(attackerPos, line, _board))
                {
                    if (MoveHelpers.IsTileUnderAttack(_board, pos, MiscHelpers.InvertColor(attackerColor), _moveCache))
                    {
                        return false;
                    }
                }
            }



            return true;
        }
    }
}
