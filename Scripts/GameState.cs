using ChessGame.Scripts;
using Godot;
using System;

public partial class GameState : Node
{
    public ChessState CurrentGameState { get; set; }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        CurrentGameState = ChessState.NormalGame;
	}
}
