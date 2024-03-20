using KIITStarter.Data;
using KIITStarter.Models;
using Microsoft.EntityFrameworkCore;

namespace KIITStarter.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAll();
    Task<User> GetById(int id);
    Task<User> FindByEmail(string email);
    Task<User> Create(User user);
    Task Update(int id, User user);
    Task Delete(int id);
}

public class UserRepository : IUserRepository
{

    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> Create(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task Delete(int id)
    {
        var user = await _context.Users.FindAsync(id);
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User> FindByEmail(string email)
    {
        var user = await Task.Run(() => _context.Users.FirstOrDefault(x => x.Email == email));
        return user;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return _context.Users.ToList();
    }

    public async Task<User> GetById(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public Task Update(int id, User user)
    {
        throw new NotImplementedException();
    }

    private bool CourseExists(int id)
    {
        return _context.Users.Any(e => e.Id == id);
    }
}