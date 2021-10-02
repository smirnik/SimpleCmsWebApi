using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SimpleCmsWebApi.Authentication
{

    public class SecretTokenAuthenticationHandler : AuthenticationHandler<SecretTokenAuthenticationOptions>
    {
        public SecretTokenAuthenticationHandler(
            IOptionsMonitor<SecretTokenAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(Options.TokenHeaderName))
            {
                return Task.FromResult(AuthenticateResult.Fail($"Missing token header '{Options.TokenHeaderName}'"));
            }

            var token = Request.Headers[Options.TokenHeaderName];
            var isValidToken = token == Options.SuperSecretToken;

            if (!isValidToken)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid token"));
            }

            var claims = new List<Claim>
            {
                new Claim("token", token)
            };

            var identity = new ClaimsIdentity(claims);
            AuthenticationTicket ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
