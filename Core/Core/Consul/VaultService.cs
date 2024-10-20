using Core.Domain.Consul;
using Microsoft.Extensions.Configuration;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;

namespace Core.Consul;

public class VaultService
{
    public static void SetVaultSecrets(object configurations)
    {
        var configurationBuilder = (IConfigurationBuilder)configurations;
        var configuration = (IConfiguration)configurations;

        var consulSettings = configuration.GetSection("Consul").Get<ConsulSettings>();

        var authMethod = new TokenAuthMethodInfo(consulSettings?.Vault.Token);
        var vaultClientSettings = new VaultClientSettings(consulSettings?.Vault.Address, authMethod);

        var vaultClient = new VaultClient(vaultClientSettings);

        if (!string.IsNullOrWhiteSpace(consulSettings?.ConfigName))
        {
            Secret<SecretData> secret =
                vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(consulSettings?.ConfigName, mountPoint: "secret")
                    .Result;

            configurationBuilder.AddInMemoryCollection(
                secret.Data.Data.Select(kvp => new KeyValuePair<string, string?>(kvp.Key, kvp.Value?.ToString()))
            );
        }

        Secret<SecretData> commonSecret =
            vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("CommonConfig", mountPoint: "secret").Result;

        configurationBuilder.AddInMemoryCollection(
            commonSecret.Data.Data.Select(kvp => new KeyValuePair<string, string?>(kvp.Key, kvp.Value?.ToString()))
        );
    }
}