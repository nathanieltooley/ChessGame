using ChessGame.Scripts;
using ChessGame.Scripts.ChessBoard;
using ChessGame.Scripts.ChessBoard.Controllers;
using ChessGame.Scripts.DataTypes;
using ChessGame.Scripts.Helpers;
using Godot;

public partial class ChessBoard : TileMap
{
	private Vector2I _gridMargin = new Vector2I(3, 3);

	private VisualChessPiece _pieceBeingDragged;
	private PieceInfo _pieceBeingDraggedInfo;
	private BoardPos _originalDraggedPieceLoc;
	private PackedScene _templatePiece;
	private bool _isDragging;

	private BoardController _boardController;

    [Export]
	private ChessColor playerColor = ChessColor.White;
    private ChessColor aiColor = ChessColor.Black;

    [Signal]
    public delegate void UpdateMousePosEventHandler(Vector2 mousePos, BoardPos gridPos, PieceInfo piece);

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
		_boardController = new BoardController(playerColor, this);

        aiColor = InvertColor(playerColor);

		_boardController.CreateDefaultBoard();
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
						var pieces = _boardController.GetVisualPieces();

						var gridPos = GridMathHelpers.ConvertWorldCoordsToGridCoords(mousePos, ChessConstants.TileSize);
						var boardPos = GridMathHelpers.ConvertWorldCoordsToBoardChords(mousePos, ChessConstants.TileSize, ChessConstants.BoardMargin);


                        if (_boardController.GetPieceInfoAtPos(boardPos).PieceId == ChessPieceId.Empty)
						{
							ToggleHighlightCell(gridPos);
							return;
						}

                        foreach (VisualChessPiece piece in pieces)
                        {
                            if (mousePos.DistanceTo(piece.Position) <= ChessConstants.TileSize.X / 2)
                            {
                                _isDragging = true;
                                _pieceBeingDragged = piece;
								_pieceBeingDraggedInfo = _boardController.GetPieceInfoAtPos(boardPos);

								_originalDraggedPieceLoc = boardPos;

								var moves = _boardController.GetMovesForPiece(boardPos, _pieceBeingDraggedInfo.Color == playerColor);
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

						var mp = GetViewport().GetMousePosition();

						BoardPos boardPos = GridMathHelpers.ConvertWorldCoordsToBoardChords(GetViewport().GetMousePosition(), ChessConstants.TileSize, ChessConstants.BoardMargin);

						if (boardPos == _originalDraggedPieceLoc)
						{
							_pieceBeingDragged.Position = GridMathHelpers.ConvertBoardCoordsToWorld(_originalDraggedPieceLoc, ChessConstants.TileSize, ChessConstants.BoardMargin);
						} else
						{
							bool success = _boardController.MovePiece(_originalDraggedPieceLoc, boardPos);
                            if (success)
							{
								_pieceBeingDragged.Position = _boardController.GetTileCenter(boardPos);
                            } else
							{
								_pieceBeingDragged.Position = _boardController.GetTileCenter(_originalDraggedPieceLoc);
							}
                            
                        }

						// remove highlights
						ClearLayer(1);

                        _pieceBeingDragged = null;
						_originalDraggedPieceLoc = null;
                    }

                    
                    break;
			}
        }

		if (@event is InputEventMouseMotion)
		{
			InputEventMouseMotion mmEvent = (InputEventMouseMotion) @event;
			BoardPos boardPos = GridMathHelpers.ConvertWorldCoordsToBoardChords(mmEvent.Position, ChessConstants.TileSize, ChessConstants.BoardMargin);


            EmitSignal(SignalName.UpdateMousePos, mmEvent.Position, boardPos, _boardController.GetPieceInfoAtPos(boardPos));
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
