namespace KIITStarter.Helpers;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
// using KIITStarter.Services;

public class JwtMiddleware
{
    private IConfiguration _configuration;
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context, IUserService userService, ISessionService sessionService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
        {
            await attachUserToContext(context, userService, sessionService, token);
        }

        await _next(context);
    }

    private async Task attachUserToContext(HttpContext context,
    IUserService userService, ISessionService sessionService, string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                //
                ValidateIssuer = false,
                ValidateAudience = false,
                // ValidIssuer = _configuration["Jwt:Issuer"],
                // ValidAudience = _configuration["Jwt:Issuer"],
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

            // validate session data from database 
            await sessionService.validateSession(token, userId);

            // attach user to context on successful jwt validation
            context.Items["User"] = await userService.GetById(userId);
        }
        catch (System.Exception e)
        {

            Console.Write(e);

            // do nothing if jwt validation fails
            // user is not attached to context so request won't have access to secure routes
        }
    }
}