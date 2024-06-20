using ChessGame.Scripts;
using ChessGame.Scripts.Helpers;
using Godot;
using System;

public partial class MainMenu : Control
{
	[Export]
	private Button playGameButton;

	[Export]
	private ColorPanel panel;

	[Export]
	private CheckButton colorCheckBox;

	private RunInfo _gameState;
	private ChessColor _playerColor = ChessColor.White;

	private bool colorPanelOpen = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_gameState = GetNode<RunInfo>("/root/RunInfo");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void PlayButtonPressed()
	{
		if (!colorPanelOpen)
		{
			playGameButton.Text = "press again to start";
			panel.Activate();
			colorPanelOpen = true;
		} else
		{
            _gameState.GameRunEnvironment = RunEnvironment.Normal;
			_gameState.PlayerColor = colorCheckBox.ButtonPressed ? ChessColor.Black : ChessColor.White;
            GetTree().ChangeSceneToFile("res://chess_board.tscn");
		}

	}

	public void TestButtonPressed()
	{
		_gameState.GameRunEnvironment = RunEnvironment.Test;
        GetTree().ChangeSceneToFile("res://chess_board.tscn");
    }

	public void ColorCheckToggled()
	{
		_playerColor = MiscHelpers.InvertColor(_playerColor);
	}
}
