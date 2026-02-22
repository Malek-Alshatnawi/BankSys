namespace BankSys.Middleware
{
    using BankSys.Models;
    using Microsoft.EntityFrameworkCore;
    using System.Text;

    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, MyDBContext db)
        {
            var requestTime = DateTime.Now;

            string requestBody = "";
            string responseText = "";
            string? exceptionMessage = null;
            string? stackTrace = null;

            // 🧾 Query String
            var queryString = context.Request.QueryString.ToString();

            // 🌍 User Agent
            var userAgent = context.Request.Headers["User-Agent"].ToString();

            // 🧠 Controller & Action
            var endpoint = context.GetEndpoint();
            var actionDescriptor = endpoint?.Metadata
                .GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();

            var controllerName = actionDescriptor?.ControllerName;
            var actionName = actionDescriptor?.ActionName;

            // ✅ قراءة الـ Request Body (فقط لو فيه محتوى)
            if (context.Request.ContentLength > 0)
            {
                context.Request.EnableBuffering();

                using var reader = new StreamReader(
                    context.Request.Body,
                    Encoding.UTF8,
                    leaveOpen: true);

                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            // ✅ التقاط الـ Response
            var originalBodyStream = context.Response.Body;
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                stackTrace = ex.StackTrace;
                throw;
            }

            var responseTime = DateTime.Now;

            // قراءة الـ Response
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            await responseBodyStream.CopyToAsync(originalBodyStream);

            // ⏱ Response Size
            var responseSize = responseText?.Length ?? 0;

            // 🧮 IsSuccess
            var isSuccess = context.Response.StatusCode < 400;

            // ✅ حفظ في الداتابيز
            var log = new ApiLog
            {
                Method = context.Request.Method,
                Path = context.Request.Path,
                QueryString = queryString,
                ControllerName = controllerName,
                ActionName = actionName,
                StatusCode = context.Response.StatusCode,
                RequestTime = requestTime,
                ResponseTime = responseTime,
                DurationMs = (int)(responseTime - requestTime).TotalMilliseconds,
                Ipaddress = context.Connection.RemoteIpAddress?.ToString(),
                UserName = context.User?.Identity?.Name,
                UserAgent = userAgent,
                RequestBody = requestBody,
                ResponseBody = responseText,
                ResponseSize = responseSize,
                IsSuccess = isSuccess,
                ExceptionMessage = exceptionMessage,
                StackTrace = stackTrace
            };

            db.ApiLogs.Add(log);
            await db.SaveChangesAsync();
        }


    }
}