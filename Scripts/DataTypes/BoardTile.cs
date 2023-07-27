using ChessGame.Scripts.DataTypes;
using Godot;
using System.Collections.Generic;

namespace ChessGame.Scripts.ChessBoard
{
    public partial class BoardTile : GodotObject
    {
        public ChessColor PieceColor { get; set; }
        public ChessPieceId PieceId { get; set; }
        public VisualChessPiece VisualPiece { get; set; }
        public Vector2 TileCenter { get; set; }
        public BoardPos TilePos { get; set; }

        private List<VisualChessPiece> _pointerToMainList;

        public BoardTile()
        {

        }

        public BoardTile(ChessColor pieceColor, ChessPieceId pieceId, VisualChessPiece visualPiece, BoardPos boardPos, List<VisualChessPiece> pointerToMainList)
        {
            PieceColor = pieceColor;
            PieceId = pieceId;
            VisualPiece = visualPiece;
            TilePos = boardPos;

            CalculateTileCenter();
            _pointerToMainList = pointerToMainList;
        }

        // List may be null because BoardTiles can be created outside of the LogicalBoard class (good idea?). This is
        // risky however, as errors can happen when assuming the list has already been set
        public static BoardTile BuildEmptyTile(BoardPos pos, List<VisualChessPiece> pointerToMainList = null)
        {
            var tile = new BoardTile();
            
            tile.TilePos = pos;
            tile.PieceColor = ChessColor.White;
            tile.PieceId = ChessPieceId.Empty;
            tile.AddPointerToVPList(pointerToMainList);
            tile.CalculateTileCenter();

            return tile;
        }

        public void CalculateTileCenter()
        {
            Vector2 gridPosition = new Vector2(TilePos.File, TilePos.Rank) + ChessConstants.BoardMargin;
            Vector2 worldPosition = gridPosition * ChessConstants.TileSize;
            Vector2 center = worldPosition + (ChessConstants.TileSize / 2);

            TileCenter = center;
        }

        public void ClearTile()
        {
            PieceColor = ChessColor.White;
            PieceId = ChessPieceId.Empty;

            DeleteVisualPiece();
        }

        // Update and Build Tile
        public void UpdateTile(ChessColor newColor, ChessPieceId newPiece, PackedScene templatePiece, Node2D rootPieceNode)
        {
            PieceColor = newColor;
            PieceId = newPiece;

            BuildTile(templatePiece, rootPieceNode);
        }

        /// <summary>
        ///     Builds this tile by creating a VisualChessPiece based on the stored Color and Piece Id and sets its position to the center of this tile.
        ///     Does not create any piece if this tile is empty
        /// </summary>
        /// <param name="templatePiece">
        ///     The Godot scene that is a template for the chess piece. Should be of class VisualChessPiece and should have its own Sprite2D Node as a child.
        /// </param>
        /// <param name="rootPieceNode">
        ///     The Node2D that is the parent of the newly build VisualChessPiece. Used as a way to add the new node to the SceneTree
        /// </param>
        public void BuildTile(PackedScene templatePiece, Node2D rootPieceNode)
        {
            if (PieceId != ChessPieceId.Empty)
            {
                BuildVisualPiece(templatePiece, rootPieceNode);
            }
        }

        /// <summary>
        ///     Adds a list, usually stored in LogicalBoard, that contains all of the VisualChessPieces created and added on the board.
        ///     This function may be needed because the list was not orignally passed in the constructor
        /// </summary>
        /// <param name="list"></param>
        public void AddPointerToVPList(List<VisualChessPiece> list)
        {
            _pointerToMainList = list;
        }

        public bool IsListNull()
        {
            return _pointerToMainList == null;
        }

        private void BuildVisualPiece(PackedScene templatePiece, Node2D rootPieceNode)
        {
            VisualPiece = templatePiece.Instantiate<VisualChessPiece>();
            rootPieceNode.AddChild(VisualPiece);
            _pointerToMainList.Add(VisualPiece);

            VisualPiece.UpdatePieceInfoAndSprite(PieceId, PieceColor);
            VisualPiece.Position = TileCenter;
        }

        private void DeleteVisualPiece()
        {
            if (VisualPiece != null)
            {
                _pointerToMainList.Remove(VisualPiece);

                // Making sure we don't have memory leaks
                VisualPiece.QueueFree();
                VisualPiece = null;
            }
        }
    }
}
