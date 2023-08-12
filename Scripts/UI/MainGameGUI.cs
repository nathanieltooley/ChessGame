using ChessGame.Scripts;
using Godot;
using System;

public partial class MainGameGUI : Control
{
	// White Color = f1dfc4
	// Black Color = 3c3934

	private Label _enemyTimer;
	private Label _playerTimer;
	private Control _gameOverMenu;
	private Panel _fgPanel;

	private Label[] timerMap = new Label[2];
	private bool[] disabledMap = new bool[2];

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		var timerContainer = GetNode<VBoxContainer>("MarginContainer/TimeContainer");

		_enemyTimer = timerContainer.GetNode<Label>("EnemyTime");
		_playerTimer = timerContainer.GetNode<Label>("PlayerTime");

		_gameOverMenu = GetNode<Control>("GameOverMenu");
		_fgPanel = GetNode<Panel>("FGBlur");

		timerMap[0] = _enemyTimer;
		timerMap[1] = _playerTimer;

		disabledMap[0] = false;
		disabledMap[1] = false;
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

	public void OnToggleTimer(ChessSide side)
	{
		Label timer = timerMap[(int)side];
		bool disabled = disabledMap[(int)side];

        if (!disabled)
        {
			timer.AddThemeColorOverride("font_color", Color.FromHtml("848484"));
			disabledMap[(int)side] = true;
        } else
		{
			timer.AddThemeColorOverride("font_color", Color.FromHtml("FFFFFF"));
			disabledMap[(int)side] = false;
		}
    }

	public void OnUpdateTimerTime(ChessSide side, double newTime) 
	{
		double inMinutes = (double)newTime / 60;
		int minutes = (int)Math.Floor(newTime / 60);
		int seconds = (int)Math.Floor((inMinutes - minutes) * 60);

		if (seconds == 60)
		{
			seconds = 0;
		}

		Label timer = timerMap[(int)side];

		string secondsPrefix = seconds < 10 ? "0" : "";
		timer.Text = $"{minutes}:{secondsPrefix}{seconds}";
	}

	public void ShowGameOverScreen(ChessColor winner)
	{
		_gameOverMenu.Visible = true;
		_fgPanel.Visible = true;
	}
}
