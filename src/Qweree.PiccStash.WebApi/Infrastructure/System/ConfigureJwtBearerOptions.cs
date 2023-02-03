using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace Qweree.PiccStash.WebApi.Infrastructure.System;

public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly IOptions<QwereeConfigurationDo> _qwereeOptions;
    private readonly FileAccessKey _fileAccessKey;

    public ConfigureJwtBearerOptions(IOptions<QwereeConfigurationDo> qwereeOptions, FileAccessKey fileAccessKey)
    {
        _qwereeOptions = qwereeOptions;
        _fileAccessKey = fileAccessKey;
    }

    public void Configure(JwtBearerOptions options)
    {
        Configure(string.Empty, options);
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name == "AccessToken")
        {
            options.RequireHttpsMetadata = false;
            options.Authority = _qwereeOptions.Value.AuthUri;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.FromMinutes(5),
                CryptoProviderFactory = new CryptoProviderFactory
                {
                    CacheSignatureProviders = false
                }
            };
        }
        else if (name == "FileAccessToken")
        {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = _fileAccessKey.GetKey()
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    string? token;

                    if (context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var values))
                    {
                        token = values.FirstOrDefault();

                        if (token == null)
                            return Task.CompletedTask;

                        const string prefix = "Bearer ";

                        if (token.StartsWith(prefix))
                            token = token.Substring(prefix.Length);
                        else
                            token = null;
                    }
                    else if (context.Request.Query.TryGetValue("access_token", out values))
                    {
                        token = values.FirstOrDefault();
                    }
                    else
                    {
                        return Task.CompletedTask;
                    }

                    context.Token = token;
                    return Task.CompletedTask;
                }
            };
        }
    }
}