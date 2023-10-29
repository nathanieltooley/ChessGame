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

            BlockingPieces = new List<BoardPos>();
        }

        public bool CheckCheck(BoardPos kingPos, ChessColor attackerColor)
        {
            return MoveHelpers.IsTileUnderAttack(_board, kingPos, attackerColor, _moveCache);
        }

        public bool CheckMateCheck(BoardPos kingPos, ChessColor attackerColor, List<BoardPos> kingsMoves)
        {
            // Check to see if king's position is under attack
            if (!MoveHelpers.IsTileUnderAttack(_board, kingPos, attackerColor, _moveCache))
            {
                return false;
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
            bool canBeBlocked = false;

            var attackers = BoardSearching.GetAllAttackerPositions(_board, kingPos, _moveCache);

            foreach (var attacker in attackers)
            {
                if (BoardSearching.GetAllBlockingPiecePositions(_board, _moveCache, kingPos, attacker).Count != 0)
                {
                    canBeBlocked = true;
                }
            }
            

            return !canBeBlocked;
        }
    }
}
