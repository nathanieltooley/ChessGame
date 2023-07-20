using ChessGame.Scripts;
using Godot;
using System;

public partial class ChessBoard : TileMap
{
	private int _gridMargin = 3;

	private Node2D _pieceNode;
	private PackedScene _basePieceScene;

    [Export]
	private PieceColor playerColor = PieceColor.White;

	private PieceColor aiColor = PieceColor.Black;

	private ChessPiece[,] _chessPieceGrid = new ChessPiece[8, 8];

	// White = 0
	// Black = 1
    private int[,] _chessColorGrid = new int[8, 8] {
        { 0, 1, 0, 1, 0, 1, 0, 1 },
        { 1, 0, 1, 0, 1, 0, 1, 0 },
        { 0, 1, 0, 1, 0, 1, 0, 1 },
        { 1, 0, 1, 0, 1, 0, 1, 0 },
        { 0, 1, 0, 1, 0, 1, 0, 1 },
        { 1, 0, 1, 0, 1, 0, 1, 0 },
        { 0, 1, 0, 1, 0, 1, 0, 1 },
        { 1, 0, 1, 0, 1, 0, 1, 0 }
    };

	private void ChessBoardInit()
	{
		aiColor = InvertColor(playerColor);

		PlacePlayerPieces();
		PlaceAiPieces();
	}

	private void BuildPieces()
	{
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				if (_chessPieceGrid[i, j] != null)
				{
					var newPieceNode = _basePieceScene.Instantiate();

				}
			}
		}
	}

	private void PlacePlayerPieces()
	{
		// Pawn Placement
		for (int i = 0; i < 8; i++)
		{
			_chessPieceGrid[6, i] = new ChessPiece(playerColor, ChessPieceId.Pawn);
		}

		// Rook Placement
		_chessPieceGrid[7, 0] = new ChessPiece(playerColor, ChessPieceId.Rook);
		_chessPieceGrid[7, 7] = new ChessPiece(playerColor, ChessPieceId.Rook);

		// Knight Placement
		_chessPieceGrid[7, 1] = new ChessPiece(playerColor, ChessPieceId.Knight);
		_chessPieceGrid[7, 6] = new ChessPiece(playerColor, ChessPieceId.Knight);

		// Bishop Placement
		_chessPieceGrid[7, 2] = new ChessPiece(playerColor, ChessPieceId.Bishop);
		_chessPieceGrid[7, 5] = new ChessPiece(playerColor, ChessPieceId.Bishop);

		// Queen Placement
		_chessPieceGrid[7, 3] = new ChessPiece(playerColor, ChessPieceId.Queen);

		// King Placement
		_chessPieceGrid[7, 4] = new ChessPiece(playerColor, ChessPieceId.King);
	}

	private void PlaceAiPieces()
	{
        // Pawn Placement
        for (int i = 0; i < 8; i++)
        {
            _chessPieceGrid[1, i] = new ChessPiece(playerColor, ChessPieceId.Pawn);
        }

        // Rook Placement
        _chessPieceGrid[0, 0] = new ChessPiece(playerColor, ChessPieceId.Rook);
        _chessPieceGrid[0, 7] = new ChessPiece(playerColor, ChessPieceId.Rook);

        // Knight Placement
        _chessPieceGrid[0, 1] = new ChessPiece(playerColor, ChessPieceId.Knight);
        _chessPieceGrid[0, 6] = new ChessPiece(playerColor, ChessPieceId.Knight);

        // Bishop Placement
        _chessPieceGrid[0, 2] = new ChessPiece(playerColor, ChessPieceId.Bishop);
        _chessPieceGrid[0, 5] = new ChessPiece(playerColor, ChessPieceId.Bishop);

        // Queen Placement
        _chessPieceGrid[0, 4] = new ChessPiece(playerColor, ChessPieceId.Queen);

        // King Placement
        _chessPieceGrid[0, 3] = new ChessPiece(playerColor, ChessPieceId.King);
    }

	private PieceColor InvertColor(PieceColor color)
	{
		if (color == PieceColor.White)
		{
			return PieceColor.Black;
		} else
		{
			return PieceColor.White;
		}
	}

	private Godot.Collections.Array<Node> GetPieceNodes()
	{
		return _pieceNode.GetChildren();
	}

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_pieceNode = GetNode<Node2D>("Pieces");
		_basePieceScene = ResourceLoader.Load<PackedScene>("res://TemplateScenes/base_chess_piece.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
