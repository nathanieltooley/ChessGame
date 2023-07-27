using ChessGame.Scripts.DataTypes;
using ChessGame.Scripts.Helpers;
using Godot;
using System.Collections.Generic;

namespace ChessGame.Scripts.ChessBoard.Boards
{
    public partial class GraphicalBoard : GodotObject
    {
        public Dictionary<BoardPos, VisualChessPiece> VisualPieceMap { get; set; }

        private Node2D _rootPieceNode;
        private PackedScene _pieceTemplate;

        public GraphicalBoard(Node2D rootPieceNode)
        {
            VisualPieceMap = new Dictionary<BoardPos, VisualChessPiece>();
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
            
            VisualPieceMap.Add(piecePos, newPiece);
        }

        public void RemovePiece(BoardPos piecePos)
        {
            VisualChessPiece pieceAtPos;
            VisualPieceMap.TryGetValue(piecePos, out pieceAtPos);

            if (pieceAtPos != null)
            {
                VisualPieceMap.Remove(piecePos);
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
            foreach (var key in VisualPieceMap.Keys)
            {
                RemovePiece(key);
            }
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
