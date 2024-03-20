using KIITStarter.Data;
using KIITStarter.Models;
using KIITStarter.Repositories;
using Microsoft.AspNetCore.Mvc;

public class UserLoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}
public class UserRegisterRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class AuthResponse
{
    public bool success { get; set; }
    public string? message { get; set; }
    public string? token { get; set; }
}

public interface IUserService
{
    // IEnumerable<User> GetAll();
    Task<AuthResponse> Login(UserLoginRequest model);
    Task<AuthResponse> Register(UserRegisterRequest model);
    Task<User> GetById(int id);
}

public class UserService : IUserService
{
    
    private readonly IUserRepository _userRepository;

    private readonly ISessionService _sessionService;
    private readonly IJwtService _jwtService;

    public UserService(IUserRepository userRepo, ISessionService sessionService, IJwtService jwtService)
    {
        _userRepository = userRepo;
        _sessionService = sessionService;
        _jwtService = jwtService;
    }

    public async Task<User> GetById(int id)
    {
        var user = await _userRepository.GetById(id);

        if (user == null) throw new KeyNotFoundException("User not found");

        return user;
    }

    public async Task<AuthResponse> Login(UserLoginRequest userRequest)
    {
        var user = await Task.Run(() => _userRepository.FindByEmail(userRequest.Email));

        // validate user/pass
        var failedAuthResponse = new AuthResponse { success = false, message = "Invalid credentials" };

        if (user == null)
        {
            throw new CustomErrorException(new CustomError { Code = "USER_AUTH_ERROR", Message = "Invalid credentials" });
            // return failedAuthResponse;
        }

        var authToken = _jwtService.generateToken(user);

        var authResponse = new AuthResponse
        {
            success = true,
            token = authToken,
            message = "Login Successfull"
        };

        // save session data
        await _sessionService.addSession(authToken, user.Id);

        return authResponse;
    }

    public async Task<AuthResponse> Register(UserRegisterRequest userRequest)
    {
        try
        {
            // save user data
            User user = new User
            {
                Email = userRequest.Email,
                Password = userRequest.Password,
                CreatedAt = DateTime.Now,
            };

            await _userRepository.Create(user);

            var authToken = _jwtService.generateToken(user);

            // save session data
            await _sessionService.addSession(authToken, user.Id);

            var authResponse = new AuthResponse
            {
                success = true,
                token = authToken,
                message = "Registration Successfull!"
            };
            return authResponse;

        }
        catch (System.Exception e)
        {
            return new AuthResponse { success = false, message = "Failed to register" };
        }
    }


}