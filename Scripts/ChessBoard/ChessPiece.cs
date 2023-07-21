using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Scripts
{
    public class ChessPiece
    {
        public PieceColor Color { get; set; }
        public ChessPieceId Id { get; set; } 
        public ChessPiece(PieceColor color, ChessPieceId id) {
            Color = color;
            Id = id;
        }
    }
}
