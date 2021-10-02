using Microsoft.AspNetCore.Authentication;

namespace SimpleCmsWebApi
{
    public class SecretTokenAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string SchemeName = "TokenAuthentication";
        public string TokenHeaderName { get; set; } = "SuperToken";
        public string SuperSecretToken { get; set; }
    }
}
