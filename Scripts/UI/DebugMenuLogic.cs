using ChessGame.Scripts;
using ChessGame.Scripts.DataTypes;
using Godot;

public partial class DebugMenuLogic : Control
{
	private VBoxContainer _debugContainer;

	private Label _mousePosLabel;
	private Label _mousePosGridLabel;

	private Label _pieceColorLabel;
	private Label _pieceLabel;

	private Label _fenLabel;

	private Label _whiteCheckLabel;
	private Label _blackCheckLabel;

	public override void _Ready()
	{
		_debugContainer = GetNode<VBoxContainer>("DebugMenuContainer/DebugTextContainer");

		_mousePosLabel = _debugContainer.GetNode<Label>("MousePosLabel");
		_mousePosGridLabel = _debugContainer.GetNode<Label>("MousePosGridLabel");
		_pieceColorLabel = _debugContainer.GetNode<Label>("ChessPieceColorLabel");
		_pieceLabel = _debugContainer.GetNode<Label>("ChessPieceLabel");
		_whiteCheckLabel = _debugContainer.GetNode<Label>("WhiteCheckLabel");
		_blackCheckLabel = _debugContainer.GetNode<Label>("BlackCheckLabel");

        _fenLabel = GetNode<Label>("FENDebug");
	}

	public void OnChessBoardUpdateMousePos(Vector2 mousePos, BoardPos boardPos, PieceInfo piece)
	{
		_mousePosLabel.Text = $"MousePos: {mousePos}";

		if ((boardPos.Rank < 0 || boardPos.Rank > 7) || (boardPos.File < 0 || boardPos.File > 7)) 
		{
			_mousePosGridLabel.Text = "Rank: OOB, File: OOB";

			_pieceColorLabel.Text = "Piece Color: OOB";
			_pieceLabel.Text = "Piece: OOB";
		} else
		{
            _mousePosGridLabel.Text = $"Rank: {boardPos.Rank}, File: {boardPos.File}";

			_pieceColorLabel.Text = $"Piece Color: {piece.Color}";
			_pieceLabel.Text = $"Piece: {piece.PieceId}";

		}
    }

	public void OnUpdateBoardState(string newFen)
	{
		_fenLabel.Text = $"Current FEN: {newFen}";
	}

	public void OnColorInCheck(ChessColor colorInCheck, bool inCheck)
	{
		if (colorInCheck == ChessColor.White)
		{
			_whiteCheckLabel.Text = $"White in Check?: {inCheck}";
		} else if (colorInCheck == ChessColor.Black)
		{
			_blackCheckLabel.Text = $"Black in Check?: {inCheck}";
		}
	}
}
