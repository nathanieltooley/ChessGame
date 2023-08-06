using ChessGame.Scripts;
using ChessGame.Scripts.Controllers;
using ChessGame.Scripts.DataTypes;
using Godot;

public partial class Main : Node2D
{

    private BoardController _boardController;
    private PlayerMovementController _playerMovementController;

    [Export]
    private ChessColor playerColor = ChessColor.White;
    private ChessColor aiColor = ChessColor.Black;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        RunEnvironment runEnv = GetNode<RunInfo>("/root/RunInfo").GameRunEnvironment;

        GameInfoService gameInfoService = GetNode<GameInfoService>("/root/Main/GameInfoService");
        TurnService turnService = GetNode<TurnService>("/root/Main/TurnService");
        TimerService timerService = GetNode<TimerService>("/root/Main/TimerService");

        _boardController = GetNode<BoardController>("/root/Main/BoardController");
        _playerMovementController = GetNode<PlayerMovementController>("/root/Main/PlayerInputController");

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
        if (_playerMovementController.IsDragging)
        {
            _playerMovementController.PieceBeingDragged.Position = GetViewport().GetMousePosition();
        }
    }

    public override void _Input(InputEvent @event)
    {
        _playerMovementController.InputHandler(@event);
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
