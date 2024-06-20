using ChessGame.Scripts;
using Godot;
using System;

public partial class RunInfo : Node
{
    public RunEnvironment GameRunEnvironment { get; set; }
    public ChessColor PlayerColor { get; set; }
}
