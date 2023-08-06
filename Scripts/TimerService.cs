using ChessGame.Scripts;
using ChessGame.Scripts.DataTypes;
using Godot;
using Godot.Collections;
using System;

public partial class TimerService : Node
{
    public bool TimersPaused { get; set; } = false;

    // Timer Signals
    [Signal]
    public delegate void UpdateSideTimeEventHandler(ChessSide side, double time);
    [Signal]
    public delegate void SetTimerColorEventHandler(ChessSide side, Color color);
    [Signal]
    public delegate void ToggleTimerEventHandler(ChessSide side);

    private float _startingTime = 300;
    private float _playerTimer;
    private float _enemyTimer;

    private Dictionary<ChessSide, float> _timerMap = new Dictionary<ChessSide, float>();

    private TurnService _turnService;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _turnService = GetNode<TurnService>("/root/Main/TurnService");

        _playerTimer = _startingTime;
        _enemyTimer = _startingTime;

        _timerMap = new Dictionary<ChessSide, float>
        {
            { ChessSide.Player, _playerTimer },
            { ChessSide.Enemy, _enemyTimer },
        };
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (!TimersPaused)
        {
            UpdateTimer(delta);
        } 
    }

    public void Setup(ChessSide nonStartingSide) 
    {
        EmitTimerToggleDisableSignal(nonStartingSide);
    }

    private void UpdateTimer(double delta)
    {
        float _timer;
        _timerMap.TryGetValue(_turnService.GetCurrentTurnSide(), out _timer);

        _timer += (float)delta;

        EmitTimerUpdateTimeSignal(_turnService.GetCurrentTurnSide(), _timer);
    }

    public void EmitTimerColorUpdateSignal(ChessSide side, ChessColor color)
    {
        Color hexColor = (color == ChessColor.White) ? ChessConstants.WhiteColor : ChessConstants.BlackColor;

        EmitSignal(SignalName.SetTimerColor, (int)side, hexColor);
    }

    private void EmitTimerToggleDisableSignal(ChessSide side)
    {
        EmitSignal(SignalName.ToggleTimer, (int)side);
    }

    private void EmitTimerUpdateTimeSignal(ChessSide side, double newTime)
    {
        EmitSignal(SignalName.UpdateSideTime, (int)side, newTime);
    }
}
