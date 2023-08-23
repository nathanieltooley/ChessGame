using Godot;
using System;

namespace ChessGame.Scripts
{
    public partial class BoardPos : GodotObject
    {
        public int Rank { get; }
        public int File { get; }

        private int _max = 7;
        private int _min = 0;

        public BoardPos(int rank, int file)
        {
            Rank = Math.Clamp(rank, _min, _max);
            File = Math.Clamp(file, _min, _max);
        }

        public static BoardPos operator +(BoardPos a, int b) => new BoardPos(a.Rank + b, a.File + b);
        public static BoardPos operator -(BoardPos a, int b) => a + (-b);
        public static BoardPos operator *(BoardPos a, int b) => new BoardPos(a.Rank * b, a.File * b);
        public static BoardPos operator /(BoardPos a, int b) => new BoardPos(a.Rank / b, a.File / b);

        public static BoardPos operator -(BoardPos a) => new BoardPos(-a.Rank, -a.File);

        public static BoardPos operator +(BoardPos a, BoardPos b) => new BoardPos(a.Rank + b.Rank, a.File + b.File);
        public static BoardPos operator -(BoardPos a, BoardPos b) => a + (-b);
        public static BoardPos operator +(BoardPos a, Vector2I b) => new BoardPos(a.Rank + b.X, a.File + b.Y);
        public static BoardPos operator -(BoardPos a, Vector2I b) => a + (-b);

        public static bool operator ==(BoardPos a, BoardPos b) 
        {
            if (a is null && b is null)
            {
                return true;
            } else if (a is null && !(b is null))
            {
                return false;
            } else if (!(a is null) && b is null)
            {
                return false;
            } else
            {
                return a.Rank == b.Rank && a.File == b.File;
            }
        } 
        public static bool operator !=(BoardPos a, BoardPos b) { return !(a == b); }

        public override string ToString()
        {
            return $"Rank: {Rank}, File: {File}";
        }
    }
}
