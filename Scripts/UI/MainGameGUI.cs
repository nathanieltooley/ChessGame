using ChessGame.Scripts;
using Godot;
using System;

public partial class MainGameGUI : Control
{
	// White Color = f1dfc4
	// Black Color = 3c3934

	private Label _enemyTimer;
	private Label _playerTimer;

	private Label[] timerMap = new Label[2];

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		var timerContainer = GetNode<VBoxContainer>("MarginContainer/TimeContainer");

		_enemyTimer = timerContainer.GetNode<Label>("EnemyTime");
		_playerTimer = timerContainer.GetNode<Label>("PlayerTime");

		timerMap[0] = _enemyTimer;
		timerMap[1] = _playerTimer;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnSetTimerColor(ChessSide side, Color color)
	{
		Label timerLabel = timerMap[(int)side];
		var bg = new StyleBoxFlat();

		bg.BgColor = color;
		timerLabel.AddThemeStyleboxOverride("normal", bg);
	}
}
