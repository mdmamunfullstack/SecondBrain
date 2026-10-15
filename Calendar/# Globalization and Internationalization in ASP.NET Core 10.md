![[Localization.png]]


> Course summary and practical reference based on the supplied **Globalization and Internationalization in ASP.NET Core 10** course PDF.

  

## 1. What this course is about


The course explains how to build ASP.NET Core applications that can support users from different languages, regions, and locales.

  

The main areas covered are:
- Globalization (G11n)
- Internationalization (i18n)
- Localization (L10n)
- Translation (t9n)
- Cultures and locales
- `CurrentCulture` vs `CurrentUICulture`
- Request localization middleware
- Culture providers
- `.resx` resource files
- Fallback cultures
- Number, date, time, and currency formatting
- `IStringLocalizer`
- `IHtmlLocalizer`
- Custom localizers
- MVC view localization
- Data Annotation validation localization
- ASP.NET Core built-in validation messages
- Custom validation localization
- Blazor Server localization
- Blazor WebAssembly localization

  

---
## 2. Globalization, Internationalization, Localization and Translation

### Globalization — G11n

Globalization is the overall process of making a website or product suitable for a global audience through internationalization, localization, and translation.
### Internationalization — i18n

Internationalization is the strategy and technical preparation that makes an application capable of being localized.

  

Examples from the course:


- Use Unicode instead of ASCII.
- Design UI layouts that can handle short and long translations.
- Support left-to-right (LTR) and right-to-left (RTL) text.
- Prepare for regional differences such as dates, numbers, and currencies.

  

### Localization — L10n

Localization adapts the application to a specific target market or locale.

It can include:

- Language
- Regional conventions
- Currency
- Date/time formats
- Number formats
- Local requirements and rules

  

### Translation — t9n

Translation means converting text from one language to another.

Translation is only one part of localization. A translated application can still have problems if its UI cannot handle longer text or if dates, currencies, and regional conventions are incorrect.

  

### Easy mental model

  

```text

G11n — Go global

  |

  +-- i18n — Make the application adaptable

  |

  +-- L10n — Adapt it for a specific locale

  |

  +-- t9n — Translate the text

```

  

---

  

## 3. Language, Region and Locale

  

A locale combines language and region.

Example:

```text

en-US

│  │

│  └── Region: United States

└───── Language: English

```

Therefore:

```text

Locale = Language + Region

```

  

Examples:
- `en-US` — English, United States
- `en-GB` — English, Great Britain
- `de-DE` — German, Germany

  

The course explains that a language-only culture can sometimes be sufficient, but region information becomes important when regional units, currencies, dates, or other conventions differ.

  

---

  

## 4. Culture in ASP.NET Core

  

ASP.NET Core distinguishes between two important culture concepts.

### `CurrentCulture`

  

Controls formatting such as:

  

- Dates
- Times
- Numbers
- Currency
Example:
```text

1,250.50

```
vs.

```text

1.250,50

```

  

The appropriate format depends on the current culture.

  
### `CurrentUICulture`


Controls which localized UI resources are selected.

  

For example:

  

```text

CurrentUICulture = en

    -> Welcome

  

CurrentUICulture = de

    -> German translation

  

CurrentUICulture = fr

    -> French translation

```

  

### Remember

  

```text

CurrentCulture

    -> How data is formatted

  

CurrentUICulture

    -> Which UI translation is selected

```

  

---

  

## 5. Request Localization Middleware

  

ASP.NET Core uses request localization middleware to determine the culture for an HTTP request.

  

Main method:

  

```csharp

app.UseRequestLocalization(...);

```

  

The middleware uses information from the request, such as:

  

- Query string

- Culture cookie

- `Accept-Language` HTTP header

- Custom providers

  

A default culture is used when a supported culture cannot be determined.

  

### Basic configuration

  

```csharp

builder.Services.AddLocalization(options =>

{

    options.ResourcesPath = "Resources";

});

  

var supportedCultures = new[]

{

    "en",

    "de",

    "es",

    "fr"

};

  

var localizationOptions = new RequestLocalizationOptions()

    .SetDefaultCulture("en")

    .AddSupportedCultures(supportedCultures)

    .AddSupportedUICultures(supportedCultures);

  

app.UseRequestLocalization(localizationOptions);

```

  

### Middleware order matters

  

`UseRequestLocalization()` must run before middleware that needs the culture.

  

The course specifically highlights routing, MVC, and Razor Pages as middleware/components that must come after request localization.

  

---

  

## 6. Culture Provider Precedence

  

