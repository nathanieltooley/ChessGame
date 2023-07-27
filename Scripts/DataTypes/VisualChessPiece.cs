using ChessGame.Scripts;
using ChessGame.Scripts.DataTypes;
using Godot;

public partial class VisualChessPiece : Node2D
{

    private static string[] _colorPrefixes = { "w", "b" };
	private static string[] _pieceSpriteNames = { "", "P", "K", "B", "R", "N", "Q" };

	private static string _fileExt = "svg";

	private static string _pathToAssetFolder = "res://Assets/ChessPieces/";

	private Sprite2D _pieceSprite;

    private string ConstructPieceTextureName(ChessColor color, ChessPieceId piece)
	{
		return $"{_colorPrefixes[(int)color]}{_pieceSpriteNames[(int)piece]}.{_fileExt}";
	}

	private string GetFullTexturePath(ChessColor color, ChessPieceId piece)
	{
		return $"{_pathToAssetFolder}/{ConstructPieceTextureName(color, piece)}";
	}

	private Texture2D GetPieceTexture(ChessColor color, ChessPieceId piece)
	{
		string texturePath = GetFullTexturePath(color, piece);
		return ResourceLoader.Load<Texture2D>(texturePath);
	}

	private void SetTexture(Texture2D texture)
	{
		_pieceSprite.Texture = texture;
	}

	public void UpdateSprite(PieceInfo info)
	{
        SetTexture(GetPieceTexture(info.Color, info.PieceId));
    }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_pieceSprite = GetNode<Sprite2D>("PieceSprite");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
