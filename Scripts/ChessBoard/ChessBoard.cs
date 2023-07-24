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
		_logicalBoard = new LogicalBoard(this, _templatePiece, _tileSize, playerColor);

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

						var gridPos = GridMathHelpers.ConvertWorldCoordsToGridCoords(mousePos, _tileSizeVector);
						var boardPos = GridMathHelpers.ConvertWorldCoordsToBoardChords(mousePos, _tileSizeVector, _gridMargin);


                        if (_logicalBoard.GetTile(boardPos.X, boardPos.Y).PieceId == ChessPieceId.Empty)
						{
							ToggleHighlightCell(gridPos);
							return;
						}

                        foreach (VisualChessPiece piece in pieces)
                        {
                            if (mousePos.DistanceTo(piece.Position) <= _tileSize / 2)
                            {
                                _isDragging = true;
                                _pieceBeingDragged = piece;

								_originalDraggedPieceLoc = boardPos;

								var moves = _logicalBoard.GetMovesForPiece(boardPos.X, boardPos.Y);
								LogHelpers.DebugLog($"{moves.Count}");

								foreach (var move in moves)
								{
									ToggleHighlightCell(GridMathHelpers.ConvertBoardCoordToGridChord(move, _gridMargin));
								}
                            }
                        }
                    } else if (mbEvent.IsReleased()) 
					{
						if (!_isDragging)
						{
							return;
						}

						_isDragging = false;

						Vector2I boardPos = GridMathHelpers.ConvertWorldCoordsToBoardChords(GetViewport().GetMousePosition(), _tileSizeVector, _gridMargin);

						int newRank = boardPos.X;
						int newFile = boardPos.Y;

						int oldRank = _originalDraggedPieceLoc.X;
						int oldFile = _originalDraggedPieceLoc.Y;

						if (newRank == oldRank && newFile == oldFile)
						{
							_pieceBeingDragged.Position = GridMathHelpers.ConvertBoardCoordsToWorld(_originalDraggedPieceLoc, _tileSizeVector);
						} else
						{
                            _logicalBoard.MovePiece(oldRank, oldFile, newRank, newFile);
                            _pieceBeingDragged.Position = _logicalBoard.GetTile(newRank, newFile).TileCenter;
                        }

                        _pieceBeingDragged = null;
                        _originalDraggedPieceLoc = Vector2I.Zero;
                    }

                    
                    break;
			}
        }
    }

	private bool IsCellHighlighted(Vector2I gridPos)
	{
		var td = GetCellTileData(1, gridPos);

		return td == null ? false : true;
	}

	private void ToggleHighlightCell(Vector2I gridPos)
	{
		if (!IsCellHighlighted(gridPos))
		{
            SetCell(1, gridPos, 0, new Vector2I(0, 0));
        } else
		{
			EraseCell(1, gridPos);
		}
		
    }
}
