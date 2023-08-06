using Godot;
using System;

public partial class MainMenu : Control
{
	private RunInfo _gameState;
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
		_gameState.GameRunEnvironment = RunEnvironment.Normal;
		GetTree().ChangeSceneToFile("res://chess_board.tscn");
	}

	public void TestButtonPressed()
	{
		_gameState.GameRunEnvironment = RunEnvironment.Test;
        GetTree().ChangeSceneToFile("res://chess_board.tscn");
    }
}
