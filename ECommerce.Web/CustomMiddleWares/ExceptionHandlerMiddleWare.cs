
using ECommerce.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Web.CustomMiddleWares
{
    public class ExceptionHandlerMiddleWare(RequestDelegate next, ILogger<ExceptionHandlerMiddleWare> logger)
    {

        public async Task InvokeAsync(HttpContext httpContext)

        {
            try
            {

                await next.Invoke(httpContext);


  


                await HandleNotFoundEndPoint(httpContext);



            }
            catch (Exception ex)
            {
                //logging 


                logger.LogError(ex, "Something Went wrong");

                //Custom Error Response 



                //problem details type 

                //change the status code 


                var problem = new ProblemDetails()
                {
                    Title = "Error While Processing HTTP Request",
                    Detail = ex.Message
                     ,
                    Status = ex switch
                    {
                        NotFoundException => StatusCodes.Status404NotFound,
                        UnauthorizedAccessException=>StatusCodes.Status401Unauthorized,
                        _ => StatusCodes.Status500InternalServerError,
                    }

                    ,
                    //instance 

                    Instance = httpContext.Request.Path
                };
                httpContext.Response.StatusCode = problem.Status.Value;

                httpContext.Response.ContentType = "application/json"; // ✅ Add this

                await httpContext.Response.WriteAsJsonAsync(problem);





            }
        }

        private static async Task HandleNotFoundEndPoint(HttpContext httpContext)
        {
            if (httpContext.Response.StatusCode == StatusCodes.Status404NotFound && !httpContext.Response.HasStarted)
            {
                var response = new ProblemDetails()
                {
                    Title = "Error While Processing The HTTP Request - End Point Not Found",
                    Detail = $"End Point - {httpContext.Request.Path} is Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Instance = httpContext.Request.Path
                };

                await httpContext.Response.WriteAsJsonAsync(response);
            }
        }
    }


}
