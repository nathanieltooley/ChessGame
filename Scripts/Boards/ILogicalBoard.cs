using ChessGame.Scripts.DataTypes;

namespace ChessGame.Scripts.ChessBoard
{
    public interface ILogicalBoard
    {
        void AddPiece(BoardPos boardPos, PieceInfo piece);
        void ClearBoard();
        PieceInfo[,] GetBoard();
        PieceInfo GetPieceInfoAtPos(BoardPos boardPos);
        void MovePiece(BoardPos startingPos, BoardPos targetPos);
        void RemovePiece(BoardPos boardPos);
    }
}