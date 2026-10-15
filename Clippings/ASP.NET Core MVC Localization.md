---
title: "ASP.NET Core MVC Localization"
source: "https://damienbod.com/2015/10/21/asp-net-5-mvc-6-localization/"
author:
  - "[[damienbod]]"
published: 2015-10-22
created: 2026-09-13
description: "This article shows some of the ways in which localization can be used in an ASP.NET Core MVC application. Code: https://github.com/damienbod/AspNetCoreLocalization/ Posts in this series ASP.NET Core MVC Localization Using DataAnnotations and Localization in ASP.NET Core MVC ASP.NET Core using SQL Localization 2018-12-05: Updated to ASP.NET Core 2.2 and EF Core 2.2 2017-08-19: Updated to…"
tags:
  - "clippings"
---
This article shows some of the ways in which localization can be used in an ASP.NET Core MVC application.

**Code:** [https://github.com/damienbod/AspNetCoreLocalization/](https://github.com/damienbod/AspNetCoreLocalization/)

**Posts in this series**

**2018-12-05:** Updated to ASP.NET Core 2.2 and EF Core 2.2  
**2017-08-19:** Updated to ASP.NET Core 2.0  
**2017-02-10:** Updated to VS2017 msbuild  
**2016-06-28:** Updated to ASP.NET Core 1.0 RTM  
**2016-05-16:** Updated to ASP.NET Core 1.0 RC2 dotnet  
**2015-11-20:** ASP.NET Core 1.0 RC1 version

**Localization Setup**

The localization is configured in the setup class and can be used throughout the application per dependency injection. The AddLocalization method is used in the ConfigureServices to define the resources and localization. This can then be used in the Configure method. Here, the RequestLocalizationOptions can be defined and is added to the stack using the UseRequestLocalization method. You can also define different options as required, for example the Accept-Header provider could be removed or a custom provider could be added.

```csharp
usingSystem;
usingSystem.Collections.Generic;
usingSystem.Linq;
usingSystem.Threading.Tasks;
usingMicrosoft.AspNetCore.Builder;
usingMicrosoft.AspNetCore.Hosting;
usingMicrosoft.AspNetCore.Identity.EntityFrameworkCore;
usingMicrosoft.EntityFrameworkCore;
usingMicrosoft.Extensions.Configuration;
usingMicrosoft.Extensions.DependencyInjection;
usingMicrosoft.Extensions.Logging;
 
usingMicrosoft.AspNetCore.Localization;
usingSystem.Globalization;
usingMicrosoft.Extensions.Options;
usingLocalization.SqlLocalizer.DbStringLocalizer;
 
namespaceAspNet5Localization
{
    usingSystem.IO;
    usingLocalization.SqlLocalizer;
 
    publicclassStartup
    {
        publicStartup(IHostingEnvironment env)
        {
            varbuilder = newConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
 
            if(env.IsDevelopment())
            {
                builder.AddUserSecrets();
            }
 
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }
 
        publicIConfigurationRoot Configuration { get; set; }
 
        publicvoidConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
     
            services.AddMvc()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();
 
            services.AddScoped<LanguageActionFilter>();
 
            services.Configure<RequestLocalizationOptions>(
                options =>
                    {
                        varsupportedCultures = newList<CultureInfo>
                        {
                            newCultureInfo("en-US"),
                            newCultureInfo("de-CH"),
                            newCultureInfo("fr-CH"),
                            newCultureInfo("it-CH")
                        };
 
                        options.DefaultRequestCulture = newRequestCulture(culture: "en-US", uiCulture: "en-US");
                        options.SupportedCultures = supportedCultures;
                        options.SupportedUICultures = supportedCultures;
                    });
        }
 
        publicvoidConfigure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
 
            varlocOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);
 
            app.UseStaticFiles();
 
            app.UseMvc();
        }
 
        publicstaticvoidMain(string[] args)
        {
            varhost = newWebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
 
            host.Run();
        }
    }
}
```

The localization can be used for example in an ASP.NET Core MVC controller. This is done by defining the IHtmlLocalizer with the name of your resx file(s). The resx files are defined in the folder defined in the Startup class AddLocalization method. The IHtmlLocalizer can then be used to return localized properties. A shared resource requires an emtpy class and special resource naming so that the magic string conventions work and the resource can be found. See the code for this.

```csharp
usingSystem.Globalization;
usingSystem.Threading;
usingMicrosoft.AspNetCore.Mvc;
usingMicrosoft.AspNetCore.Mvc.Localization;
usingMicrosoft.Extensions.Localization;
    
namespaceAspNet5Localization.Controllers
{
    [Route("api/[controller]")]
    publicclassAboutController : Controller
    {
        privatereadonlyIStringLocalizer<SharedResource> _localizer;
        privatereadonlyIStringLocalizer<AboutController> _aboutLocalizerizer;
 
        publicAboutController(IStringLocalizer<SharedResource> localizer, IStringLocalizer<AboutController> aboutLocalizerizer)
        {
            _localizer = localizer;
            _aboutLocalizerizer = aboutLocalizerizer;
        }
 
        [HttpGet]
        publicstringGet()
        {
            // _localizer["Name"] 
            return_aboutLocalizerizer["AboutTitle"];
        }
    }
}
```

**Setting the culture in the Query**

The culture required by the client application can be set in the query using the?culture=de-CH.  
The application can be run from the commandline, open the application in the src folder and enter:

```csharp
dotnet restore
 
dotnet run
```

Now the query localization can be tested or used as follows:

```xml
http://localhost:5000/api/About?culture=de-CH
 
http://localhost:5000/api/About?culture=it-CH
```

**Setting the culture in the Accept Header**

The HTTP Request Accept-Language header can also be used to request from the server the required culture.

This is implemented as follows for the de-CH culture

```xml
GET http://localhost:5000/api/About HTTP/1.1
 
Accept: */*
Accept-Language: de-CH
Host: localhost:5000
```

Or implemented as follows for the it-CH culture

```xml
GET http://localhost:5000/api/About HTTP/1.1
 
Accept: */*
Accept-Language: it-CH
Host: localhost:5000
```

**Setting the culture in the Request URL**

The culture can also be set in the URL. This is not supported out of the box and you must implement this yourself for example using an action filter.

The action filter can be implemented as follows:

```csharp
usingSystem.Globalization;
usingMicrosoft.Extensions.Logging;
usingMicrosoft.AspNetCore.Mvc.Filters;
usingMicrosoft.Extensions.Logging;
    
namespaceAspNet5Localization
{
    publicclassLanguageActionFilter : ActionFilterAttribute
    {
        privatereadonlyILogger _logger;
 
        publicLanguageActionFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("LanguageActionFilter");
        }
 
        publicoverridevoidOnActionExecuting(ActionExecutingContext context)
        {         
            stringculture = context.RouteData.Values["culture"].ToString();
            _logger.LogInformation($"Setting the culture from the URL: {culture}");
 
#if NET451
            System.Threading.Thread.CurrentThread.CurrentCulture = newCultureInfo(culture);
            System.Threading.Thread.CurrentThread.CurrentUICulture = newCultureInfo(culture);
#elif NET46
            System.Threading.Thread.CurrentThread.CurrentCulture = newCultureInfo(culture);
            System.Threading.Thread.CurrentThread.CurrentUICulture = newCultureInfo(culture);
#else
            CultureInfo.CurrentCulture = newCultureInfo(culture);
            CultureInfo.CurrentUICulture = newCultureInfo(culture);
#endif
            base.OnActionExecuting(context);
        }
    }
}
```

The culture value is defined as route data and this is then used to set the culture.

The action filter is then registered in the Startup ConfigureServices method.

```csharp
publicvoidConfigureServices(IServiceCollection services)
{
    services.AddLocalization(options => options.ResourcesPath = "Resources");
 
    services.AddMvc()
        .AddViewLocalization()
        .AddDataAnnotationsLocalization();
 
    services.AddScoped<LanguageActionFilter>();
}
```

This can then be used in a controller using an attribute routing parameter and applying the action filter to the controller class.

```csharp
usingMicrosoft.AspNetCore.Mvc;
usingMicrosoft.Extensions.Localization;
 
namespaceAspNet5Localization.Controllers
{
    [ServiceFilter(typeof(LanguageActionFilter))]
    [Route("api/{culture}/[controller]")]
    publicclassAboutWithCultureInRouteController : Controller
    {
        // http://localhost:5000/api/it-CH/AboutWithCultureInRoute
        // http://localhost:5000/api/fr-CH/AboutWithCultureInRoute
        privatereadonlyIStringLocalizer<SharedResource> _localizer;
 
 
        publicAboutWithCultureInRouteController(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }
 
        [HttpGet]
        publicstringGet()
        {
            return_localizer["Name"];
        }
    }
}
```

This can then be used as follows:

```xml
http://localhost:5000/api/it-CH/AboutWithCultureInRoute
 
http://localhost:5000/api/fr-CH/AboutWithCultureInRoute
```

This is very useful if you cannot rely on the browser culture.

**Notes**

Localization in ASP.NET Core RC2 dotnet supports most of the requirements for implementing localization in a web application. It is flexible and can be extended as required.

**Links:**

[Microsoft Docs: Globalization and localization](https://docs.asp.net/en/latest/fundamentals/localization.html)

[https://github.com/aspnet/Localization](https://github.com/aspnet/Localization)

[https://github.com/aspnet/Tooling/issues/236](https://github.com/aspnet/Tooling/issues/236)

[http://www.jerriepelser.com/blog/how-aspnet5-determines-culture-info-for-localization](http://www.jerriepelser.com/blog/how-aspnet5-determines-culture-info-for-localization)

[http://www.vodovnik.com/2015/09/20/localization-in-asp-net-5-mvc-6/](http://www.vodovnik.com/2015/09/20/localization-in-asp-net-5-mvc-6/)

[https://github.com/WeebDo/WeebDo.CMF](https://github.com/WeebDo/WeebDo.CMF)

[https://github.com/joeaudette/cloudscribe](https://github.com/joeaudette/cloudscribe)

[https://github.com/aspnet/Mvc/tree/dev/test/WebSites/LocalizationWebSite](https://github.com/aspnet/Mvc/tree/dev/test/WebSites/LocalizationWebSite)

[Example of localization middleware for culture in route](https://github.com/aspnet/Localization/issues/122)

[http://weblogs.asp.net/jeff/beating-localization-into-submission-on-asp-net-5](http://weblogs.asp.net/jeff/beating-localization-into-submission-on-asp-net-5)

[https://github.com/joeaudette/experiments](https://github.com/joeaudette/experiments)

[https://github.com/avodovnik/Cancel/tree/master/Source/Demo06.ViewLocationExpanders](https://github.com/avodovnik/Cancel/tree/master/Source/Demo06.ViewLocationExpanders)