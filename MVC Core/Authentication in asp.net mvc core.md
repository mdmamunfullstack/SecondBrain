## Authentication & Authorize Middleware 
ASP.NET Core's security model has two distinct layers that work together:

Authentication defines "Who is this user?" It reads incoming requests for credentials (in this case, a cookie), deserializes them into a **ClaimsPrincipal**, and populates **HttpContext.User**. This happens in two parts: first, a cookie authentication handler is registered in the dependency injection container (AddAuthentication + AddCookie); then, the UseAuthentication() middleware is added to the request pipeline to invoke that handler on every request.

Authorization addresses "Is this user allowed to do this?" The UseAuthorization() middleware reads endpoint metadata — attributes like [Authorize] and [AllowAnonymous] — and decides whether to allow, challenge, or forbid the request. Without this middleware, those attributes are inert metadata that the framework never evaluates.

The order of these middleware calls matters. ASP.NET Core processes middleware in the order they are registered. UseAuthentication() must run before UseAuthorization() so that the user's identity is established before access decisions are made. Both must come after UseRouting() (which determines which endpoint will handle the request) and before endpoint mapping (which executes the endpoint).


## Authorize Attribute
With the middleware pipeline in place from Step 1, you can now use attributes to control access at the controller or action level:

The [Authorize] marks a controller or action as requiring an authenticated user. When an unauthenticated request reaches an [Authorize] endpoint, the authorization middleware issues a challenge. The cookie handler converts this into a 302 redirect to the LoginPath you configured in TODO 1.

[AllowAnonymous] explicitly marks a controller or action as accessible without authentication. This is a defensive annotation: even if a global authorization policy is added later, [AllowAnonymous] ensures the endpoint stays public.


## ClaimsPrincipal & HttpContext
Cookie authentication works by serializing a ClaimsPrincipal into an encrypted cookie. On each subsequent request, the UseAuthentication() middleware reads the cookie, decrypts it, and reconstructs the ClaimsPrincipal as HttpContext.User. The sign-in and sign-out flow is managed through two methods:

HttpContext.SignInAsync(scheme, principal)—Serializes the given ClaimsPrincipal into a cookie and includes it in the response's Set-Cookie header. The browser stores this cookie and sends it with all subsequent requests to the same domain.

HttpContext.SignOutAsync(scheme)—Tells the cookie handler to issue a Set-Cookie header that expires the authentication cookie. The browser deletes the cookie, and subsequent requests are treated as unauthenticated.

A ClaimsPrincipal is built from one or more ClaimsIdentity objects, each containing a list of Claim key-value pairs. For this exercise you need only one claim: ClaimTypes.Name, which stores the username. This value becomes accessible as User.Identity.Name in controllers and Razor views




## Conclusion
If you have reached this section, then good job on completing the lab. You have implemented a complete cookie-based authentication and authorization flow in an ASP.NET Core 10 MVC application. The work covered four layers of the security pipeline:

Service registration and middleware: the invisible plumbing that reads cookies and evaluates access rules on every request.
Authorization attributes: declarative access control that protects specific controllers and explicitly keeps others public.
Sign-in and sign-out actions: the code that creates and destroys authenticated sessions by issuing and expiring cookies.
Authentication-aware UI: Razor conditional rendering that reflects the user's login state in the navigation. 


## 1: Register cookie authentication services.

```cs
// TODO 1: Register cookie authentication services.
// Call AddAuthentication with CookieAuthenticationDefaults.AuthenticationScheme,
// then chain AddCookie() and set the LoginPath option to "/Account/Login".

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
      options.LoginPath = "/Account/Login";
      
    });
```


## 2. Register Middleweare
```cs
// TODO 2: Add the authentication middleware.
// Call UseAuthentication() here, after UseRouting() and before endpoint mapping.
app.UseAuthentication();

// TODO 3: Add the authorization middleware.
// Call UseAuthorization() here, immediately after UseAuthentication().
app.UseAuthorization(); 
```



## 3. Add Authorize Attribute 
```cs
// TODO 4: Protect the Projects area.
// Apply the [Authorize] attribute to this controller so only
// authenticated users can access the project list.
[Authorize] 



// TODO 5: Keep the Home page public..
// Apply the [AllowAnonymous] attribute to this controller so it
// remains accessible without authentication.
[AllowAnonymous] 

```



## 4. Account Controller

```cs
public class AccountController : Controller
{
    // TODO 6: Create the Login GET action.
    // Add a Login() GET action that returns a view containing a simple
    // username form (no password required for this exercise).
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }


    // TODO 7: Implement the Login POST action.
    // Add a Login(string username) POST action that:
    //   1. Creates a ClaimsIdentity with at least a ClaimTypes.Name claim.
    //   2. Wraps it in a ClaimsPrincipal.
    //   3. Calls HttpContext.SignInAsync() to issue the authentication cookie.
    //   4. Redirects to the Home page.
      [HttpPost]
    public async Task<IActionResult> Login(string username)
    {
         var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal);
        return RedirectToAction("Index", "Home");
    }

    // TODO 8: Implement the Logout action.
    // Add a Logout() POST action that:
    //   1. Calls HttpContext.SignOutAsync() to clear the authentication cookie.
    //   2. Redirects to the Home page. 

      [HttpPost]
      public async Task<IActionResult> Logout()
      {
          
          await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
          return RedirectToAction("Index", "Home");
      }
}  
```

## 5. UI Changes
```cs
       <ul class="navbar-nav">
          @if (User.Identity.IsAuthenticated)
          {
              <li class="nav-item">
                  <span class="nav-link">Hello, @User.Identity.Name</span>
              </li>
              <li class="nav-item">
                  <form method="post" asp-controller="Account" asp-action="Logout" style="display:inline;">
                      <button type="submit" class="nav-link btn btn-link" style="padding:0;">Logout</button>
                  </form>
              </li>
          }
          else
          {
              <li class="nav-item">
                  <a class="nav-link" asp-controller="Account" asp-action="Login">Login</a>
              </li>
          }
      </ul>
```

#Authentication
#Authorization
