using Appointment.Domain.DTO;
using Appointment.Domain.Exceptions;
using Newtonsoft.Json;
using System;
using System.Net;
namespace Appointment.Api.Exceptions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next) => _next = next;
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            if(exception is DomainException)
            {
                var body = new ExceptionDto((int)HttpStatusCode.BadRequest, exception.GetType().Name, exception.Message);

                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return context.Response.WriteAsync(JsonConvert.SerializeObject(body));
            }
            else if(exception is BadHttpRequestException)
            {
                var body = new ExceptionDto((int)HttpStatusCode.BadRequest, exception.GetType().Name, exception.Message);

                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return context.Response.WriteAsync(JsonConvert.SerializeObject(body));
            }
            else
            {
                var body = new ExceptionDto((int)HttpStatusCode.InternalServerError, exception.GetType().Name, exception.Message);

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return context.Response.WriteAsync(JsonConvert.SerializeObject(body));
            }
        }
    }
}
