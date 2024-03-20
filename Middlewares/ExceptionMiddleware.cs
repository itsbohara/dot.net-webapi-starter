using Microsoft.AspNetCore.Mvc;

namespace KIITStarter.Middlewares;
public class ExceptionMiddleware
{

    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (System.Exception e)
        {
            if (e.GetType() == typeof(CustomErrorException))
            {
                var error = (CustomErrorException)e;

                // save log
                // await saveErrorLog(error.Code, error.Message);
            }

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { message = e.Message ?? "Internal Server Error" });
        }

    }

    public async Task saveErrorLog(string code, string error)
    {
        // await logService.SaveLog(new Models.SystemLog
        // {
        //     LogLevel = 0,
        //     Code = code,
        //     Error = error,
        // });
    }
}