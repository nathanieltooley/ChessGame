using ChessGame.Scripts.ChessBoard;
using ChessGame.Scripts.ChessBoard.Boards;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Scripts.Factories
{
    public static class BoardFactory
    {
        public static Node RootNode { get; set; }

        public static GraphicalBoard GetGraphicalBoard()
        {
            return RootNode.GetNode<GraphicalBoard>("/root/Main/Boards/GraphicalBoard");
        }

        public static LogicalBoard GetLogicalBoard()
        {
            return RootNode.GetNode<LogicalBoard>("/root/Main/Boards/LogicalBoard");
        }
    }
}
