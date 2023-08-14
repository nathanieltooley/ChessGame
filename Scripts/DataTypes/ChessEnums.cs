using System;

namespace ChessGame.Scripts
{
    public enum ChessPieceId
    {
        Empty = 0,
        Pawn = 1,
        King = 2,
        Bishop = 3,
        Rook = 4,
        Knight = 5,
        Queen = 6
    }

    public enum ChessColor
    {
        White = 0,
        Black = 1,
    }

    public enum ChessSide
    {
        Enemy = 0,
        Player = 1,
    }

    public enum CastleSide
    {
        KingSide = 0,
        QueenSide = 1
    }
}