The course demonstrates these providers in order:

  

```text

1. QueryStringRequestCultureProvider

2. CookieRequestCultureProvider

3. AcceptLanguageHeaderRequestCultureProvider

4. DefaultRequestCulture

```

  

### Query string

  

Example:

  

```text

https://example.com/?culture=es

```

  

or:

  

```text

https://example.com/?ui-culture=es

```

  

The query string takes precedence over the cookie and browser language.

  

### Cookie

  

ASP.NET Core can store the user's selected culture in a culture cookie.

  

The course uses:

  

```csharp

CookieRequestCultureProvider.DefaultCookieName

```

  

and:

  

```csharp

CookieRequestCultureProvider.MakeCookieValue(...)

```

  

This allows the application to remember the user's language choice.

  

### Browser `Accept-Language`

  

Browsers can send preferred languages through the HTTP header:

  

```text

Accept-Language: de-DE,de,en-US

```

  

The localization middleware can use this information when higher-priority providers do not provide a supported culture.

  

---

  

## 7. Custom Culture Providers

  

ASP.NET Core allows custom culture-selection logic.

  

The course demonstrates `CustomRequestCultureProvider`.

  

A custom provider can determine the culture from:

  

- Domain name

- URL path

- Route values

- Other request information

- Another application-specific source

  

A custom provider can be registered at a specific position or added first using:

  

```csharp

AddInitialRequestCultureProvider(...)

```

  

This is useful when culture selection needs to follow application-specific rules.

  

---

  

## 8. Troubleshooting Localization

  

The course highlights several common problems.

  

### Check supported cultures

  

The requested culture must be included in:

  

```text

SupportedCultures

SupportedUICultures

```

  

### Always configure a default

  

A default culture provides fallback behavior when the requested culture is unsupported.

  

### Check resource-file names

  

Resource names must match the expected namespace, class, or view conventions.

  

### Check middleware order

  

Localization must run before components that need culture information.

  

### Use diagnostics

  

Localization failures often do not throw exceptions. Instead, the application may simply display the wrong language or resource key.

  

Debug logs can help determine which resources were loaded.

  

---

  

# 9. Resource Files (`.resx`)

  

Resource files contain localized values.

  

They use the `.resx` extension and are XML-based.

  

A common project structure is:

  

```text

MyApp/

├── Resources/

│   ├── SharedResources.resx

│   ├── SharedResources.de.resx

│   ├── SharedResources.es.resx

│   └── SharedResources.fr.resx

└── Program.cs

```

  

The `Resources` folder is a common convention, although the folder name itself is configurable.

  

### Resource entries

  

Example:

  

```text

Name: Welcome

Value: Welcome

```

  

A German resource can contain the German value for the same key.

  

The application code continues to use the key:

  

```csharp

_localizer["Welcome"]

```

  

The selected culture determines which value is returned.

  

---

  

## 10. Resource Naming

  

ASP.NET Core uses naming conventions to associate resources with classes, Razor Pages, or views.

  

For example, a Razor Page associated with `Pages.IndexModel` can have resources such as:

  

```text

Pages.IndexModel.resx

Pages.IndexModel.de.resx

```

  

or an equivalent folder structure under `Resources`.

  

The important lesson is:

  

> Resource-file naming conventions must match the resource/class/view being localized.

  

---

  

# 11. Shared Resources

  

Instead of creating duplicate translations for every page, common strings can be placed in shared resources.

  

Example:

  

```text

SharedResources.resx

SharedResources.de.resx

SharedResources.es.resx

SharedResources.fr.resx

```

  

Create a corresponding class outside the `Resources` folder:

  

```csharp

public class SharedResources

{

}

```

  

Then inject:

  

```csharp

IStringLocalizer<SharedResources>

```

  

This is useful for strings reused across multiple pages.

  

In practice, the course recommends that applications can use a combination of:

  

- Page/view-specific resources

- Shared resources

  

---

  

# 12. `IStringLocalizer`

  

`IStringLocalizer` is used to retrieve localized plain strings.

  

Example:

  

```csharp

@inject IStringLocalizer<SharedResources> Localizer

  

<h1>@Localizer["Welcome"]</h1>

```

  

The localizer looks up the key in the resource associated with the current UI culture.

  

Useful members include:

  

```csharp

Localizer["Key"]

Localizer.GetString(...)

Localizer.GetAllStrings()

```

  

---

  

# 13. Missing Resource Translations

  

An important behavior demonstrated in the course:

  

