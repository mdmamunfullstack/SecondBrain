---
title: Http Client
date:
---

Traditionally, making HTTP calls in .NET applications involved using the `HttpClient` class directly. This often led to issues with managing the lifecycle of the `HttpClient` instances, resulting in socket exhaustion and performance problems.

Here’s an example of the old way of making an HTTP call:

```cs
public async Task<string> GetWeatherDataAsync(string city, string apiKey)  
{  
    using (var httpClient = new HttpClient())  
    {  
        var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}";  
        var response = await httpClient.GetAsync(url);  
          
        if (response.IsSuccessStatusCode)  
        {  
            return await response.Content.ReadAsStringAsync();  
        }  
  
        return null;  
    }  
}

```

When `CreateClient` is called, a new instance of `HttpClient` is created.

### Why Use IHttpClientFactory?

The `IHttpClientFactory` was introduced in .NET Core 2.1 to address several issues with the direct use of `HttpClient`. It provides a centralized way to configure and create `HttpClient` instances, ensuring proper resource management and improved performance.

### Problems Solved by IHttpClientFactory

1. **Socket Exhaustion**: Improper disposal of `HttpClient` instances can lead to socket exhaustion. `IHttpClientFactory` manages the lifetimes of `HttpClientMessageHandler` instances to prevent this.
2. **Centralized Configuration**: It allows centralized configuration of `HttpClient` instances, making it easier to apply consistent settings.
3. **Named and Typed Clients**: Provides a way to create named and typed clients, allowing specific configurations for different uses.

## 3 Ways to Use IHttpClientFactory

There are three possible ways you can use IHttpClientFactory in your .NET project. Let’s see all three in action, one by one.

### #1: Simply inject IHttpClientFactory and use it:

First, register the `IHttpClientFactory` in the `Startup.cs`:

```cs
public void ConfigureServices(IServiceCollection services)  
{  
    services.AddHttpClient();  
}
```

Now, modify the method to use `IHttpClientFactory`:
```cs

public class WeatherService  
{  
    private readonly IHttpClientFactory _httpClientFactory;  
  
    public WeatherService(IHttpClientFactory httpClientFactory)  
    {  
        _httpClientFactory = httpClientFactory;  
    }  
  
    public async Task<string> GetWeatherDataAsync(string city, string apiKey)  
    {  
        var httpClient = _httpClientFactory.CreateClient();  
        var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}";  
        var response = await httpClient.GetAsync(url);  
  
        if (response.IsSuccessStatusCode)  
        {  
            return await response.Content.ReadAsStringAsync();  
        }  
  
        return null;  
    }  
}
```

### #2: IHttpClientFactory using named clients:

Named clients allow you to configure `HttpClient` instances with specific settings for different purposes.



Registering Named Clients:

```cs
public void ConfigureServices(IServiceCollection services)  
{  
    services.AddHttpClient("WeatherClient", client =>  
    {  
        client.BaseAddress = new Uri("https://api.openweathermap.org/");  
        client.DefaultRequestHeaders.Add("Accept", "application/json");  
    });  
}

```
Now modify the `WeatherService` to use the named client instead:

```cs
public class WeatherService  
{  
    private readonly IHttpClientFactory _httpClientFactory;  
  
    public WeatherService(IHttpClientFactory httpClientFactory)  
    {  
        _httpClientFactory = httpClientFactory;  
    }  
  
    public async Task<string> GetWeatherDataAsync(string city, string apiKey)  
    {  
        var httpClient = _httpClientFactory.CreateClient("WeatherClient");  
        var url = $"data/2.5/weather?q={city}&appid={apiKey}";  
        var response = await httpClient.GetAsync(url);  
  
        if (response.IsSuccessStatusCode)  
        {  
            return await response.Content.ReadAsStringAsync();  
        }  
  
        return null;  
    }  
}

```
Notice that we haven’t specify the hostname inside `GetWeatherDataAsync` method this time, because we have already set the base address for this client when registering it in DI.

### #3: IHttpClientFactory using Typed clients:

Typed clients provide a way to encapsulate HTTP logic into a reusable class. Let’s create one and name it `WeatherCleint`

