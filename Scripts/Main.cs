using ChessGame.Scripts;
using ChessGame.Scripts.Controllers;
using ChessGame.Scripts.Factories;
using ChessGame.Scripts.Helpers;
using Godot;

public partial class Main : Node2D
{

    private BoardController _boardController;

    private TurnService _turnService;
    private GameInfoService _gameInfoService;
    private TimerService _timerService;
    private Control _gameOverMenu;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _gameInfoService = ServiceFactory.GetGameInfoService();
        _turnService = ServiceFactory.GetTurnService();
        _timerService = ServiceFactory.GetTimerService();

        _boardController = ControllerFactory.GetBoardController();

        _gameOverMenu = GetNode<Control>("MainGameGUI/GameOverMenu");

        Load();
        
    }

    public void Load()
    {
        RunInfo runInfo = GetNode<RunInfo>("/root/RunInfo");
        RunEnvironment runEnv = runInfo.GameRunEnvironment;

        var playerColor = runInfo.PlayerColor;
        var aiColor = MiscHelpers.InvertColor(playerColor);

        ChessSide whitePlayer;
        ChessSide blackPlayer;

        // Init Player Colors
        if (playerColor == ChessColor.White)
        {
            whitePlayer = ChessSide.Player;
            blackPlayer = ChessSide.Enemy;
        }
        else
        {
            whitePlayer = ChessSide.Enemy;
            blackPlayer = ChessSide.Player;
        }

        // Setup Services
        _gameInfoService.SetupGameInfo(runEnv, playerColor, aiColor);
        _turnService.Setup(whitePlayer, blackPlayer);
        _timerService.Setup(blackPlayer);

        // Init Timer BG Colors
        _timerService.EmitTimerColorUpdateSignal(ChessSide.Enemy, aiColor);
        _timerService.EmitTimerColorUpdateSignal(ChessSide.Player, playerColor);

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

    public void OnRestartButtonPressed()
    {
        // Load();
        // _gameOverMenu.Visible = false;
        GetTree().ChangeSceneToFile("res://chess_board.tscn");
    }

    public void OnReturnButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://MainMenu.tscn");
    }
}