If a translation is missing, ASP.NET Core can output the resource key instead of throwing an exception.

  

For example:

  

```csharp

Localizer["TicketPurchase"]

```

  

may display:

  

```text

TicketPurchase

```

  

if no translation is found.

  

This is why the course emphasizes:

  

- Translate all strings for supported languages.

- Use fallback cultures where appropriate.

- Choose meaningful resource keys.

  

---

  

# 14. Resource Fallback

  

ASP.NET Core supports fallback when resolving localized resources.

  

The general idea is:

  

```text

de-DE

  ↓

de

  ↓

default resource

```

  

A more specific culture is checked first.

  

If no translation exists for `de-DE`, ASP.NET Core can fall back to `de`.

  

If that also fails, the culture-neutral resource can serve as the ultimate fallback.

  

A common strategy is to make the primary/default language the fallback resource.

  

---

  

# 15. Formatting Dates, Numbers and Currency

  

The current culture affects .NET formatting.

  

Examples include:

  

```csharp

date.ToShortDateString();

  

date.ToLongDateString();

  

number.ToString();

  

amount.ToString("C");

```

  

The culture determines conventions such as:

  

- Decimal point vs decimal comma

- Date format

- Currency symbol

- Currency formatting

  

You can also explicitly specify a culture when you need formatting for a specific culture rather than the current one.

  

---

  

# 16. Localized Strings with Parameters

  

Localized strings often need dynamic values.

  

Resource:

  

```text

Confirmation =

"{0} ticket(s) cost {1} on {2}"

```

  

Code can supply the values:

  

```csharp

Localizer["Confirmation", ticketCount, totalAmount, date]

```

  

This keeps the complete sentence inside the resource file while the application supplies dynamic values.

  

### Why this is important

  

Do not build a sentence in C# and expect localization to translate the already-built sentence.

  

Instead:

  

```text

Resource = complete localized sentence

Code     = supplies variable values

```

  

---

  

# 17. `IHtmlLocalizer`

  

Use `IHtmlLocalizer` when localized resources intentionally contain HTML.

  

Example concept:

  

```html

<em>{0}</em>

```

  

`IStringLocalizer` output is treated as plain text and HTML encoded by Razor.

  

`IHtmlLocalizer` allows localized HTML content to be rendered as HTML.

  

### Security warning

  

`IHtmlLocalizer` does not provide the same HTML encoding behavior.

  

Therefore:

  

> Never allow untrusted user-controlled HTML or script content to be rendered through an HTML localizer.

  

Otherwise, the application may become vulnerable to cross-site scripting (XSS).

  

---

  

# 18. Custom Localizers

  

ASP.NET Core provides interfaces that allow custom localization implementations.

  

For strings:

  

```csharp

IStringLocalizer

```

  

For HTML:

  

```csharp

IHtmlLocalizer

```

  

A custom localizer can retrieve translations from sources other than standard `.resx` resources, such as:

  

- Database

- JSON files

- External systems

  

The course demonstrates a custom `ColorHtmlLocalizer` mainly to show how the mechanism works.

  

---

  

# 19. MVC Localization

  

MVC applications can use:

  

```text

IStringLocalizer

IStringLocalizer<T>

IHtmlLocalizer

IHtmlLocalizer<T>

IViewLocalizer

```

  

### `IViewLocalizer`

  

`IViewLocalizer` is specifically useful for localizing MVC/Razor views.

  

For:

  

```text

Views/Home/Index.cshtml

```

  

the associated resources follow the view-based naming convention, such as:

  

```text

Views.Home.Index.<culture>.resx

```

  

or the corresponding folder structure under `Resources`.

  

MVC setup also requires:

  

```csharp

AddViewLocalization()

```

  

---

  

# 20. Data Annotation Localization

  

Validation messages from Data Annotations can be localized.

  

Enable it with:

  

```csharp

AddDataAnnotationsLocalization()

```

  

For example, a view model might have validation attributes whose error-message values are resource keys.

  

Resource files then contain translations for those keys.

  

### Shared Data Annotation resources

  

If the application uses shared resources instead of model-specific resource files, configure:

  

```text

DataAnnotationLocalizerProvider

```

  

The provider can use:

  

```csharp

IStringLocalizerFactory

```

  

to create a localizer for the shared resource class.

  

---

  

# 21. Built-in ASP.NET Core Validation Messages

  

Not all validation errors come from Data Annotations.

  

Some messages are generated by ASP.NET Core model binding.

  

The course demonstrates using:

  

```text

IStringLocalizerFactory

ModelBindingMessageProvider

```

  

