using ChessGame.Scripts;
using ChessGame.Scripts.Helpers;
using Godot;
using System;

public partial class TurnService : Node
{
	private short _turnIndex = 0;
	private ChessColor[] _colorArray = new ChessColor[] { ChessColor.White, ChessColor.Black };
	private ChessSide[] _sideArray = new ChessSide[2];

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void Setup(ChessSide whitePlayer, ChessSide blackPlayer)
    {
        _sideArray[0] = whitePlayer;
        _sideArray[1] = blackPlayer;
    }

    public ChessColor GetCurrentTurnColor() 
    {
        return _colorArray[_turnIndex];
    }

    public ChessSide GetCurrentTurnSide()
    {
        return _sideArray[_turnIndex];
    }

    public void SwitchTurn()
    {
        var startingTurnSide = GetCurrentTurnSide();
        _turnIndex++;

        if (_turnIndex > 1)
        {
            _turnIndex = 0;
        }

        TimerService _timerService = ServiceHelpers.GetTimerService();

        _timerService.EmitTimerToggleDisableSignal(startingTurnSide);
        _timerService.EmitTimerToggleDisableSignal(GetCurrentTurnSide());
    }
}
