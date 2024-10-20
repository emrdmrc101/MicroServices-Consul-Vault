using System.Net.Http.Json;
using Core.Domain.Consul;
using Core.Domain.Log.Interfaces;

namespace Core.Consul;

public class ConsulService(IHttpClientFactory httpClientFactory, ILogger logger)
{
    public async Task<string?> GetServiceUrl(string serviceName)
    {
        var client = httpClientFactory.CreateClient("ConsulClient");

        var response = await client.GetAsync($"/v1/catalog/service/{serviceName}");

        if (!response.IsSuccessStatusCode) return null;

        var services1 = await response.Content.ReadAsStringAsync();
        
        var services = await response.Content.ReadFromJsonAsync<List<ConsulServiceModel>>(); 
        var service = services?.FirstOrDefault();

        if (service is null)
            await logger.Warning($"No service found. Name:{serviceName}");
            
        return service?.ServiceAddress;
    }

}