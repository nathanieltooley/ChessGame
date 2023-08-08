using ChessGame.Scripts;
using Godot;
using System.Collections.Generic;

public partial class MoveInfoService : Node
{
    public List<BoardPos>[,] MoveCache { get; set; }

	public List<BoardPos> GetMovesAtPos(BoardPos pos)
    {
        return MoveCache[pos.Rank, pos.File];
    }
}
