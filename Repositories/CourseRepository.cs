using KIITStarter.Data;
using KIITStarter.Models;
using Microsoft.EntityFrameworkCore;

namespace KIITStarter.Repositories;

public interface ICourseRepository
{
    Task<IEnumerable<Course>> GetAll();
    Task<Course> GetById(int id);
    Task<Course> Create(Course course);
    Task Update(int id, Course course);
    Task Delete(int id);
}

public class CourseRepository : ICourseRepository
{

    private readonly AppDbContext _context;
    public CourseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Course> Create(Course course)
    {
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        return course;
    }

    public async Task Delete(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Course>> GetAll()
    {
        return _context.Courses.Include(x => x.Author).ToList();
    }

    public async Task<Course> GetById(int id)
    {
        return await _context.Courses.FindAsync(id);
    }

    public Task Update(int id, Course course)
    {
        throw new NotImplementedException();
    }

    private bool CourseExists(int id)
    {
        return _context.Courses.Any(e => e.Id == id);
    }
}