using ChessGame.Scripts;
using ChessGame.Scripts.Factories;
using Godot;
using System;

public partial class TurnService : Node
{
	private short _turnCount = 0;
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
        return _colorArray[_turnCount % 2];
    }

    public ChessSide GetCurrentTurnSide()
    {
        return _sideArray[_turnCount % 2];
    }

    public void SwitchTurn()
    {
        var startingTurnSide = GetCurrentTurnSide();
        _turnCount++;

        TimerService _timerService = ServiceFactory.GetTimerService();

        _timerService.EmitTimerToggleDisableSignal(startingTurnSide);
        _timerService.EmitTimerToggleDisableSignal(GetCurrentTurnSide());
    }
}
