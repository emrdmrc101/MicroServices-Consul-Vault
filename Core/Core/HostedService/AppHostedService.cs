using Core.Consul;
using Microsoft.Extensions.Hosting;

namespace Core.HostedService;

public class AppHostedService(
    ConsulRegistrationService consulRegistrationService,
    VaultService vaultService) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await consulRegistrationService.RegisterServiceAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}