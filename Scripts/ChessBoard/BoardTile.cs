using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Scripts.ChessBoard
{
    public class BoardTile
    {
        public ChessColor PieceColor { get; set; }
        public ChessColor TileColor { get; set; }
        public ChessPieceId PieceId { get; set; }
        public VisualChessPiece VisualPiece { get; set; }
        public Vector2 TileCenter { get; set; }

        private List<VisualChessPiece> _pointerToMainList;

        public BoardTile()
        {

        }

        public BoardTile(ChessColor pieceColor, ChessPieceId pieceId, VisualChessPiece visualPiece, int rank, int file, List<VisualChessPiece> pointerToMainList)
        {
            ChessColor tileColor = ChessConstants.GetColor(rank, file);

            PieceColor = pieceColor;
            TileColor = tileColor;
            PieceId = pieceId;
            VisualPiece = visualPiece;

            CalculateTileCenter(rank, file);
            _pointerToMainList = pointerToMainList;
        }

        public static BoardTile BuildEmptyTile(int rank, int file, List<VisualChessPiece> pointerToMainList = null)
        {
            var tile = new BoardTile();
            ChessColor tileColor = ChessConstants.GetColor(rank, file);

            tile.PieceColor = ChessColor.White;
            tile.TileColor = tileColor;
            tile.PieceId = ChessPieceId.Empty;
            tile.AddPointerToVPList(pointerToMainList);
            tile.CalculateTileCenter(rank, file);

            return tile;
        }

        public void CalculateTileCenter(int rank, int file)
        {
            Vector2 gridPosition = new Vector2(file, rank) + ChessConstants.BoardMargin;
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

        // Just Build Tile
        public void BuildTile(PackedScene templatePiece, Node2D rootPieceNode)
        {
            if (PieceId != ChessPieceId.Empty)
            {
                BuildVisualPiece(templatePiece, rootPieceNode);
            }
        }

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
                VisualPiece.QueueFree();
                VisualPiece = null;
            }
        }
    }
}
