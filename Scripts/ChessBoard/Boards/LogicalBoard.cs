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
        private Dictionary<BoardPos, List<BoardPos>> _cachedMoves = new Dictionary<BoardPos, List<BoardPos>>();

        public List<VisualChessPiece> VisualPiecesList { get; set; } = new List<VisualChessPiece>();

        public LogicalBoard()
        {

        }

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
                    _board[rank, file] = BoardTile.BuildEmptyTile(new BoardPos(rank, file), VisualPiecesList);
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

            _cachedMoves.Remove(tile.TilePos);
        }

        public void MovePiece(BoardPos startingPos, BoardPos targetPos, bool isPlayerMove, out bool success)
        {
            BoardTile startingTile = GetTile(startingPos.Rank, startingPos.File);
            BoardTile targetTile = GetTile(targetPos.Rank, targetPos.File);

            List<BoardPos> moves;

            if (_cachedMoves.ContainsKey(startingTile.TilePos))
            {
                _cachedMoves.TryGetValue(startingTile.TilePos, out moves);
            } else
            {
                MoveFinder mf = new MoveFinder(this, startingTile, isPlayerMove);

                moves = mf.GetCapableMoves(startingPos.Rank, startingPos.File, startingTile, mf.GetMovesAssumingEmptyBoard());
                AddToCachedMoves(startingPos, moves);
            }

            if (MoveFinder.IsMovePossible(targetPos, moves))
            {
                AddPiece(targetPos.Rank, targetPos.File, startingTile.PieceId, startingTile.PieceColor);
                startingTile.ClearTile();

                _cachedMoves.Remove(startingTile.TilePos);

                success = true;
            } else
            {
                success = false;
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

        public List<BoardPos> GetMovesForPiece(int rank, int file, bool isPlayerMove)
        {
            BoardTile boardTile = GetTile(rank, file);
            bool isPlayer = boardTile.PieceColor == _playerColor;

            if (_cachedMoves.ContainsKey(boardTile.TilePos))
            {
                List<BoardPos> moves;

                _cachedMoves.TryGetValue(boardTile.TilePos, out moves);
                return moves;
            } else
            {
                MoveFinder mf = new MoveFinder(this, boardTile, isPlayerMove);
                return mf.GetCapableMoves(rank, file, boardTile, mf.GetMovesAssumingEmptyBoard());
            }
        }

        private void AddToCachedMoves(BoardPos boardPos, List<BoardPos> moves)
        {
            if (!_cachedMoves.ContainsKey(boardPos))
            {
                _cachedMoves.Add(boardPos, moves);
            } else
            {
                // Update the cache if we already have this key
                _cachedMoves.Remove(boardPos);
                _cachedMoves.Add(boardPos, moves);
            }
        }
    }
}
