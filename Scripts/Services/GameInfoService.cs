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

    public bool CanWKingCastle { get; set; } = true;
    public bool CanWQueenCastle { get; set; } = true;
    public bool CanBKingCastle { get; set; } = true;
    public bool CanBQueenCastle { get; set; } = true;

    public bool WhiteInCheck { get; set; }
    public bool BlackInCheck { get; set; }

    public bool[] WhiteEnpassantArray { get; set; }
    public bool[] BlackEnpassantArray { get; set; }

    [Signal]
    public delegate void UpdateBoardStateEventHandler(string fenString);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        WhiteEnpassantArray = new bool[8];
        BlackEnpassantArray = new bool[8];
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

    public bool IsPlayerColor(ChessColor color)
    {
        return color == PlayerSideColor;
    }

    public void OnCheckUpdate(ChessColor color, bool inCheck) 
    {
        if (color == ChessColor.White)
        {
            WhiteInCheck = inCheck;
        } else
        {
            BlackInCheck = inCheck;
        }
    }

    public bool ColorInCheck(ChessColor color)
    {
        var check = color == ChessColor.White ? WhiteInCheck : BlackInCheck;
        return check;
    }

    public bool ViewInverted()
    {
        return PlayerSideColor == ChessColor.Black;
    }

    public bool WhiteAnyCastlePossible()
    {
        return CanWKingCastle || CanWQueenCastle;
    }

    public bool BlackAnyCastlePossible()
    {
        return CanBKingCastle || CanBQueenCastle;
    }

    public void ToggleEnpassant(ChessColor color, int file)
    {
        if (ChessColor.White == color)
        {
            WhiteEnpassantArray[file] = !WhiteEnpassantArray[file];
        } else
        {
            BlackEnpassantArray[file] = !BlackEnpassantArray[file];
        }
    }

    public bool FileInEnPassantPosition(ChessColor color, int file)
    {
        if (color == ChessColor.White)
        {
            return WhiteEnpassantArray[file];
        } else
        {
            return BlackEnpassantArray[file];
        }
    }
}

public enum RunEnvironment
{
	Normal = 0,
	Test = 1,
}
