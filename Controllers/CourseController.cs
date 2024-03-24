using KIITStarter.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

[Route("courses")]
[ApiController]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly IUserService _userService;

    public CoursesController(ICourseService courseService, IUserService userService)
    {
        _userService = userService;
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<IEnumerable<Course>> GetCourses()
    {
        return await _courseService.GetCourses();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Course>> GetCourse(int id)
    {
        var course = await _courseService.GetCourse(id);

        if (course == null)
        {
            return NotFound();
        }

        return course;
    }

    [HttpPost]
    public async Task<ActionResult<Course>> CreateCourse(Course course)
    {
        try
        {
            course.IsPaid = false;
            course.CreatedAt = DateTime.Now;
            course.ConvertToLocalTime();
            // ! demo
            course.Author = await _userService.GetById(2);

            await _courseService.CreateCourse(course);

            return CreatedAtAction("GetCourse", new { id = course.Id }, course);
        }
        catch (System.Exception)
        {
            throw new CustomErrorException(new CustomError { Code = "COURSE_CREATE_ERROR", Message = "Failed to save course data", Payload = JsonSerializer.Serialize(course), ReportToTeam = true });
        }
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        try
        {
            await _courseService.DeleteCourse(id);

            return NoContent();
        }
        catch (System.Exception)
        {
            return NotFound();
        }

    }

}