Examples of configuration include:

  

```csharp

SetValueIsInvalidAccessor(...)

SetValueMustNotBeNullAccessor(...)

```

  

The localized message can come from a shared resource key such as:

  

```text

ValueInvalid

```

  

This allows built-in model-binding errors to participate in localization.

  

---

  

# 22. Custom Validation Localization

  

Custom validation attributes need additional integration.

  

The course demonstrates:

  

```text

EvenNumberAttribute

EvenNumberAttributeAdapter

CustomValidationAttributeAdapterProvider

```

  

The custom validation attribute uses a resource key such as:

  

```text

EvenNumberRequired

```

  

The adapter retrieves the translated error message.

  

The provider must then be registered with the application.

  

The key lesson is:

  

> Custom validators can be localized, but they require an adapter/provider integration so the localization system can supply the translated error message.

  

---

  

# 23. Blazor Localization

  

Blazor also supports localization.

  

The general approach remains:

  

```text

Program.cs

   ↓

Configure cultures

   ↓

Configure localization resources

   ↓

Inject IStringLocalizer

   ↓

Use Localizer["Key"]

```

  

The same resource files can be used.

  

---

  

# 24. Blazor Server / Blazor Web App

  

The course's Blazor Server example:

  

1. Configure supported cultures.

2. Configure supported UI cultures.

3. Set the default culture.

4. Configure the resources folder.

5. Use `UseRequestLocalization`.

6. Inject `IStringLocalizer<SharedResources>`.

7. Use localized keys in Razor components.

8. Provide a language picker.

  

For language switching, the course uses JavaScript to perform a full browser redirect and append a query-string culture.

  

The selected culture can then be stored in a culture cookie using `CookieRequestCultureProvider`.

  

---

  

# 25. Blazor WebAssembly

  

Blazor WebAssembly runs in the browser, so culture persistence works differently.

  

The course notes that cookies cannot simply be used in the same way as a server application for remembering the selected language.

  

One possible approach is browser `localStorage` with JavaScript interop.

  

For changing culture while the WebAssembly application is already running, the course highlights:

  

```xml

<BlazorWebAssemblyLoadAllGlobalizationData>true</BlazorWebAssemblyLoadAllGlobalizationData>

```

  

This makes all globalization data available and allows culture changes at runtime.

  

The browser's language preferences can also influence the `Accept-Language` header.

  

---

  

# 26. Blazor Localization Limitations Covered by the Course

  

The course describes Blazor localization support as more limited than MVC/Razor Pages.

  

It specifically notes that:

  

- `IStringLocalizer` works.

- `IStringLocalizer<T>` works.

- Resource files work.

- `IHtmlLocalizer` is not available in the course's Blazor examples.

- `IViewLocalizer` is not available.

- Data Annotation localization is not supported in the course's Blazor examples.

  

Therefore, the course focuses on `IStringLocalizer` for Blazor.

  

---

  

# 27. End-to-End Localization Flow

  

A useful mental model for the entire course:

  

```text

                 HTTP REQUEST

                       │

                       ▼

            Request Localization

                 Middleware

                       │

          ┌────────────┼─────────────┐

          ▼            ▼             ▼

      Query String    Cookie    Accept-Language

          │            │             │

          └────────────┴─────────────┘

                       │

                       ▼

                Selected Culture

                       │

              ┌────────┴────────┐

              ▼                 ▼

       CurrentCulture      CurrentUICulture

              │                 │

              ▼                 ▼

      Date/Number/       Resource Selection

        Currency                 │

                                ▼

                         `.resx` resource

                                │

                                ▼

                       Localized UI string

```

  

---

  

# 28. Recommended Project Structure

  

A practical structure following the course concepts:

  

```text

MyApp/

│

├── Controllers/

├── Models/

├── Views/

│   └── Home/

│       └── Index.cshtml

│

├── Resources/

│   ├── SharedResources.resx

│   ├── SharedResources.de.resx

│   ├── SharedResources.es.resx

│   └── SharedResources.fr.resx

│

├── SharedResources.cs

└── Program.cs

```

  

For a larger application, combine shared resources with view/model-specific resources where appropriate.

  

---

  

# 29. Practical Checklist

  

When adding localization to an ASP.NET Core application:

  

- [ ] Identify supported languages/locales.

- [ ] Decide the default/fallback culture.

- [ ] Add `AddLocalization()`.

- [ ] Configure `ResourcesPath`.

- [ ] Configure `SupportedCultures`.

- [ ] Configure `SupportedUICultures`.

