using ChessGame.Scripts;
using ChessGame.Scripts.ChessBoard.Controllers;
using ChessGame.Scripts.DataTypes;
using ChessGame.Scripts.Helpers;
using Godot;
using System.Collections.Generic;

public partial class ChessBoard : TileMap
{
	private ChessColor _currentTurn = ChessColor.White;
	private ChessSide _currentTurnSide;

	private BoardController _boardController;
	private GameState _gameState;
	private PlayerMovementController _playerMovementController;

    [Export]
	private ChessColor playerColor = ChessColor.White;
    private ChessColor aiColor = ChessColor.Black;

	private Dictionary<ChessSide, ChessColor> sideColors;

	private Color _whiteColor = Color.FromHtml("f1dfc4");
	private Color _blackColor = Color.FromHtml("3c3934");

    [Signal]
    public delegate void UpdateMousePosEventHandler(Vector2 mousePos, BoardPos gridPos, PieceInfo piece);
	[Signal]
	public delegate void UpdateBoardStateEventHandler(string fenString);

	// Timer Signals
	[Signal]
	public delegate void UpdateSideTimeEventHandler(ChessSide side, float time);
	[Signal]
	public delegate void SetTimerColorEventHandler(ChessSide side, Color color);
	[Signal]
	public delegate void ToggleTimerEventHandler(ChessSide side);

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
        _boardController = new BoardController(playerColor, this, EmitFenStringSignal, SwitchTurn, GetCurrentTurn);
		_playerMovementController = new PlayerMovementController(playerColor, _boardController, ToggleHighlightCell, ClearLayer, EmitInputUpdateSignal);

        aiColor = InvertColor(playerColor);

		EmitTimerColorUpdateSignal(ChessSide.Enemy, aiColor);
		EmitTimerColorUpdateSignal(ChessSide.Player, playerColor);

		if (playerColor == _currentTurn)
		{
			EmitTimerToggleDisableSignal(ChessSide.Enemy);
			_currentTurnSide = ChessSide.Player;
		} else
		{
			EmitTimerToggleDisableSignal(ChessSide.Player);
			_currentTurnSide = ChessSide.Enemy;
		}

		sideColors = new Dictionary<ChessSide, ChessColor>
		{
			{ ChessSide.Player, playerColor },
			{ ChessSide.Enemy, aiColor },
		};

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

    public void SwitchTurn()
    {
		var startingTurnSide = _currentTurnSide;

		ToggleTurn();
		SwitchTurnSide();

		EmitTimerToggleDisableSignal(startingTurnSide);
		EmitTimerToggleDisableSignal(_currentTurnSide);
    }

	public ChessColor GetCurrentTurn() { return _currentTurn; } 

	private void ToggleTurn()
	{
        if (_currentTurn == ChessColor.White)
        {
            _currentTurn = ChessColor.Black;
        }
        else
        {
            _currentTurn = ChessColor.White;
        }
    }

	private void SwitchTurnSide()
	{
		if (_currentTurnSide == ChessSide.Player)
		{
			_currentTurnSide = ChessSide.Enemy;
		} else
		{
			_currentTurnSide = ChessSide.Player;
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

	private void EmitInputUpdateSignal(Vector2 mousePos, BoardPos pos, PieceInfo pInfo)
	{
		EmitSignal(SignalName.UpdateMousePos, mousePos, pos, pInfo);
	}

	private void EmitFenStringSignal(string fenString)
	{
		EmitSignal(SignalName.UpdateBoardState, fenString);
	}

	private void EmitTimerColorUpdateSignal(ChessSide side, ChessColor color)
	{
		Color hexColor = (color == ChessColor.White) ? _whiteColor : _blackColor;

		EmitSignal(SignalName.SetTimerColor, (int)side, hexColor);
	}

	private void EmitTimerToggleDisableSignal(ChessSide side)
	{
		EmitSignal(SignalName.ToggleTimer, (int)side);
	}
}
