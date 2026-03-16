
 we follow a three-step process: **Abstract, Implement, and Inject.**

```cs
public interface IUserRepository  
{  
	List<User> GetAll();  
} 

class UserRepository : IUserRepository  
{  
	public List<User> GetAll()  
	{  
		// Imagine database logic here  
		return new List<User>();  
	}  
} 

class UserService  
{  
	private readonly IUserRepository _repo;  
	  
	// The "Injection" happens here  
	public UserService(IUserRepository repo)  
	{  
	_repo = repo;  
	}  
	  
	public List<User> GetUsers() => _repo.GetAll();  
}
```


 Service Class - It's the place where "Business Logic" lives.

Now:
- Service does NOT create repository
- Service depends on abstraction
- Someone else supplies the dependency


## What is meant by ‘Service depends on abstraction’

We mean:

> _The service does NOT depend on a concrete class.  
>  It depends on an interface (a contract). 


##  Dependency Inversion Principle (DIP)

DIP states:

> _High-level modules should not depend on low-level modules.  
> Both should depend on abstractions._

Here:

- `UserService` = high-level (business logic)
- `UserRepository` = low-level (data access)

## 3️⃣ How Does It Resolve UserService?

You registered it earlier:

```cs 
builder.Services.AddScoped<UserService>();  
builder.Services.AddScoped<IUserRepository, UserRepository>();
```

Now the container knows:

- If someone asks for `UserService`, create one.
- If someone asks for `IUserRepository`, give `UserRepository`.

So the container builds the object graph.


