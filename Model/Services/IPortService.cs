namespace ServerInstancingService.Model.Services;

public interface IPortService
{
    public int TryGetPort();
    public void ReturnPort(int port);
}