

using KIITStarter.Data;
using KIITStarter.Models;
using KIITStarter.Repositories;

public interface ICourseService
{
    Task<IEnumerable<Course>> GetCourses();
    Task<Course> GetCourse(int id);
    Task<Course> CreateCourse(Course course);
    Task DeleteCourse(int id);
}


public class CourseService : ICourseService
{

    private readonly ICourseRepository _courseRepository;

    public CourseService(ICourseRepository courseRepo)
    {
        _courseRepository = courseRepo;
    }


    public Task<Course> GetCourse(int id)
    {
        return _courseRepository.GetById(id);
    }

    public Task<IEnumerable<Course>> GetCourses()
    {
        return _courseRepository.GetAll();
    }

    public Task<Course> CreateCourse(Course course)
    {
        return _courseRepository.Create(course);
    }


    public async Task DeleteCourse(int id)
    {
        await _courseRepository.Delete(id);
    }

}