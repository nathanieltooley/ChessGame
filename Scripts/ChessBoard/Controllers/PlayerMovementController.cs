using ChessGame.Scripts.DataTypes;
using ChessGame.Scripts.Helpers;
using Godot;
using System;
using System.Linq;

namespace ChessGame.Scripts.ChessBoard.Controllers
{
    public class PlayerMovementController
    {

        public VisualChessPiece PieceBeingDragged { get; set; }
        public bool IsDragging { get; set; }

        private PieceInfo _pieceBeingDraggedInfo;
        private BoardPos _originalDraggedPieceLoc;

        private ChessColor _playerColor;
        private BoardController _boardController;

        private Action<Vector2I> _ToggleHighlight;
        private Action<int> _ClearLayer;
        private Action<Vector2, BoardPos, PieceInfo> _EmitInputUpdateSignal;

        public PlayerMovementController(ChessColor playerColor, 
            BoardController boardController, 
            Action<Vector2I> highlightFunc, 
            Action<int> clearLayerFunc, 
            Action<Vector2, BoardPos, PieceInfo> emitSignal)
        {
            _playerColor = playerColor;
            _boardController = boardController;

            _ToggleHighlight = highlightFunc;
            _ClearLayer = clearLayerFunc;
            _EmitInputUpdateSignal = emitSignal;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        

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
                                _ToggleHighlight(gridPos);
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
                                        _ToggleHighlight(GridMathHelpers.ConvertBoardCoordToGridChord(move, ChessConstants.BoardMargin));
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
                            _ClearLayer(1);

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

                _EmitInputUpdateSignal(mmEvent.Position, boardPos, _boardController.GetPieceInfoAtPos(boardPos));
            }
        }
    }
}
