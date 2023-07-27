﻿using ChessGame.Scripts.DataTypes;
using ChessGame.Scripts.Helpers;
using Godot;
using System.Collections.Generic;

namespace ChessGame.Scripts.ChessBoard.Boards
{
    public partial class GraphicalBoard : GodotObject
    {
        private VisualChessPiece[,] _visBoard = new VisualChessPiece[8,8];

        private Node2D _rootPieceNode;
        private PackedScene _pieceTemplate;

        public GraphicalBoard(Node2D rootPieceNode)
        {
            _rootPieceNode = rootPieceNode;
            _pieceTemplate = ResourceLoader.Load<PackedScene>("res://TemplateScenes/base_chess_piece.tscn");
        }

        public void AddPiece(BoardPos piecePos, PieceInfo info)
        {
            var newPiece = _pieceTemplate.Instantiate<VisualChessPiece>();
            _rootPieceNode.AddChild(newPiece);
            newPiece.UpdateSprite(info);

            var tileSize = ChessConstants.TileSize;
            var gridMargin = ChessConstants.BoardMargin;

            newPiece.Position = GridMathHelpers.ConvertBoardCoordsToWorld(piecePos, tileSize, gridMargin);

            _visBoard[piecePos.Rank, piecePos.File] = newPiece;
        }

        public void RemovePiece(BoardPos piecePos)
        {
            VisualChessPiece pieceAtPos = _visBoard[piecePos.Rank, piecePos.File];

            if (pieceAtPos != null)
            {
                _visBoard[piecePos.Rank, piecePos.File] = null;
                pieceAtPos.QueueFree();
            }
            
        }

        public void MovePiece(BoardPos startingPos, PieceInfo movingPieceInfo, BoardPos newPos)
        {
            RemovePiece(newPos);
            AddPiece(newPos, movingPieceInfo);
            RemovePiece(startingPos);
        }

        public void ClearBoard()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    RemovePiece(new BoardPos(i, j));
                }
            }
        }

        public VisualChessPiece GetPiece(BoardPos startingPos)
        {
            return _visBoard[startingPos.Rank, startingPos.File];
        }

        public static Vector2 CalculateTileCenter(BoardPos boardPos)
        {
            Vector2 gridPosition = new Vector2(boardPos.File, boardPos.Rank) + ChessConstants.BoardMargin;
            Vector2 worldPosition = gridPosition * ChessConstants.TileSize;
            Vector2 center = worldPosition + (ChessConstants.TileSize / 2);

            return center;
        }
    }
}
