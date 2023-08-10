using ChessGame.Scripts.Factories;
using Godot;

public partial class FactoryInit : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var rootNode = GetNode<Node>("/root/Main");

		ServiceFactory.RootNode = rootNode;
		ControllerFactory.RootNode = rootNode;
		BoardFactory.RootNode = rootNode;
    }

}
