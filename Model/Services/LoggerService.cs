namespace ServerInstancingService.Model.Services;

public class LoggerService
{
    public void Log(LogType type, string message)
    {
        string line = "[API] ";
        line += Enum.GetName(typeof(LogType), type) + ": ";
        line += message;
        
        Console.WriteLine(line);
    }
}