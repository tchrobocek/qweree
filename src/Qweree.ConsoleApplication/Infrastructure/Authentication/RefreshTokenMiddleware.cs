using System;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.ConsoleApplication.Infrastructure.RunContext;
using Qweree.ConsoleHost;
using Qweree.ConsoleHost.Extensions;

namespace Qweree.ConsoleApplication.Infrastructure.Authentication;

public class RefreshTokenMiddleware : IMiddleware
{
    private readonly AuthenticationService _authenticationService;
    private readonly Context _context;

    public RefreshTokenMiddleware(AuthenticationService authenticationService, Context context)
    {
        _authenticationService = authenticationService;
        _context = context;
    }

    public async Task NextAsync(ConsoleContext context, RequestDelegate? next,
        CancellationToken cancellationToken = new())
    {
        string token;

        try
        {
            token = await _context.GetRefreshTokenAsync(cancellationToken);
        }
        catch (Exception)
        {
            token = string.Empty;
        }

        if (!string.IsNullOrWhiteSpace(token))
        {
            try
            {
                await _authenticationService.AuthenticateAsync(new RefreshTokenGrantInput(token), cancellationToken);
            }
            catch (Exception)
            {
                //
            }
        }

        if (next != null)
            await next.Invoke(context, cancellationToken);
    }
}