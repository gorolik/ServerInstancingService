using ServerInstancingService.Model;

namespace ServerInstancingService.Utils;

public static class ExitCodeInterpreter
{
    public static string Translate(int exitCode, out LogType logType)
    {
        switch (exitCode)
        {
            case -1:
                logType = LogType.Error;
                return "Instance management problem (-1)";
            case 0:
                logType = LogType.Warning;
                return "Completed (0)";
            case 1:
                logType = LogType.Error;
                return "Launch parameters interpretation problem (1)";
            case 2:
                logType = LogType.Info;
                return "All players disconnected (2)";
            case 3:
                logType = LogType.Info;
                return "Match ended (3)";
            case 137:
                logType = LogType.Error;
                return "Killed by backend (OOM / Kill, 137)";
            default:
                logType = LogType.Error;
                return $"Unknown exit code ({exitCode})";
        }
    }
}