```cs
public class WeatherCleint  
{  
     private readonly HttpClient _httpClient;  
  
     public WeatherCleint(HttpClient httpClient)  
     {  
        _httpClient = httpClient;  
     }  
  
     public async Task<string> GetWeatherDataAsync(string city, string apiKey)  
     {  
          var url = $"data/2.5/weather?q={city}&appid={apiKey}";  
          var response = await _httpClient.GetAsync(url);  
          if (response.IsSuccessStatusCode)  
          {  
              return await response.Content.ReadAsStringAsync();  
          }  
          return null;  
     }  
}
```

Now, let’s register our typed client in DI:

```cs
public void ConfigureServices(IServiceCollection services)  
{  
    services.AddHttpClient<WeatherClient>(client =>  
    {  
        client.BaseAddress = new Uri("https://api.openweathermap.org/");  
        client.DefaultRequestHeaders.Add("Accept", "application/json");  
    });  
}

```
The `AddHttpClient`method registers the `WeatherClient` as `transient`service.

Now modify the `WeatherService` to use the named client instead:

```cs
public class WeatherService  
{  
    private readonly WeatherClient _weatherClient;  
  
    public WeatherService(WeatherClient weatherClient)  
    {  
        _weatherClient = weatherClient;  
    }  
  
    public async Task<string> GetWeatherDataAsync(string city, string apiKey)  
    {  
        return await _weatherClient.GetWeatherDataAsync(city, apiKey);  
    }  
}

```

### Warning:

Avoid injecting typed clients inside singleton services. The `HttpClient` is expected to be short lived. If typed client is captured in singleton, it may not get get disposed appropriately, hence defeating the purpose of `IHttpClientFactory`.

## Closing

Using `IHttpClientFactory` in .NET is a powerful way to manage HTTP client lifetimes, avoid common pitfalls like socket exhaustion, and apply centralized configuration. Whether you use the default client, named clients, or typed clients, `IHttpClientFactory` provides flexibility and improved performance for your applications.

I hope this tutorial has helped you gain a understanding of using IHttpClientFacotry and different ways to implement it. Stay tuned for more articles as we continue to explore the powerful features of .NET 8.0 and beyond.

Happy coding!

![[Pasted image 20260731091727.png]]



# 📌Async এবং Await ব্যবহার করে API Call – বাংলা সারসংক্ষেপ

- ✅ সব API Call `async/await` ব্যবহার করে লিখুন।
- ✅ **Async All the Way** নীতি অনুসরণ করুন।
- ✅ `.Result`, `.Wait()`, `Task.Run()` এড়িয়ে চলুন।
- ✅ প্রতিটি API Method-এ `CancellationToken` ব্যবহার করুন।
- ✅ `CancellationTokenSource` Dispose করতে ভুলবেন না।
- ✅ Typed API Client ব্যবহার করলে Networking Logic সুন্দরভাবে আলাদা থাকে।
- ✅ `EnsureSuccessStatusCode()` দিয়ে সফল Response নিশ্চিত করুন।
- ✅ `await` ব্যবহার করলে Thread Block হয় না এবং UI Responsive থাকে।
- ✅ Timeout ও Cancellation-এর জন্য `OperationCanceledException` Handle করার প্রস্তুতি রাখুন।
- ✅ পরবর্তী ধাপে Custom Error Handling যোগ করলে Client আরও শক্তিশালী ও ব্যবহারবান্ধব হবে।

# 📌 API Error Handling ও Typed Exception – বাংলা সারসংক্ষেপ

- ✅ `EnsureSuccessStatusCode()`-এর পরিবর্তে Custom `EnsureSuccessAsync()` ব্যবহার করুন।
- ✅ API Failure-এর জন্য `IssueApiException` ব্যবহার করুন।
- ✅ Exception-এর মধ্যে Status Code ও Response Body সংরক্ষণ করুন।
- ✅ Exception সাধারণ Control Flow-এর জন্য ব্যবহার করবেন না।
- ✅ Request বা Response Body-তে Sensitive Data (PII) Log করবেন না।
- ✅ API Error (`4xx`, `5xx`) এবং Network Error (`HttpRequestException`) আলাদাভাবে Handle করুন।
- ✅ সব API Method-এ একই Error Handling Pattern অনুসরণ করুন।
- ✅ `404 Not Found` সবসময় Exception হওয়া প্রয়োজন নয়; Business Requirement অনুযায়ী Handle করুন।
- ✅ `ILogger` ব্যবহার করে নিরাপদভাবে Error Log করুন।
- ✅ পরবর্তী ধাপে **Resilience Pipeline** (Retry, Timeout, Circuit Breaker ইত্যাদি) যোগ করলে Application আরও নির্ভরযোগ্য হবে।


