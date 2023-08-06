using ChessGame.Scripts;
using ChessGame.Scripts.DataTypes;
using Godot;
using System.Collections.Generic;

public partial class GameInfoService : Node
{
    public RunEnvironment GameRunEnv { get; set; }
    public ChessColor PlayerSideColor { get; set; }
    public ChessColor EnemySideColor { get; set; }

    public List<PieceInfo> WhitePieces { get; set; }
    public List<PieceInfo> BlackPieces { get; set; }
    public List<PieceInfo> CapturedWhitePieces { get; set; }
    public List<PieceInfo> CapturedBlackPieces { get; set; }

    [Signal]
    public delegate void UpdateBoardStateEventHandler(string fenString);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void SetupGameInfo(RunEnvironment runEnvironment, ChessColor playerColor, ChessColor aiColor)
    {
        GameRunEnv = runEnvironment;
        PlayerSideColor = playerColor;
        EnemySideColor = aiColor;
    }

    public void SetupPieceInfo(List<PieceInfo> whitePieces, List<PieceInfo> blackPieces)
    {
        WhitePieces = whitePieces;
        BlackPieces = blackPieces;

        CapturedWhitePieces = new List<PieceInfo>();
        CapturedBlackPieces = new List<PieceInfo>();
    }
    public void EmitFenStringSignal(string fenString)
    {
        EmitSignal(SignalName.UpdateBoardState, fenString);
    }
}

public enum RunEnvironment
{
	Normal = 0,
	Test = 1,
}
