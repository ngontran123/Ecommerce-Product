using Ecommerce_Product.Repository;


namespace Ecommerce_Product.Support_Serive;

public class NotFoundMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;    

    public NotFoundMiddleware(RequestDelegate next,IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider=serviceProvider;
    }
    

    public async Task InvokeAsync(HttpContext context)
    {   
        if(!IsPageRoute(context.Request.Path.Value))
        {
            await _next(context);
            return;
        }
        int status=0;

        var path = context.Request.Path.Value;
         Console.WriteLine("ADMIn PATHH:"+path);

      if(!context.Request.Path.StartsWithSegments("/admin"))
      {
         Console.WriteLine("ADMIn PATH:"+path);

       using(var scope = _serviceProvider.CreateScope())
    {
        var _setting = scope.ServiceProvider.GetRequiredService<ISettingRepository>();
    
    var isUserMaintenancePage = path.Contains("maintainance", StringComparison.OrdinalIgnoreCase);
   
    Console.WriteLine("USER MAINTENANCE:"+isUserMaintenancePage);


        //var _sp_services = scope.ServiceProvider.GetRequiredService<Service>();
         status=await _setting.getStatusByName("maintainance");
            if(status==1 && !isUserMaintenancePage)
        {  
            context.Response.Redirect("maintainance");            
            return;
        }
    }
    }
   

    await _next(context);
    
     
        if (context.Response.StatusCode == 404)
        {  
            if (context.Request.Path.StartsWithSegments("/admin"))
            {
                context.Response.Redirect("Error/404");
            }
            else
            {
                context.Response.Redirect("404");
            }
        }
    }

    private bool IsPageRoute(string path)
{
    return (path .StartsWith("/") || path.StartsWith("/admin") || path.StartsWith("/page"))
           && !path.Contains(".")
           && !path.StartsWith("/assets") && !path.StartsWith("/css") 
           && !path.StartsWith("/js") && !path.StartsWith("/images");
}
      
}