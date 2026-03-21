namespace ServerInstancingService.Model.Data;

public class AllocationData
{
    public string Ip { get; set; }
    public string Port { get; set; }

    public override string ToString()
    {
        return Ip + ":" + Port;
    }
}