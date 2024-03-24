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
        try
        {
            return _courseRepository.GetAll();
        }
        catch (System.Exception)
        {
            throw new CustomErrorException(new CustomError { Code = "COURSES_FETCH_ERROR", Message = "Failed to fetch courses data", ReportToTeam = true });
        }
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