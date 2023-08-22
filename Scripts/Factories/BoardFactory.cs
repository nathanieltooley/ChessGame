using ChessGame.Scripts.ChessBoard.Boards;
using Godot;

namespace ChessGame.Scripts.Factories
{
    public static class BoardFactory
    {
        public static Node RootNode { get; set; }

        public static GraphicalBoard GetGraphicalBoard()
        {
            return RootNode.GetNode<GraphicalBoard>("/root/Main/Boards/GraphicalBoard");
        }
    }
}
