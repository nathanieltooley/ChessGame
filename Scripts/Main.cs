using ChessGame.Scripts;
using ChessGame.Scripts.Controllers;
using ChessGame.Scripts.Factories;
using Godot;

public partial class Main : Node2D
{

    private BoardController _boardController;

    [Export]
    private ChessColor playerColor = ChessColor.White;
    private ChessColor aiColor = ChessColor.Black;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        RunEnvironment runEnv = GetNode<RunInfo>("/root/RunInfo").GameRunEnvironment;

        GameInfoService gameInfoService = ServiceFactory.GetGameInfoService();
        TurnService turnService = ServiceFactory.GetTurnService();
        TimerService timerService = ServiceFactory.GetTimerService();

        _boardController = ControllerFactory.GetBoardController();

        aiColor = InvertColor(playerColor);

        ChessSide whitePlayer;
        ChessSide blackPlayer;

        if (playerColor == ChessColor.White)
        {
            whitePlayer = ChessSide.Player;
            blackPlayer = ChessSide.Enemy;
        } else
        {
            whitePlayer = ChessSide.Enemy;
            blackPlayer = ChessSide.Player;
        }

        gameInfoService.SetupGameInfo(runEnv, playerColor, aiColor);
        turnService.Setup(whitePlayer, blackPlayer);
        timerService.Setup(blackPlayer);

        // Init Timer BG Colors
        timerService.EmitTimerColorUpdateSignal(ChessSide.Enemy, aiColor);
        timerService.EmitTimerColorUpdateSignal(ChessSide.Player, playerColor);

        switch (runEnv)
        {
            case RunEnvironment.Normal:
                _boardController.CreateDefaultBoard();
                break;
            case RunEnvironment.Test:
                _boardController.Test();
                break;
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

    }


    private ChessColor InvertColor(ChessColor color)
    {
        if (color == ChessColor.White)
        {
            return ChessColor.Black;
        }
        else
        {
            return ChessColor.White;
        }
    }
}
