using ChessGame.Scripts;
using ChessGame.Scripts.ChessBoard;
using ChessGame.Scripts.ChessBoard.Controllers;
using ChessGame.Scripts.Helpers;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class ChessBoard : TileMap
{
	private Vector2I _gridMargin = new Vector2I(3, 3);

	private LogicalBoard _logicalBoard;
	private VisualChessPiece _pieceBeingDragged;
	private Vector2I _originalDraggedPieceLoc;
	private PackedScene _templatePiece;
	private bool _isDragging;
	private int _tileSize;
	private Vector2I _tileSizeVector;

    [Export]
	private ChessColor playerColor = ChessColor.White;

	private ChessColor aiColor = ChessColor.Black;

	private ChessColor InvertColor(ChessColor color)
	{
		if (color == ChessColor.White)
		{
			return ChessColor.Black;
		} else
		{
			return ChessColor.White;
		}
	}

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_tileSize = TileSet.TileSize.X;
		_tileSizeVector = TileSet.TileSize;
		_templatePiece = ResourceLoader.Load<PackedScene>("res://TemplateScenes/base_chess_piece.tscn");
		_logicalBoard = new LogicalBoard(this, _templatePiece, _tileSize);

        aiColor = InvertColor(playerColor);

		_logicalBoard.BuildBoardFromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR");
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
						var pieces = _logicalBoard.VisualPiecesList;

                        foreach (VisualChessPiece piece in pieces)
                        {
                            if (mousePos.DistanceTo(piece.Position) <= _tileSize / 2)
                            {
                                _isDragging = true;
                                _pieceBeingDragged = piece;
								_originalDraggedPieceLoc = GridMathHelpers.ConvertWorldCoordsToBoard(mousePos, _tileSizeVector, _gridMargin);
                            }
                        }
                    } else if (mbEvent.IsReleased()) 
					{
						if (!_isDragging)
						{
							return;
						}

						_isDragging = false;

						Vector2I boardPos = GridMathHelpers.ConvertWorldCoordsToBoard(GetViewport().GetMousePosition(), _tileSizeVector, _gridMargin);

						int newRank = boardPos.Y;
						int newFile = boardPos.X;

						int oldRank = _originalDraggedPieceLoc.Y;
						int oldFile = _originalDraggedPieceLoc.X;

						_logicalBoard.MovePiece(oldRank, oldFile, newRank, newFile);

						_pieceBeingDragged = null;
						_originalDraggedPieceLoc = Vector2I.Zero;
					}

                    
                    break;
			}
        }
    }
}