# 📌 Resilience Pipeline, Typed HttpClient ও Dependency Injection – বাংলা সারসংক্ষেপ

- ✅ **Resilience Pipeline** সাময়িক (Transient) Error-এ স্বয়ংক্রিয় Retry করে।
- ✅ **500, 408, 429, Timeout এবং `HttpRequestException`** Retry করা হয়।
- ✅ **400 ও 404**-এর মতো Client Error Retry করা হয় না।
- ✅ **POST Request** Retry করার ক্ষেত্রে **Idempotency** নিশ্চিত করা গুরুত্বপূর্ণ।
- ✅ `AddStandardResilienceHandler()` ব্যবহার করে সহজেই Built-in Retry ও Timeout Policy যোগ করা যায়।
- ✅ Retry-এর মাঝে **Exponential Backoff** ব্যবহার করা হয়।
- ✅ Retry, Timeout, Circuit Breaker ও Rate Limiter প্রয়োজন অনুযায়ী Customize করা যায়।
- ✅ `Program.cs`-এ শুধুমাত্র Application Startup রাখা উচিত; Business Logic আলাদা Workflow বা Service Class-এ রাখা ভালো।
- ✅ Constructor Injection ও Dependency Injection ব্যবহার করলে Code আরও **Loose Coupled**, **Testable** এবং **Maintainable** হয়।
- ✅ এই Architecture Production-Ready এবং বিভিন্ন ধরনের .NET Application-এ পুনঃব্যবহারযোগ্য (Reusable)।



https://www.telerik.com/blogs/building-resilient-apis-retry-pattern 



## প্রোডাকশনের জন্য Global Error Handling (বাংলা সারাংশ)

- ✅ সব Exception এক জায়গা থেকে Handle করা হয়।
- ✅ Domain Rule ভাঙলে **400 Bad Request** রিটার্ন হয়।
- ✅ Unexpected Error হলে **500 Internal Server Error** রিটার্ন হয়।
- ✅ **ProblemDetails** ব্যবহার করে standard error response দেওয়া হয়।
- ✅ Internal implementation বা sensitive তথ্য client-এর কাছে leak হয় না।
- ✅ Integration Test-ও নতুন behavior অনুযায়ী আপডেট করা হয়।

**ফলাফল:** API আরও **secure**, **consistent**, **maintainable**, এবং **production-ready** হয়ে যায়।

## Adding structured logging 

- ✅ Structured Logging গুরুত্বপূর্ণ তথ্যকে Property হিসেবে সংরক্ষণ করে।
- ✅ Message Template ব্যবহার করলে Log Search ও Filter করা সহজ হয়।
- ✅ Interpolated String ব্যবহার করলে Structured Data হারিয়ে যায়।
- ✅ JSON Console, Application Insights, Seq ও OpenTelemetry Structured Log ভালোভাবে ব্যবহার করতে পারে।
- ✅ বারবার ব্যবহৃত Log-এর জন্য **LoggerMessage Source Generator** Performance বাড়ায়।
- ✅ সঠিক Log Level ব্যবহার করলে Monitoring ও Troubleshooting আরও কার্যকর হয়।
- ✅ Production-এ Structured Logging ব্যবহার করলে অ্যাপ্লিকেশন পর্যবেক্ষণ, Debugging এবং Performance Analysis অনেক সহজ হয়ে যায়।

## Adding OpenTelemetry tracing 

## Logs, Metrics ও Traces-এর পার্থক্য

- **Logs** → একটি নির্দিষ্ট সময়ে কী ঘটেছে তা জানায়।
- **Metrics** → কতবার বা কত ঘনঘন কিছু ঘটছে তা দেখায়।
- **Traces** → একটি সম্পূর্ণ Request কীভাবে পুরো সিস্টেমের মধ্য দিয়ে চলেছে তা দেখায়।

Resource
[# Serilog in ASP.NET Core .NET 10 - Structured Logging Done Right](https://codewithmukesh.com/blog/structured-logging-with-serilog-in-aspnet-core/ )
[# Logging in ASP.NET Core: Best Practices for API Development)](https://treblle.com/blog/logging-aspnet-core-best-practices) 
 fg bc            
