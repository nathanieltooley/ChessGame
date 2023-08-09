using Godot;

namespace ChessGame.Scripts.Factories
{
    public static class ServiceFactory
    {
        public static Node RootNode { get; set; }

        private static string GetMainPath()
        {
            return "/root/Main";
        }

        private static string GetServicePath(string serviceName)
        {
            return $"{GetMainPath()}/Services/{serviceName}";
        }

        public static Service GetService<Service>(ServiceName name) where Service : class
        {
            return RootNode.GetNode<Service>(GetServicePath(name.ToString()));
        }

        public static GameInfoService GetGameInfoService()
        {
            return GetService<GameInfoService>(ServiceName.GameInfoService);
        }

        public static TimerService GetTimerService()
        {
            return GetService<TimerService>(ServiceName.TimerService);
        }

        public static TurnService GetTurnService()
        {
            return GetService<TurnService>(ServiceName.TurnService);
        }

        public static MoveInfoService GetMoveInfoService()
        {
            return GetService<MoveInfoService>(ServiceName.MoveInfoService);
        }
    }

    public enum ServiceName
    {
        GameInfoService,
        TimerService,
        TurnService,
        MoveInfoService
    }
}
