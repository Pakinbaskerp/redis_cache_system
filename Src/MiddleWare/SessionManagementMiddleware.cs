using RedisProductAPI.Domain.Dto;
using RedisProductAPI.Infrastructure.Contract;

namespace RedisProductAPI.MiddleWare;

public class SessionManagementMiddleware {
    private readonly RequestDelegate _requestDelegate;
    private readonly IRedisCacheService _redisCacheService;

    public SessionManagementMiddleware(
        RequestDelegate requestDelegate,
        IRedisCacheService redisCacheService)
        {
            _redisCacheService = redisCacheService;
            _requestDelegate = requestDelegate;
        }
    
    public async Task InvokeAsync(HttpContext httpContext){
        string path = httpContext.Request.Path.ToString().ToLower();
        if(path.Contains("/login")){
            await _requestDelegate(httpContext);
            return;
        }

        if(!httpContext.Request.Headers.TryGetValue("SessionId", out var sessionId)){
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsync("Session Id is missing");
            return ;
        }
        var cacheSession =await _redisCacheService.GetDataAsync<LoginDto>(sessionId!);

        if(cacheSession is null){
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsync("Session is invalid...");
            return;
        }

        await _requestDelegate(httpContext);
    }
}