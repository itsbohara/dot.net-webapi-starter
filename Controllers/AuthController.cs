
using KIITStarter.Data;
using KIITStarter.Helpers;
using KIITStarter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

[Route("/auth/")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    private readonly IUserService _userService;

    public AuthController(AppDbContext context, IConfiguration config, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> LoginUser(UserLoginRequest userRequest)
    {
        return await _userService.Login(userRequest);
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> RegisterUser(UserRegisterRequest userRequest)
    {
        var result = await _userService.Register(userRequest);
        
        if (result.success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [Authorize]
    [HttpGet("session")]
    public string validateSession()
    {
        var currentUser = (User)HttpContext.Items["User"];

        return currentUser.Email;
    }


}

