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
            int worldX = boardCoords.Y;
            int worldY = boardCoords.X;

            Vector2I worldCoords = new Vector2I(worldX, worldY);

            return (worldCoords * tileSize) + (tileSize / 2);
        }

        public static Vector2I ConvertWorldCoordsToGridCoords(Vector2 worldCoords, Vector2I tileSize)
        {
            int boardX = (int)Math.Floor(worldCoords.X / tileSize.X);
            int boardY = (int)Math.Floor(worldCoords.Y / tileSize.Y);

            return new Vector2I(boardX, boardY);
        }

        public static Vector2I ConvertWorldCoordsToBoardChords(Vector2 worldCoords, Vector2I tileSize, Vector2I _gridMarginOffset)
        {
            var gridCoords = ConvertWorldCoordsToGridCoords(worldCoords, tileSize);

            int rank;
            int file;

            gridCoords.Deconstruct(out file, out rank);

            return new Vector2I(rank, file) - _gridMarginOffset;
        }

        public static Vector2I ConvertBoardCoordToGridChord(Vector2I rankAndFile, Vector2I gridMargin)
        {
            int x;
            int y;

            rankAndFile.Deconstruct(out y, out x);

            var gridCoords = new Vector2I(x, y);

            return gridCoords + gridMargin;
        }
    }
}
