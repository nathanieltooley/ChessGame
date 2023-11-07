using ChessGame.Scripts.ChessBoard.Boards;
using ChessGame.Scripts.DataTypes;
using ChessGame.Scripts.Factories;
using ChessGame.Scripts.Helpers;
using Godot;

namespace ChessGame.Scripts.Controllers
{
    public partial class PlayerMovementController : Node
    {

        public VisualChessPiece PieceBeingDragged { get; set; }
        public bool IsDragging { get; set; }

        private PieceInfo _pieceBeingDraggedInfo;
        private BoardPos _originalDraggedPieceLoc;

        private ChessColor _playerColor;
        private BoardController _boardController;
        private GameInfoService _gameInfoService;

        [Signal]
        public delegate void UpdateMousePosEventHandler(Vector2 mousePos, BoardPos gridPos, PieceInfo piece);
        [Signal]
        public delegate void ToggleCellHighlightEventHandler(Vector2I gridPos);
        [Signal]
        public delegate void ClearHighlightsEventHandler();
        [Signal]
        public delegate void PlaySoundEventHandler(string audioResPath);

        // Called every frame. 'delta' is the elapsed time since the previous frame.

        public override void _Ready()
        {
            _gameInfoService = ServiceFactory.GetGameInfoService();
            _boardController = ControllerFactory.GetBoardController();

            _playerColor = _gameInfoService.PlayerSideColor;
        }

        public void InputHandler(InputEvent @event)
        {
            if (@event is InputEventMouseButton)
            {
                MouseClickHandler((InputEventMouseButton)@event);
            }

            if (@event is InputEventMouseMotion)
            {
                var mEvent = (InputEventMouseMotion)@event;
                MouseMotionHander(mEvent.Position);
            }
        }

        private void MouseClickHandler(InputEventMouseButton @event)
        {
            var buttonPressed = @event.ButtonIndex;
            var isPressed = @event.IsPressed();
            var isReleased = @event.IsReleased();
            var mousePos = @event.Position;

            switch (buttonPressed)
            {
                case MouseButton.Left:
                    if (isPressed)
                    {
                        StartDrag(mousePos);
                    }
                    else if (isReleased)
                    {
                        EndDrag(mousePos);
                    }
                    break;
            }
        }

        private void MouseMotionHander(Vector2 mousePos)
        {
            BoardPos boardPos = GridMathHelpers.ConvertWorldCoordsToBoardChords(mousePos, ChessConstants.TileSize, ChessConstants.BoardMargin);

            if (_gameInfoService.ViewInverted())
            {
                boardPos = GridMathHelpers.InvertBoardPos(boardPos);
            }

            EmitSignal(SignalName.UpdateMousePos, mousePos, boardPos, _boardController.GetPieceInfoAtPos(boardPos));
        }

        private void StartDrag(Vector2 mousePos)
        {
            var pieces = _boardController.GetVisualPieces();

            var gridPos = GridMathHelpers.ConvertWorldCoordsToGridCoords(mousePos, ChessConstants.TileSize);
            var boardPos = GridMathHelpers.ConvertWorldCoordsToBoardChords(mousePos, ChessConstants.TileSize, ChessConstants.BoardMargin);

            if (_gameInfoService.ViewInverted())
            {
                boardPos = GridMathHelpers.InvertBoardPos(boardPos);
            }

            if (_boardController.GetPieceInfoAtPos(boardPos).PieceId == ChessPieceId.Empty)
            {
                EmitSignal(SignalName.ToggleCellHighlight, gridPos);
                return;
            }

            foreach (VisualChessPiece piece in pieces)
            {
                if (mousePos.DistanceTo(piece.Position) <= ChessConstants.TileSize.X / 2)
                {
                    IsDragging = true;
                    PieceBeingDragged = piece;
                    _pieceBeingDraggedInfo = _boardController.GetPieceInfoAtPos(boardPos);

                    _originalDraggedPieceLoc = boardPos;

                    var moves = _boardController.GetMovesForPiece(boardPos);

                    if (moves != null)
                    {
                        foreach (var move in moves)
                        {
                            EmitSignal(SignalName.ToggleCellHighlight, GridMathHelpers.ConvertBoardCoordToGridChord(move, ChessConstants.BoardMargin));
                        }
                    }

                    return;
                }
            }
        }

        private void EndDrag(Vector2 mousePos)
        {
            if (!IsDragging)
            {
                return;
            }

            IsDragging = false;


            BoardPos boardPos = GridMathHelpers.ConvertWorldCoordsToBoardChords(mousePos, ChessConstants.TileSize, ChessConstants.BoardMargin);

            if (_gameInfoService.ViewInverted())
            {
                boardPos = GridMathHelpers.InvertBoardPos(boardPos);
            }

            if (boardPos == _originalDraggedPieceLoc)
            {
                PieceBeingDragged.Position = GridMathHelpers.ConvertBoardCoordsToWorld(_originalDraggedPieceLoc, ChessConstants.TileSize, ChessConstants.BoardMargin);
            }
            else
            {
                bool success = _boardController.MovePiece(_originalDraggedPieceLoc, boardPos);
                if (success)
                {
                    PieceBeingDragged.Position = _boardController.GetTileCenter(boardPos);
                    EmitSignal(SignalName.PlaySound, "res://Assets//Audio//chess_piece_place.mp3");
                }
                else
                {
                    PieceBeingDragged.Position = _boardController.GetTileCenter(_originalDraggedPieceLoc);
                }

            }

            // remove highlights
            EmitSignal(SignalName.ClearHighlights);

            PieceBeingDragged = null;
            _originalDraggedPieceLoc = null;
        }
    }
}
