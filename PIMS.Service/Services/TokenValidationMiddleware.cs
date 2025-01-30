using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PIMS.EntityFramework.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace PIMS.Service.Services
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _nextMiddleware;
        private readonly IServiceProvider _serviceProvider;

        public TokenValidationMiddleware(RequestDelegate nextMiddleware, IServiceProvider serviceProvider)
        {
            _nextMiddleware = nextMiddleware;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var authorizationHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
            var token = authorizationHeader?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var applicationDbContext = scope.ServiceProvider.GetRequiredService<PimsDbContext>();

                    var blackListToken = await applicationDbContext.Usersessions.FirstOrDefaultAsync(t => t.Token == token);

                    if (blackListToken != null)
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        httpContext.Response.ContentType = "application/json";
                        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new { message = "Token has been invalidated." }));
                        return;
                    }
                }

                if (AddUserToHttpContext(httpContext, token))
                {
                    Console.WriteLine("Middleware Called, User is Authorized");
                }
                else
                {
                    Console.WriteLine("Invalid Token. User is not Authorized");
                }
            }
            else
            {
                Console.WriteLine("No Authorization header found. User is not Authorized");
            }

            await _nextMiddleware(httpContext);
        }

        private bool AddUserToHttpContext(HttpContext httpContext, string token)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                if (jwtTokenHandler.CanReadToken(token))
                {
                    var jwtToken = jwtTokenHandler.ReadJwtToken(token);

                    var claims = jwtToken.Claims.ToList();

                    var claimsIdentity = new ClaimsIdentity(claims, "JwtSettings");

                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    httpContext.User = claimsPrincipal;

                    httpContext.Items["JwtToken"] = token;

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading JWT token: {ex.Message}");
            }

            return false;
        }
    }
}