using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Scripts.Helpers
{
    public static class GridMathHelpers
    {
        public static Vector2I ConvertBoardCoordsToWorld(Vector2I boardCoords, Vector2I tileSize)
        {
            return (boardCoords * tileSize) + (tileSize / 2);
        }

        public static Vector2I ConvertWorldCoordsToBoard(Vector2 worldCoords, Vector2I tileSize, Vector2I _gridMarginOffset)
        {
            int boardX = (int)Math.Floor(worldCoords.X / tileSize.X);
            int boardY = (int)Math.Floor(worldCoords.Y / tileSize.Y);
            return new Vector2I(boardX, boardY) - _gridMarginOffset;
        }
    }
}
