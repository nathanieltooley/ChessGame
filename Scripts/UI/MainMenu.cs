using Godot;
using System;

public partial class MainMenu : Control
{
	private GameState _gameState;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_gameState = GetNode<GameState>("/root/GameState");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void PlayButtonPressed()
	{
		_gameState.CurrentGameState = ChessGame.Scripts.ChessState.NormalGame;
		GetTree().ChangeSceneToFile("res://chess_board.tscn");
	}

	public void TestButtonPressed()
	{
		_gameState.CurrentGameState = ChessGame.Scripts.ChessState.Test;
        GetTree().ChangeSceneToFile("res://chess_board.tscn");
    }
}