- [ ] Add `UseRequestLocalization()`.

- [ ] Put localization middleware before middleware that needs culture.

- [ ] Create `.resx` resources.

- [ ] Follow resource naming conventions.

- [ ] Inject `IStringLocalizer` or `IStringLocalizer<T>`.

- [ ] Replace hard-coded UI strings with resource keys.

- [ ] Localize strings containing dynamic values with placeholders.

- [ ] Consider shared resources for repeated text.

- [ ] Configure fallback behavior.

- [ ] Verify dates, numbers, and currency formatting.

- [ ] Localize MVC view text with `IViewLocalizer` where appropriate.

- [ ] Add Data Annotation localization for MVC validation.

- [ ] Configure built-in model-binding validation messages if needed.

- [ ] Add adapters/providers for localized custom validators.

- [ ] Be careful with `IHtmlLocalizer` and XSS.

- [ ] For Blazor WebAssembly runtime culture switching, load all globalization data.

- [ ] Test every supported culture.

  

---

  

# 30. Common Mistakes

  

### Mistake 1 — Supporting a culture but forgetting UI culture

  

Configure both:

  

```csharp

.AddSupportedCultures(cultures)

.AddSupportedUICultures(cultures)

```

  

### Mistake 2 — Forgetting the default culture

  

Always provide a sensible fallback/default.

  

### Mistake 3 — Incorrect `.resx` naming

  

Resource files must follow the naming conventions for the class, model, page, or view they serve.

  

### Mistake 4 — Wrong middleware order

  

```csharp

app.UseRequestLocalization(...);

```

  

must execute before components that need the culture.

  

### Mistake 5 — Building localized sentences in C#

  

Instead of:

  

```csharp

$"{count} tickets cost {amount}"

```

  

put the complete localized template in the resource file and pass values as parameters.

  

### Mistake 6 — Treating translation as the whole problem

  

Localization also involves:

  

- Dates

- Numbers

- Currency

- UI layout

- Regional conventions

- Text direction

- Other locale-specific behavior

  

### Mistake 7 — Unsafe HTML localization

  

Do not pass untrusted content through an HTML localizer.

  

---

  

# 31. Key APIs to Remember

  

```text

AddLocalization()

UseRequestLocalization()

  

RequestLocalizationOptions

  

IStringLocalizer

IStringLocalizer<T>

  

IHtmlLocalizer

IHtmlLocalizer<T>

  

IViewLocalizer

  

CookieRequestCultureProvider

QueryStringRequestCultureProvider

AcceptLanguageHeaderRequestCultureProvider

CustomRequestCultureProvider

  

AddViewLocalization()

AddDataAnnotationsLocalization()

  

IStringLocalizerFactory

ModelBindingMessageProvider

  

CustomValidationAttributeAdapterProvider

```

  

---

  

# 32. Final Takeaway

  

The course's central message can be reduced to this:

  

```text

Globalization

      ↓

Internationalization

      ↓

Localization

      ↓

Translation + Regional Formatting

      ↓

A web application that can serve a global audience

```

  

For an ASP.NET Core developer, the core implementation pattern is:

  

```text

1. Configure cultures

2. Configure request localization middleware

3. Create `.resx` resource files

4. Inject a localizer

5. Use resource keys instead of hard-coded UI text

6. Format dates/numbers/currency using culture

7. Localize validation messages

8. Test every supported locale

```

  

The most important distinction to remember is:

  

```text

CurrentCulture   → formatting

CurrentUICulture → UI resources

```

  

And the most important architectural idea is:

  

> **Internationalization prepares the application for different cultures; localization supplies the culture-specific experience.**



<div class="alert alert-info col-12" role="alert">
    Current culture: @CultureInfo.CurrentCulture<br />
    Current UI culture: @CultureInfo.CurrentUICulture
</div> 



var supportedCultures = new[] { "en", "de", "es" };

  

app.UseRequestLocalization(options =>

{

    options.AddSupportedCultures(supportedCultures)

           .AddSupportedUICultures(supportedCultures)

           .SetDefaultCulture("de");

});


builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en", "de", "es", "fr" };

    options.AddSupportedCultures(supportedCultures)
       .AddSupportedUICultures(supportedCultures)
       .SetDefaultCulture("de");
});



builder.Services.AddLocalization(
    options => options.ResourcesPath = "Resources");

   <h1 class="display-4">@Localizer["TicketPurchase"]</h1>

       <button class="btn btn-primary" type="submit">@Localizer["Order"]</button>