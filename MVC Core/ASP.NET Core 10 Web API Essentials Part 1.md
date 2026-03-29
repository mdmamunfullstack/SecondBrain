# ASP.NET Core 10 Web API: Advanced Querying & Application Architecture

## Overview

This project demonstrates building a robust ASP.NET Core Web API with support for:

- **CRUD operations**
- **Advanced resource querying**: Filtering, Searching, and Paging
- **Pagination Metadata**
- **Service Layer (Business Logic Layer)**
- **Repository Pattern**
- **AutoMapper for DTO/entity mapping**
- **Best practices for maintainability, security, and scalability**

---

## Features

### 1. Filtering

- Filter resource collections by parameters (e.g. city name) using query string.
- **Example:**  
  ```
  GET /api/cities?name=Antwerp
  ```

### 2. Searching

- Search within collections for text across fields like `name` or `description`.
- **Example:**  
  ```
  GET /api/cities?searchQuery=Tower
  ```
- Searches all cities whose `name` or `description` contains the query, case-insensitive.

### 3. Combining Filtering & Searching

- Both can be used together for refined results.
- **Example:**  
  ```
  GET /api/cities?name=New%20York%20City&searchQuery=park
  ```

### 4. Paging (Pagination)

- Paginate large collections via `pageNumber` and `pageSize` query parameters.
- Default values applied; maximum page size enforced to protect performance.
- **Example:**  
  ```
  GET /api/cities?pageNumber=2&pageSize=5
  ```

#### Controller Example

```csharp
[HttpGet]
public async Task<IActionResult> GetCities(
    string? name, 
    string? searchQuery, 
    int pageNumber = 1, 
    int pageSize = 10, 
    CancellationToken cancellationToken = default)
{
    if (pageSize > maxCitiesPageSize) pageSize = maxCitiesPageSize;
    var (cityEntities, paginationMetadata) = await _repository.GetCitiesReadOnlyAsync(
        name, searchQuery, pageNumber, pageSize, cancellationToken
    );
    if (paginationMetadata != null)
        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));
    var results = _mapper.Map<IEnumerable<CityDto>>(cityEntities);
    return Ok(results);
}
```

### 5. Pagination Metadata

- API returns pagination info in an `X-Pagination` response header (**not** in the JSON body).
- Includes total record count, page size, current page, and total page count.

#### Example `X-Pagination` header:

```json
{
  "TotalItemCount": 43,
  "PageSize": 10,
  "CurrentPage": 2,
  "TotalPageCount": 5
}
```

---

## Internal Architecture

### Repository Pattern

- All data access is handled via repository interfaces (e.g. `ICityInfoRepository`) and implementations.
- Keeps controllers “persistence ignorant” and simplifies testing/mocking.

### AutoMapper Integration

- All DTO/entity transformations use [AutoMapper](https://automapper.org/) and explicit mapping profiles.
- Eliminates manual mapping logic.

### Service Layer (Optional)

- Add a service layer (e.g. `IPointOfInterestService`) to encapsulate business logic **when business rules go beyond simple CRUD**.
- Service layer methods can return result objects indicating operation success/failure and details.
- **DO NOT** add a service layer if controller → repository mapping is 1:1 with no meaningful business rules!

---

## Security and Best Practices

- **Deferred Execution:** All filtering, searching, and paging is implemented using `IQueryable<T>`, ensuring efficient SQL queries and no in-memory over-fetching.
- **SQL Injection Safety:** All queries are LINQ-based and parameterized using EF Core, so input is safe by default.
- **Max Page Size:** Imposed via controller logic to prevent abuse.
- **Separation of Concerns:**  
    - Controller: HTTP protocol, input validation  
    - Service: Business logic (when necessary)  
    - Repository: Data access  
    - Mapping: AutoMapper only  
- **Testing:** Easily write unit tests for controllers (by mocking services/repos) and for service/business logic.

---

## Example Models

```csharp
public class PaginationMetadata
{
    public int TotalItemCount { get; }
    public int PageSize { get; }
    public int CurrentPage { get; }
    public int TotalPageCount { get; }

    public PaginationMetadata(int totalItemCount, int pageSize, int currentPage)
    {
        TotalItemCount = totalItemCount;
        PageSize = pageSize;
        CurrentPage = currentPage;
        TotalPageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
    }
}
```

---

## Extending Further

- **Sorting:** Add optional sort parameters to most querying endpoints, applying `OrderBy`/`OrderByDescending` as appropriate.
- **Unit of Work:** Use if needing atomic operations across multiple repositories.
- **Caching, Rate Limiting:** Add for performance and security in production.
- **Comprehensive Error Handling and Logging:** Use middleware for global error handling and structured logging.

---

## When to Introduce a Service Layer

- You have substantial business logic (e.g., enforcing business rules, orchestrating multiple repositories, or sending notifications).
- If all your controller does is call repository methods, **avoid adding the service layer** for no reason – keep things simple!

---

## Conclusion

This project’s advanced querying and architectural layering ensure clean, efficient, and maintainable APIs—ready for production workloads!

For further improvement, see the successor courses covering **security, versioning, documentation, and deployment**.

---

*Happy coding & building awesome APIs!*