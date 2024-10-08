using ChessGame.Scripts;
using ChessGame.Scripts.DataTypes;
using ChessGame.Scripts.Factories;
using Godot;
using Godot.Collections;
using System;

public partial class TimerService : Node
{
    public bool TimersPaused { get; set; } = false;

    [Export]
    public float StartingTime = 300;

    // Timer Signals
    [Signal]
    public delegate void UpdateSideTimeEventHandler(ChessSide side, double time);
    [Signal]
    public delegate void SetTimerColorEventHandler(ChessSide side, Color color);
    [Signal]
    public delegate void ToggleTimerEventHandler(ChessSide side);
    [Signal]
    public delegate void GameOverOOTEventHandler(ChessSide side);

    private float _playerTimer;
    private float _enemyTimer;

    private Dictionary<ChessSide, float> _timerMap = new Dictionary<ChessSide, float>();

    private TurnService _turnService;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _turnService = ServiceFactory.GetTurnService();

        _playerTimer = StartingTime;
        _enemyTimer = StartingTime;

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
            UpdateTimer(-delta);
        } 
    }

    public void Setup(ChessSide nonStartingSide) 
    {
        EmitTimerToggleDisableSignal(nonStartingSide);
    }

    private void UpdateTimer(double delta)
    {
        var currentTurn = _turnService.GetCurrentTurnSide();

        float _timer;
        _timerMap.TryGetValue(currentTurn, out _timer);

        float newTime = Godot.Mathf.Max(_timer + (float)delta, 0);
        _timerMap[currentTurn] = newTime;

        EmitTimerUpdateTimeSignal(_turnService.GetCurrentTurnSide(), newTime);

        if (newTime <= 0)
        {
            TimersPaused = true;
            EmitSignal(SignalName.GameOverOOT, (int)_turnService.GetCurrentTurnSide());
        }
    }

    public void EmitTimerColorUpdateSignal(ChessSide side, ChessColor color)
    {
        Color hexColor = (color == ChessColor.White) ? ChessConstants.WhiteColor : ChessConstants.BlackColor;

        EmitSignal(SignalName.SetTimerColor, (int)side, hexColor);
    }

    public void EmitTimerToggleDisableSignal(ChessSide side)
    {
        EmitSignal(SignalName.ToggleTimer, (int)side);
    }

    public void EmitTimerUpdateTimeSignal(ChessSide side, double newTime)
    {
        EmitSignal(SignalName.UpdateSideTime, (int)side, newTime);
    }
}
