using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Scripts.DataTypes
{
    public class ChessConstants
    {
        // White = 0
        // Black = 1
        public static readonly int[,] ColorBoard = new int[8, 8] {
            { 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0 },
            { 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0 },
            { 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0 },
            { 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0 }
        };

        public static Vector2I BoardMargin = new Vector2I(3, 3);
        public static Vector2I TileSize = new Vector2I(64, 64);

        public static Vector2I NorthWestDiagonal = new Vector2I(-1, -1);
        public static Vector2I NorthEastDiagonal = new Vector2I(-1, 1);
        public static Vector2I SouthWestDiagonal = new Vector2I(1, -1);
        public static Vector2I SouthEastDiagonal = new Vector2I(1, 1);

        public static Vector2I UpRank = new Vector2I(-1, 0);
        public static Vector2I DownRank = new Vector2I(1, 0);
        public static Vector2I LeftFile = new Vector2I(0, -1);
        public static Vector2I RightFile = new Vector2I(0, 1);

        public static ChessColor GetColor(int rank, int file)
        {
            return (ChessColor)ColorBoard[rank, file];
        }
    }
}
