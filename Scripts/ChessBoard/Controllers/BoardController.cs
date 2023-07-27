﻿using ChessGame.Scripts.ChessBoard.Boards;
using ChessGame.Scripts.DataTypes;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace ChessGame.Scripts.ChessBoard.Controllers
{
    public class BoardController
    {
        private GraphicalBoard _gBoard { get; set; }
        private LogicalBoard _logicBoard { get; set; }
        private PlayerMovementController PlayerMovement { get; set; }

        private List<PieceInfo> _whitePieces = new List<PieceInfo>();
        private List<PieceInfo> _blackPieces = new List<PieceInfo>();

        private ChessColor _playerColor;
        private static string _startingFenString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

        public BoardController(ChessColor playerColor, Node2D rootPieceNode)
        {
            _playerColor = playerColor;

            _logicBoard = new LogicalBoard();
            _gBoard = new GraphicalBoard(rootPieceNode);
        }

        public void CreateDefaultBoard()
        {
            CreateBoard(_startingFenString);
        }

        public void CreateBoard(string fenString)
        {
            PieceInfo[,] fenBoard = GetBoardFromFEN(fenString);
            UpdateBoard(fenBoard);
        }

        public void AddPiece(BoardPos pos, PieceInfo piece)
        {
            _gBoard.AddPiece(pos, piece);
            _logicBoard.AddPiece(pos, piece);
        }

        public void RemovePiece(BoardPos pos)
        {
            _gBoard.RemovePiece(pos);
            _logicBoard.RemovePiece(pos);
        }

        public bool MovePiece(BoardPos pos, BoardPos targetPos)
        {
            bool success;
            var movingPieceInfo = _logicBoard.GetPieceInfoAtPos(pos);

            _logicBoard.MovePiece(pos, targetPos, movingPieceInfo.Color == _playerColor, out success);

            if (success)
            {
                _gBoard.MovePiece(pos, movingPieceInfo, targetPos);
            }

            return success;
        }

        public PieceInfo GetPieceInfoAtPos(BoardPos pos)
        {
            return _logicBoard.GetPieceInfoAtPos(pos);
        }

        public List<VisualChessPiece> GetVisualPieces()
        {
            return _gBoard.VisualPieceMap.Values.ToList();
        }

        public List<BoardPos> GetMovesForPiece(BoardPos pos, bool isPlayerMove) 
        {
            var pieceInfo = _logicBoard.GetPieceInfoAtPos(pos);
            return _logicBoard.GetMovesForPiece(pos, isPlayerMove);
        }

        public Vector2 GetTileCenter(BoardPos pos)
        {
            return GraphicalBoard.CalculateTileCenter(pos);
        }

        private PieceInfo[,] GetBoardFromFEN(string fenString)
        {
            PieceInfo[,] newBoard = FEN.Decrypt(fenString, _whitePieces, _blackPieces);
            return newBoard;
        }

        private void UpdateBoard(PieceInfo[,] board)
        {
            _gBoard.ClearBoard();
            _logicBoard.ClearBoard();

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    AddPiece(new BoardPos(rank, file), board[rank, file]);
                }
            }
        }
    }
}
