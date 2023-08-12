using ChessGame.Scripts.DataTypes;
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

        public static BoardPos ConvertGridCoordToBoardCoords(Vector2I gridPos, Vector2I gridMargin)
        {
            return new BoardPos(gridPos.Y - gridMargin.Y, gridPos.X - gridMargin.X);
        }

        public static BoardPos InvertBoardPos(BoardPos pos)
        {
            return new BoardPos(7 - pos.Rank, 7 - pos.File);
        }

        public static Vector2I InvertGridPos(Vector2I gridPos)
        {
            BoardPos p = ConvertGridCoordToBoardCoords(gridPos, ChessConstants.BoardMargin);
            BoardPos invP = InvertBoardPos(p);
            return ConvertBoardCoordToGridChord(invP, ChessConstants.BoardMargin);
        }

        public static Vector2 CalculateTileCenter(BoardPos boardPos, bool inverted)
        {
            if (inverted)
            {
                boardPos = InvertBoardPos(boardPos);
            }

            Vector2 gridPosition = new Vector2(boardPos.File, boardPos.Rank) + ChessConstants.BoardMargin;
            Vector2 worldPosition = gridPosition * ChessConstants.TileSize;
            Vector2 center = worldPosition + (ChessConstants.TileSize / 2);

            return center;
        }
    }
}
