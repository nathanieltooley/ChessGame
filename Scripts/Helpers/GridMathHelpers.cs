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
        public static Vector2I ConvertBoardCoordsToWorld(BoardPos boardCoords, Vector2I tileSize, Vector2I _gridMargin)
        {
            int boardX = boardCoords.File;
            int boardY = boardCoords.Rank;

            Vector2I gridCoord = new Vector2I(boardX, boardY) + _gridMargin;


            return (gridCoord * tileSize) + (tileSize / 2);
        }

        public static Vector2I ConvertWorldCoordsToGridCoords(Vector2 worldCoords, Vector2I tileSize)
        {
            int boardX = (int)Math.Floor(worldCoords.X / tileSize.X);
            int boardY = (int)Math.Floor(worldCoords.Y / tileSize.Y);

            return new Vector2I(boardX, boardY);
        }

        public static BoardPos ConvertWorldCoordsToBoardChords(Vector2 worldCoords, Vector2I tileSize, Vector2I _gridMarginOffset)
        {
            var gridCoords = ConvertWorldCoordsToGridCoords(worldCoords, tileSize);

            int rank = gridCoords.Y - _gridMarginOffset.Y;
            int file = gridCoords.X - _gridMarginOffset.X;

            return new BoardPos(rank, file);
        }

        public static Vector2I ConvertBoardCoordToGridChord(BoardPos boardPos, Vector2I gridMargin)
        {
            return new Vector2I(boardPos.File + gridMargin.X, boardPos.Rank  + gridMargin.Y);
        }
    }
}
