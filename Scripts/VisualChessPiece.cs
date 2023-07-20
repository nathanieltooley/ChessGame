using ChessGame.Scripts;
using Godot;
using System;

public partial class VisualChessPiece : Node2D
{
	public PieceColor Color { get; set; }
	public ChessPieceId Piece { get; set; }

	private string[] _colorPrefixes = { "w", "b" };
	private string[] _pieceSpriteNames = { "", "P", "K", "B", "R", "N", "Q" };

	private string _fileExt = "svg";

	private string _pathToAssetFolder = "res://Assets/ChessPieces/";

	private Sprite2D _pieceSprite;

	private string ConstructPieceTextureName(PieceColor color, ChessPieceId piece)
	{
		return $"{_colorPrefixes[(int)color]}{_pieceSpriteNames[(int)piece]}.{_fileExt}";
	}

	private string GetFullTexturePath(PieceColor color, ChessPieceId piece)
	{
		return $"{_pathToAssetFolder}/{ConstructPieceTextureName(color, piece)}";
	}

	private Texture2D GetPieceTexture(PieceColor color, ChessPieceId piece)
	{
		string texturePath = GetFullTexturePath(color, piece);
		return ResourceLoader.Load<Texture2D>(texturePath);
	}

	private void SetTexture(Texture2D texture)
	{
		_pieceSprite.Texture = texture;
	}

	public void ChangePieceType(ChessPieceId piece)
	{
		Piece = piece;
	}

	public void ChangeColor(PieceColor color)
	{
		Color = color;
	}

	public void UpdatePieceInfoAndSprite(ChessPieceId newId, PieceColor newColor)
	{
		ChangePieceType(newId);
		ChangeColor(newColor);

        SetTexture(GetPieceTexture(Color, Piece));
    }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_pieceSprite = GetNode<Sprite2D>("PieceSprite");
		ChangePieceType(ChessPieceId.Pawn);
		
		SetTexture(GetPieceTexture(Color, Piece));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
