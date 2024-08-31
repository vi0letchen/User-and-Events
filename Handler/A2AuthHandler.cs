using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using A2.Data;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Security.Claims;

namespace A2.Handler
{
    public class A2AuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IA2Repo _repository;

        public A2AuthHandler(
            IA2Repo repository,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _repository = repository;
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                Response.Headers.Add("WWW-Authenticate", "Basic");
                return AuthenticateResult.Fail("Authorization header not found.");
            }
            else
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(":");
                var username = credentials[0];
                var password = credentials[1];
                
                var organizer = _repository.GetOrganizerByName(username);
                var user = _repository.GetUserByName(username);
                

                if (_repository.ValidLogin(username, password))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, username)
                    };

                    if (organizer != null) {

                        claims.Add(new Claim(ClaimTypes.Role, "organizer"));
                    }
                    if (user != null) {

                        claims.Add(new Claim(ClaimTypes.Role, "user"));

                    }

                    ClaimsIdentity identity = new ClaimsIdentity(claims, "Basic");
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                    AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);
                    return AuthenticateResult.Success(ticket);
                }


                Response.Headers.Add("WWW-Authenticate", "Basic");
                return AuthenticateResult.Fail("userName and password do not match");
            }
        }
    }
}
