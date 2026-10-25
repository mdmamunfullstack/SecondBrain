## মূল শিক্ষা

Business Requirements বোঝার সময় এই চারটি প্রশ্ন করুন:

1. কী কী তথ্য রাখতে হবে? — Entities ও Properties।
2. ব্যবহারকারী কী কী কাজ করতে পারবেন? — Use Cases।
3. কোন নিয়মগুলো মানতে হবে? — Business Rules ও Validation।
4. কীভাবে তথ্য দেখানো বা খোঁজা হবে? — Screens, Queries ও Reports।

এই কাজগুলো আগে করলে Domain Model ও Application Core সঠিকভাবে ডিজাইন করা অনেক সহজ হয়। এরপর Entity, Repository Contract এবং Use Case বাস্তবায়ন শুরু করা যায়।


সহজভাবে বললে:

- Domain Project: ব্যবসার মূল ধারণা ও নিয়ম রাখে।
- Application Project: সেই ব্যবসার কাজগুলো কীভাবে সম্পন্ন হবে, তা Use Case আকারে সাজায়।
- Infrastructure Project: Database, EF Core ও অন্যান্য বাইরের প্রযুক্তির Implementation দেয়।
- API Project: HTTP Request গ্রহণ করে Application-এর কাজ চালু করে।

## . Repository Pattern কী?

Repository Pattern হলো Application Code এবং Data Access Implementation-এর মধ্যে একটি মধ্যবর্তী স্তর।

![[Pasted image 20261024203137.png]]

Application জানে যে `IEventRepository` ব্যবহার করতে হবে। কিন্তু ডেটা SQL Server, কোনো External Service, অথবা File থেকে আসছে কি না, সেটি Application-এর জানতে হয় না।

![[Pasted image 20261024203447.png]]

## ৯. সবকিছু একসঙ্গে মনে রাখুন

|ধারণা|কেন ব্যবহার করি?|
|---|---|
|Application Project|Business Use Case সংগঠিত করতে|
|Interface / Contract|Implementation থেকে Application-কে স্বাধীন রাখতে|
|Repository Pattern|Data Access-কে আলাদা রাখতে|
|Generic Repository|সাধারণ Data Access Method পুনর্ব্যবহার করতে|
|Specific Repository|বিশেষ Data Access প্রয়োজন পূরণ করতে|
|Mediator|Request ও Handler-এর মধ্যে সরাসরি নির্ভরতা কমাতে|
|Unit of Work|একাধিক Change একসঙ্গে Commit করতে; এই লেকচারে EF Core `DbContext`-এর সুবিধা ব্যবহার করা হচ্ছে|
|DRY|একই Logic অপ্রয়োজনীয়ভাবে বারবার লেখা এড়াতে|


আপনার জন্য শেখার সঠিক ক্রম: প্রথমে Application Project তৈরি করুন, তারপর `IGenericRepository<T>` ও প্রয়োজনীয় Specific Repository Interface লিখুন। এরপর CQRS Command, Query এবং Handler তৈরি করুন। সবশেষে Infrastructure-এ Repository Implementation ও API-তে Dependency Injection যুক্ত করুন।

এই ক্রম অনুসরণ করলে Domain, Application এবং Infrastructure-এর দায়িত্ব আলাদা রাখা অনেক সহজ হবে।

## ২. MediatR-এর প্রধান দুটি অংশ

### A. Request — কী কাজ করতে হবে?

Request হলো একটি message বা object, যা বলে application-কে কী করতে হবে।

উদাহরণ:

- `GetAllEventsQuery` — সব Event আনতে হবে।
    
- `GetEventByIdQuery` — নির্দিষ্ট Event আনতে হবে।
    
- `CreateEventCommand` — নতুন Event তৈরি করতে হবে।
    
- `UpdateEventCommand` — Event পরিবর্তন করতে হবে।
    

### B. Handler — কাজটি কীভাবে হবে?

Handler হলো সেই class যেখানে নির্দিষ্ট request-এর business logic লেখা থাকে।

উদাহরণ:

- `GetAllEventsQueryHandler`
    
- `GetEventByIdQueryHandler`
    
- `CreateEventCommandHandler`
    

মনে রাখবেন:

Request = কী করতে চাই?

Handler = কাজটি কীভাবে সম্পন্ন হবে?