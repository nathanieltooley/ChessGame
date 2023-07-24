using ChessGame.Scripts.ChessBoard;
using ChessGame.Scripts.Helpers;
using Godot;
using System;

public partial class DebugMenu : Control
{
	private VBoxContainer _debugContainer;

	private Label _mousePosLabel;
	private Label _mousePosGridLabel;

	private Label _pieceColorLabel;
	private Label _pieceLabel;

	public override void _Ready()
	{
		_debugContainer = GetNode<VBoxContainer>("DebugTextContainer");

		_mousePosLabel = _debugContainer.GetNode<Label>("MousePosLabel");
		_mousePosGridLabel = _debugContainer.GetNode<Label>("MousePosGridLabel");
		_pieceColorLabel = _debugContainer.GetNode<Label>("ChessPieceColorLabel");
		_pieceLabel = _debugContainer.GetNode<Label>("ChessPieceLabel");
	}

	public void OnUpdateMousePos(Vector2 mousePos, Vector2I boardPos, BoardTile tile)
	{
		_mousePosLabel.Text = $"MousePos: {mousePos}";

		if ((boardPos.X < 0 || boardPos.X > 7) || (boardPos.Y < 0 || boardPos.Y > 7)) 
		{
			_mousePosGridLabel.Text = "Rank: OOB, File: OOB";

			_pieceColorLabel.Text = "Piece Color: OOB";
			_pieceLabel.Text = "Piece: OOB";
		} else
		{
            _mousePosGridLabel.Text = $"Rank: {boardPos.X}, File: {boardPos.Y}";

			_pieceColorLabel.Text = $"Piece Color: {tile.PieceColor}";
			_pieceLabel.Text = $"Piece: {tile.PieceId}";

		}
    }
}
