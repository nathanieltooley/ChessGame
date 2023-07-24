using ChessGame.Scripts.ChessBoard.Controllers;
using Godot;
using System.Collections.Generic;

namespace ChessGame.Scripts.ChessBoard
{
    public partial class LogicalBoard : Node2D
    {
        private BoardTile[,] _board = new BoardTile[8,8];

        private Node2D _rootPieceNode;
        private PackedScene _pieceTemplate;

        private ChessColor _playerColor;

        public List<VisualChessPiece> VisualPiecesList { get; set; } = new List<VisualChessPiece>();

        public LogicalBoard(Node2D rootNode, PackedScene pieceTemplate, int tileSize, ChessColor playerColor) 
        {
            _rootPieceNode = rootNode;
            _pieceTemplate = pieceTemplate;
            _playerColor = playerColor;

            // Init Board
            for (int rank =  0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    _board[rank, file] = BoardTile.BuildEmptyTile(rank, file, VisualPiecesList);
                }
            }
        }

        public BoardTile GetTile(int rank, int file)
        {
            return _board[rank, file];
        }

        public void AddPiece(int rank, int file, ChessPieceId piece, ChessColor color)
        {
            BoardTile tile = GetTile(rank, file);

            // if there is a piece on this tile already, get rid of it
            tile.ClearTile();

            tile.UpdateTile(color, piece, _pieceTemplate, _rootPieceNode);
        }

        public void RemovePiece(int rank, int file)
        {
            BoardTile tile = GetTile(rank, file);
            tile.ClearTile();
        }

        public void MovePiece(int rank, int file, int targetRank, int targetFile)
        {
            BoardTile startingTile = GetTile(rank, file);
            BoardTile targetTile = GetTile(targetRank, targetFile);

            if (MoveVerificator.IsMovePossible(startingTile, targetTile, this))
            {
                AddPiece(targetRank, targetFile, startingTile.PieceId, startingTile.PieceColor);
                startingTile.ClearTile();
            }
        }

        public void BuildBoardFromFEN(string fenString)
        {
            BoardTile[,] newBoard = FEN.Decrypt(fenString);

            ClearBoard();

            _board = newBoard;
            BuildCurrentBoard();
        }

        public void ClearBoard()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    GetTile(i, j).ClearTile();
                }
            }
        }

        public void BuildCurrentBoard()
        {
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    var currentTile = GetTile(rank, file);

                    if (currentTile.IsListNull())
                    {
                        currentTile.AddPointerToVPList(VisualPiecesList);
                    }

                    currentTile.BuildTile(_pieceTemplate, _rootPieceNode);
                }
            }
        }

        public VisualChessPiece GetVisualChessPieceAtGridLoc(int rank, int file)
        {
            return GetTile(rank, file).VisualPiece;
        }

        public List<Vector2I> GetMovesForPiece(int rank, int file)
        {
            BoardTile boardTile = GetTile(rank, file);
            bool isPlayer = boardTile.PieceColor == _playerColor;

            List<Vector2I> theoMoves = MoveVerificator.GetMovesAssumingEmptyBoard(boardTile.PieceId, rank, file, isPlayer);

            return MoveVerificator.GetCapableMoves(rank, file, boardTile, isPlayer, theoMoves, this);
        }
    }
}
