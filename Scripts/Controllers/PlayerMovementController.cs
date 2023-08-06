using ChessGame.Scripts.ChessBoard.Boards;
using ChessGame.Scripts.DataTypes;
using ChessGame.Scripts.Helpers;
using Godot;
using System;
using System.Linq;

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
        private GraphicalBoard _graphicalBoard;

        [Signal]
        public delegate void UpdateMousePosEventHandler(Vector2 mousePos, BoardPos gridPos, PieceInfo piece);

        // Called every frame. 'delta' is the elapsed time since the previous frame.

        public override void _Ready()
        {
            GameInfoService gameInfoService = GetNode<GameInfoService>("/root/Main/GameInfoService");

            _playerColor = gameInfoService.PlayerSideColor;
            _boardController = GetNode<BoardController>("/root/Main/BoardController");
            _graphicalBoard = GetNode<GraphicalBoard>("/root/Main/GraphicalBoard");
        }

        public void InputHandler(InputEvent @event)
        {
            if (@event is InputEventMouseButton)
            {
                InputEventMouseButton mbEvent = (InputEventMouseButton)@event;
                var mousePos = mbEvent.Position;

                switch (mbEvent.ButtonIndex)
                {
                    case MouseButton.Left:
                        if (mbEvent.IsPressed())
                        {

                            var pieces = _boardController.GetVisualPieces();

                            var gridPos = GridMathHelpers.ConvertWorldCoordsToGridCoords(mousePos, ChessConstants.TileSize);
                            var boardPos = GridMathHelpers.ConvertWorldCoordsToBoardChords(mousePos, ChessConstants.TileSize, ChessConstants.BoardMargin);


                            if (_boardController.GetPieceInfoAtPos(boardPos).PieceId == ChessPieceId.Empty)
                            {
                                _graphicalBoard.ToggleHighlightCell(gridPos);
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

                                    var moves = _boardController.GetMovesForPiece(boardPos, _pieceBeingDraggedInfo.Color == _playerColor);
                                    LogHelpers.DebugLog($"{moves.Count}");

                                    foreach (var move in moves)
                                    {
                                        _graphicalBoard.ToggleHighlightCell(GridMathHelpers.ConvertBoardCoordToGridChord(move, ChessConstants.BoardMargin));
                                    }
                                }
                            }
                        }
                        else if (mbEvent.IsReleased())
                        {
                            if (!IsDragging)
                            {
                                return;
                            }

                            IsDragging = false;


                            BoardPos boardPos = GridMathHelpers.ConvertWorldCoordsToBoardChords(mousePos, ChessConstants.TileSize, ChessConstants.BoardMargin);

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
                                }
                                else
                                {
                                    PieceBeingDragged.Position = _boardController.GetTileCenter(_originalDraggedPieceLoc);
                                }

                            }

                            // remove highlights
                            _graphicalBoard.ClearLayer(1);

                            PieceBeingDragged = null;
                            _originalDraggedPieceLoc = null;
                        }


                        break;
                }
            }

            if (@event is InputEventMouseMotion)
            {
                InputEventMouseMotion mmEvent = (InputEventMouseMotion)@event;
                BoardPos boardPos = GridMathHelpers.ConvertWorldCoordsToBoardChords(mmEvent.Position, ChessConstants.TileSize, ChessConstants.BoardMargin);

                EmitSignal(SignalName.UpdateMousePos, mmEvent.Position, boardPos, _boardController.GetPieceInfoAtPos(boardPos));
            }
        }
    }
}
