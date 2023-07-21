using ChessGame.Scripts;
using Godot;
using System;
using System.Diagnostics;

public partial class ChessBoard : TileMap
{
	private Vector2I _gridMargin = new Vector2I(3, 3);

	private Node2D _piecesNode;
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

		BuildPieces();
	}

	private void InitPieceGrid()
	{
		for (int rank = 0; rank < 8; rank++)
		{
			for (int file = 0; file < 8; file++)
			{
				_chessPieceGrid[rank, file] = new ChessPiece(PieceColor.White, ChessPieceId.Empty);
			}
		}
	}

	private void BuildPieces()
	{
		for (int rank = 0; rank < 8; rank++)
		{
			for (int file = 0; file < 8; file++)
			{
				Vector2I boardCoords = new Vector2I(file, rank);
				ChessPiece pieceAtCoords = _chessPieceGrid[rank, file];

				if (pieceAtCoords != null && pieceAtCoords.Id != ChessPieceId.Empty)
				{
					var newPieceNode = (VisualChessPiece)_basePieceScene.Instantiate();
					_piecesNode.AddChild(newPieceNode);

					newPieceNode.UpdatePieceInfoAndSprite(pieceAtCoords.Id, pieceAtCoords.Color);
					newPieceNode.Position = ConvertBoardCoordsToWorld(boardCoords);

					Debugger.Log(2, "inf", $"Placing {Enum.GetName(newPieceNode.Color)} {Enum.GetName(newPieceNode.Piece)} at position {newPieceNode.Position}\n");
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
            _chessPieceGrid[1, i] = new ChessPiece(aiColor, ChessPieceId.Pawn);
        }

        // Rook Placement
        _chessPieceGrid[0, 0] = new ChessPiece(aiColor, ChessPieceId.Rook);
        _chessPieceGrid[0, 7] = new ChessPiece(aiColor, ChessPieceId.Rook);

        // Knight Placement
        _chessPieceGrid[0, 1] = new ChessPiece(aiColor, ChessPieceId.Knight);
        _chessPieceGrid[0, 6] = new ChessPiece(aiColor, ChessPieceId.Knight);

        // Bishop Placement
        _chessPieceGrid[0, 2] = new ChessPiece(aiColor, ChessPieceId.Bishop);
        _chessPieceGrid[0, 5] = new ChessPiece(aiColor, ChessPieceId.Bishop);

        // Queen Placement
        _chessPieceGrid[0, 4] = new ChessPiece(aiColor, ChessPieceId.Queen);

        // King Placement
        _chessPieceGrid[0, 3] = new ChessPiece(aiColor, ChessPieceId.King);
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
		return _piecesNode.GetChildren();
	}

	private Vector2I ConvertBoardCoordsToWorld(Vector2I boardCoords)
	{
		Vector2I realGridCoords = _gridMargin + boardCoords;

		return (realGridCoords * TileSet.TileSize) + (TileSet.TileSize / 2);
	}

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_piecesNode = GetNode<Node2D>("Pieces");
		_basePieceScene = ResourceLoader.Load<PackedScene>("res://TemplateScenes/base_chess_piece.tscn");

		ChessBoardInit();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
