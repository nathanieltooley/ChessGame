using ChessGame.Scripts;
using Godot;
using System;

public partial class VisualChessPiece : Node2D
{
	[Export]
	PieceColor Color { get; set; }

	[Export]
	ChessPieceId Piece { get; set; }

	private string[] _colorPrefixes = { "w", "b" };
	private string[] _pieceSpriteNames = { "", "P", "K", "B", "R", "N", "Q" };

	private string _pathToAssetFolder = "res://Assets/ChessPieces/";

	private Sprite2D _pieceSprite;

	private string ConstructPieceImageName()
	{
		return _colorPrefixes[(int)Color] + _pieceSpriteNames[(int)Piece];
	}

	private string GetFullImagePath()
	{
		return $"{_pathToAssetFolder}/{ConstructPieceImageName()}";
	}

	private void SetSpriteImage(string imagePath)
	{
		_pieceSprite.Texture = ResourceLoader.Load<Texture2D>(imagePath);
	}

	public void ChangePieceType(ChessPieceId piece)
	{
		Piece = piece;
		SetSpriteImage(GetFullImagePath());
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_pieceSprite = GetNode<Sprite2D>("PieceSprite");
		SetSpriteImage(GetFullImagePath());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
