


using KIITStarter.Data;
using KIITStarter.Models;
using Microsoft.EntityFrameworkCore;

public interface ISessionService
{
    Task<bool> addSession(string token, int user);

    Task validateSession(string token, int user);
    Task<bool> clearSession();
}


public class SessionService : ISessionService
{

    private readonly AppDbContext _context;


    public SessionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> addSession(string token, int user)
    {
        Session newSession = new Session
        {
            User = user,
            createdAt = DateTime.Now,
            token = token
        };

        _context.Sessions.Add(newSession);
        await _context.SaveChangesAsync();

        return true;

    }

    public Task<bool> clearSession()
    {
        throw new NotImplementedException();
    }

    public async Task validateSession(string token, int user)
    {

        try
        {
            var userSession = await _context.Sessions.FirstAsync(x => x.User == user &&
             x.token == token);

            if (userSession == null) throw new Exception("Invalid session here");

        }
        catch (System.Exception e)
        {
            throw new Exception("Invalid session");
        }
    }
}