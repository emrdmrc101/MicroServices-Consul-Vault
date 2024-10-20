using Core.Domain.Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Winton.Extensions.Configuration.Consul;

namespace Core.Consul;

public static class ConsulConfiguration
{
    public static void AddMyConsul(this WebApplicationBuilder builder)
    {
        var consulSettings = builder.Configuration.GetSection("Consul").Get<ConsulSettings>();
        builder.Configuration.AddConsul(consulSettings.Vault.Token, o =>
        {
            o.ConsulConfigurationOptions = config =>
            {
                config.Address = new Uri("http://localhost:8500");
            };
            o.Optional = true;
            o.ReloadOnChange = true;
        });
        
        builder.Services.AddHttpClient("ConsulClient" ,o =>
            {
                o.BaseAddress = new Uri("http://localhost:8500");
            })
            .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2)));
    }
}