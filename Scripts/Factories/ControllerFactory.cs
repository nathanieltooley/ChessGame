﻿
using ChessGame.Scripts.ChessBoard;
using ChessGame.Scripts.Controllers;
using ChessGame.Scripts.DataTypes;
using Godot;

namespace ChessGame.Scripts.Factories
{
    public static class ControllerFactory
    {
        public static Node RootNode { get; set; }

        public static BoardController GetBoardController()
        {
            return RootNode.GetNode<BoardController>("/root/Main/Controllers/BoardController");
        }

        public static PlayerMovementController GetPlayerMovementController()
        {
            return RootNode.GetNode<PlayerMovementController>("/root/Main/Controllers/PlayerInputController");
        }
    }
}
