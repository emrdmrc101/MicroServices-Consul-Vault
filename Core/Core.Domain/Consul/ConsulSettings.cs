namespace Core.Domain.Consul;

public class ConsulSettings
{
    public string ServiceName { get; set; }
    public string ConfigName { get; set; }
    public string ServiceId { get; set; }
    public Vault Vault { get; set; }
}

public class Vault
{
    public string Token { get; set; }
    public string Address { get; set; }
}