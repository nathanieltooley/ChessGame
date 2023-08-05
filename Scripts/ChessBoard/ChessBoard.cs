using ChessGame.Scripts;
using ChessGame.Scripts.ChessBoard.Controllers;
using ChessGame.Scripts.DataTypes;
using ChessGame.Scripts.Helpers;
using Godot;

public partial class ChessBoard : TileMap
{
	private Vector2I _gridMargin = new Vector2I(3, 3);

	private BoardController _boardController;
	private GameState _gameState;
	private PlayerMovementController _playerMovementController;

    [Export]
	private ChessColor playerColor = ChessColor.White;
    private ChessColor aiColor = ChessColor.Black;

    [Signal]
    public delegate void UpdateMousePosEventHandler(Vector2 mousePos, BoardPos gridPos, PieceInfo piece);
	[Signal]
	public delegate void UpdateBoardStateEventHandler(string fenString);

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
		_gameState = GetNode<GameState>("/root/GameState");
        _boardController = new BoardController(playerColor, this);
		_playerMovementController = new PlayerMovementController(playerColor, _boardController, ToggleHighlightCell, ClearLayer, EmitInputUpdateSignal);

        aiColor = InvertColor(playerColor);

        switch (_gameState.CurrentGameState)
		{
			case ChessState.NormalGame:
                _boardController.CreateDefaultBoard();
                break;
			case ChessState.Test:
				_boardController.Test();
				break;
		}
		
    }

    public override void _Process(double delta)
    {
        if (_playerMovementController.IsDragging)
        {
            _playerMovementController.PieceBeingDragged.Position = GetViewport().GetMousePosition();
        }
    }

    public override void _Input(InputEvent @event)
    {
        _playerMovementController.InputHandler(@event);
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

	private void EmitInputUpdateSignal(Vector2 mousePos, BoardPos pos, PieceInfo pInfo)
	{
		EmitSignal(SignalName.UpdateMousePos, mousePos, pos, pInfo);
	}
}
