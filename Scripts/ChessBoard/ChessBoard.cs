using ChessGame.Scripts;
using ChessGame.Scripts.ChessBoard.Controllers;
using ChessGame.Scripts.Helpers;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class ChessBoard : TileMap
{
	private Vector2I _gridMargin = new Vector2I(3, 3);

	private BoardController _boardController;
	private VisualChessPiece _pieceBeingDragged;
	private bool _isDragging;
	private int _tileSize;
	private Vector2I _tileSizeVector;

	private List<VisualChessPiece> _pieceList = new List<VisualChessPiece>();

    [Export]
	private PieceColor playerColor = PieceColor.White;

	private PieceColor aiColor = PieceColor.Black;

	private void BuildPieces()
	{
		for (int rank = 0; rank < 8; rank++)
		{
			for (int file = 0; file < 8; file++)
			{
				_boardController.BuildPiece(rank, file, ref _pieceList);
			}
		}
	}

	private void PlacePlayerPieces()
	{
		// Pawn Placement
		for (int i = 0; i < 8; i++)
		{
			_boardController.AddPiece(6, i, new ChessPiece(playerColor, ChessPieceId.Pawn));
		}

		// Rook Placement
		_boardController.AddPiece(7, 0, new ChessPiece(playerColor, ChessPieceId.Rook));
        _boardController.AddPiece(7, 7, new ChessPiece(playerColor, ChessPieceId.Rook));

        // Knight Placement
        _boardController.AddPiece(7, 1, new ChessPiece(playerColor, ChessPieceId.Knight));
		_boardController.AddPiece(7, 6, new ChessPiece(playerColor, ChessPieceId.Knight));

		// Bishop Placement
		_boardController.AddPiece(7, 2, new ChessPiece(playerColor, ChessPieceId.Bishop));
		_boardController.AddPiece(7, 5, new ChessPiece(playerColor, ChessPieceId.Bishop));

		// Queen Placement
		_boardController.AddPiece(7, 3, new ChessPiece(playerColor, ChessPieceId.Queen));

		// King Placement
		_boardController.AddPiece(7, 4, new ChessPiece(playerColor, ChessPieceId.King));
	}

	private void PlaceAiPieces()
	{
        // Pawn Placement
        for (int i = 0; i < 8; i++)
        {
            _boardController.AddPiece(1, i, new ChessPiece(aiColor, ChessPieceId.Pawn));
        }

        // Rook Placement
        _boardController.AddPiece(0, 0, new ChessPiece(aiColor, ChessPieceId.Rook));
        _boardController.AddPiece(0, 7, new ChessPiece(aiColor, ChessPieceId.Rook));

        // Knight Placement
        _boardController.AddPiece(0, 1, new ChessPiece(aiColor, ChessPieceId.Knight));
        _boardController.AddPiece(0, 6, new ChessPiece(aiColor, ChessPieceId.Knight));

        // Bishop Placement
        _boardController.AddPiece(0, 2, new ChessPiece(aiColor, ChessPieceId.Bishop));
        _boardController.AddPiece(0, 5, new ChessPiece(aiColor, ChessPieceId.Bishop));

        // Queen Placement
        _boardController.AddPiece(0, 4, new ChessPiece(aiColor, ChessPieceId.Queen));

        // King Placement
        _boardController.AddPiece(0, 3, new ChessPiece(aiColor, ChessPieceId.King));
    }

	private PieceColor InvertColor(PieceColor color)
	{
		if (color == PieceColor.White)
		{
			return PieceColor.Black;
		} else
		{
			return PieceColor.White;
		}
	}

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_tileSize = TileSet.TileSize.X;
		_tileSizeVector = TileSet.TileSize;
		_pieceDragging = new PieceDragging(_tileSize);
		_boardController = new BoardController(TileSet.TileSize, this);

        aiColor = InvertColor(playerColor);

        PlacePlayerPieces();
        PlaceAiPieces();

        BuildPieces();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_isDragging)
		{
			_pieceBeingDragged.Position = GetViewport().GetMousePosition();
		}
	}

    public override void _Input(InputEvent @event)
    {
		if (@event is InputEventMouseButton)
		{
			InputEventMouseButton mbEvent = (InputEventMouseButton) @event;

			switch (mbEvent.ButtonIndex)
			{
				case MouseButton.Left:
					if (mbEvent.IsPressed())
					{
                        var mousePos = mbEvent.Position;

                        foreach (var piece in _pieceList)
                        {
                            if (mousePos.DistanceTo(piece.Position) <= _tileSize / 2)
                            {
                                _isDragging = true;
                                _pieceBeingDragged = piece;
                            }
                        }
                    } else if (mbEvent.IsReleased()) 
					{
						_isDragging = false;

						Vector2I boardPos = GridMathHelpers.ConvertWorldCoordsToBoard(GetViewport().GetMousePosition(), _tileSizeVector, _gridMargin);

						int newRank = boardPos.Y;
						int newFile = boardPos.X;

						int oldRank = _pieceBeingDragged.Rank;
						int oldFile = _pieceBeingDragged.File;

                        _pieceBeingDragged.UpdateGridPosition(newRank, newFile, _gridMargin, _tileSizeVector);
						_boardController.MovePiece(oldRank, oldFile, newRank, newFile);

						_pieceBeingDragged = null;
					}

                    
                    break;
			}
        }
    }
}
