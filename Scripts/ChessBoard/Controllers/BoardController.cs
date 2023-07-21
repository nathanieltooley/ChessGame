using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using ChessGame.Scripts.Helpers;

namespace ChessGame.Scripts.ChessBoard.Controllers
{
    public partial class BoardController : Node2D
    {
        private ChessPiece[,] _pieceBoard;
        private int[,] _colorBoard;

        private PackedScene _basePieceScene;
        private Node2D _chessBoardNode;

        private int _boardMargin = 3;
        private Vector2I _boardMarginVector; 
        private Vector2I _tileSize;

        public BoardController(Vector2I tileSize, Node2D chessBoard)
        {
            // Init Piece Board
            _pieceBoard = new ChessPiece[8, 8];

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    _pieceBoard[rank, file] = new ChessPiece(PieceColor.White, ChessPieceId.Empty);
                }
            }

            // White = 0
            // Black = 1
            _colorBoard = new int[8, 8] {
                { 0, 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1, 0 },
                { 0, 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1, 0 },
                { 0, 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1, 0 },
                { 0, 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1, 0 }
            };

            _basePieceScene = ResourceLoader.Load<PackedScene>("res://TemplateScenes/base_chess_piece.tscn");
            _tileSize = tileSize;
            _boardMarginVector = new Vector2I(_boardMargin, _boardMargin);
            _chessBoardNode = chessBoard;
        }

        public void AddPiece(int rank, int file, ChessPiece piece)
        {
            _pieceBoard[rank, file] = piece;
        }

        public void RemovePiece(int rank, int file) 
        {
            _pieceBoard[rank, file] = null;
        }

        public ChessPiece GetPiece(int rank, int file)
        {
            return _pieceBoard[rank, file];
        }

        public void BuildPiece(int rank, int file, ref List<VisualChessPiece> pieceNodeList)
        {
            Vector2I boardCoords = new Vector2I(file, rank);
            ChessPiece pieceAtCoords = GetPiece(rank, file);

            if (pieceAtCoords != null && pieceAtCoords.Id != ChessPieceId.Empty)
            {
                var newPieceNode = (VisualChessPiece)_basePieceScene.Instantiate();
                pieceNodeList.Add(newPieceNode);
                _chessBoardNode.AddChild(newPieceNode);

                newPieceNode.UpdatePieceInfoAndSprite(pieceAtCoords.Id, pieceAtCoords.Color);
                newPieceNode.UpdateGridPosition(rank, file, _boardMarginVector, _tileSize);

                Debugger.Log(2, "inf", $"Placing {Enum.GetName(newPieceNode.Color)} {Enum.GetName(newPieceNode.Piece)} at position {newPieceNode.Position}\n");
            }
        }

        public void MovePiece(int oldRank, int oldFile, int newRank, int newFile)
        {
            ChessPiece piece = GetPiece(oldRank, oldFile);

            AddPiece(newRank, newFile, piece);
            RemovePiece(oldRank, oldFile);
        }

        public PieceColor GetColorOfTile(int rank, int file)
        {
            return (PieceColor)_colorBoard[rank, file];
        }
    }
}
