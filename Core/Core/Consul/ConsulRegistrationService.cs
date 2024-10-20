using System.Net.Http.Json;
using Core.Domain.Consul;
using Microsoft.Extensions.Configuration;

namespace Core.Consul;

public class ConsulRegistrationService(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration)
{
    public async Task RegisterServiceAsync()
    {
        var consulSettings = configuration.GetSection("Consul").Get<ConsulSettings>();
        var serviceUrl = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(";").FirstOrDefault();
        var client = httpClientFactory.CreateClient("ConsulClient");

        var consulServiceRegistration = new
        {
            ID = consulSettings.ServiceId,
            Name = consulSettings.ServiceName,
            Address = serviceUrl,
            //Port = 5000,
            Check = new
            {
                HTTP = $"{serviceUrl}/health",
                Interval = "10s",
                Timeout = "5s"
            }
        };

        var response = await client.PutAsJsonAsync("/v1/agent/service/register", consulServiceRegistration);

        Console.WriteLine(response.IsSuccessStatusCode
            ? "Service successfully registered with Consul."
            : "Registration with the Consul failed.");
    }
}