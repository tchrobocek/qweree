using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Commands;
using Qweree.ConsoleApplication.Infrastructure.Authentication;
using Qweree.ConsoleApplication.Infrastructure.Commands;

namespace Qweree.ConsoleApplication.Commands
{
    public class LoginCommand : ICommand
    {
        private readonly AuthenticationService _authenticationService;

        public LoginCommand(AuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public string CommandPath => "login";

        public async Task<int> ExecuteAsync(OptionsBag optionsBag, CancellationToken cancellationToken = new())
        {
            if (!optionsBag.Options.TryGetValue("--username", out var userNameCollection))
                return -1;

            var username = userNameCollection.Single();
            var password = Console.ReadLine() ?? string.Empty;

            try
            {
                await _authenticationService.AuthenticateAsync(new PasswordGrantInput(username, password), cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }

            return 0;
        }
    }
}