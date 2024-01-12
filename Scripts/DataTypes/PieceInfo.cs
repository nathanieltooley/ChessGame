using Godot;
using System.Collections.Generic;

namespace ChessGame.Scripts.DataTypes 
{
    public partial class PieceInfo : GodotObject
    {
        public ChessPieceId PieceId { get; set; }
        public ChessColor Color { get; set; }
        public List<BoardPos> CachedMoves { get; set; } = new List<BoardPos>();

        public static PieceInfo GetEmptyPiece()
        {
            return new PieceInfo { Color = ChessColor.White, PieceId = ChessPieceId.Empty };
        }

        public static bool IsEmptyPiece(PieceInfo pieceInfo)
        {
            return pieceInfo.PieceId == ChessPieceId.Empty; 
        }
        public static bool operator ==(PieceInfo left, PieceInfo right)
        {
            return (left.PieceId == right.PieceId && left.Color == right.Color);
        }

        public static bool operator !=(PieceInfo left, PieceInfo right)
        {
            return !(left == right);
        }
    }
}
