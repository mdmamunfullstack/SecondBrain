# Spec-Driven Development with AI in Practice

## বাংলা সারসংক্ষেপ ও ASP.NET Core Project-এ প্রয়োগ

> **মূল ধারণা:** AI দিয়ে দ্রুত কোড তৈরি করাই Spec-Driven Development
> (SDD)-এর উদ্দেশ্য নয়। বরং AI যেন সঠিক business requirement বুঝে, নির্ধারিত
> সীমার মধ্যে কাজ করে এবং তৈরি করা কোড approved specification অনুযায়ী
> হয়েছে---তা যাচাই করা যায়, সেটিই SDD-এর মূল লক্ষ্য।

এই ডকুমেন্টটি *Spec-driven Development with AI in Practice* শিরোনামের উৎস
ডকুমেন্টের সারসংক্ষেপ। এতে একটি Compliance Obligation Tracker উদাহরণের
মাধ্যমে requirement থেকে specification, planning, implementation, testing
এবং verification পর্যন্ত AI-কে নিয়ন্ত্রিতভাবে ব্যবহার করার পদ্ধতি তুলে ধরা
হয়েছে।

------------------------------------------------------------------------

## 1. কেন Spec-Driven Development প্রয়োজন?

সাধারণ একটি requirement হতে পারে:

-   Compliance obligation track করতে হবে।
-   Deadline কাছাকাছি এলে notify করতে হবে।
-   কোন বিষয়গুলো attention প্রয়োজন তা দেখাতে হবে।

কিন্তু এখানে অনেক অস্পষ্টতা রয়েছে:

-   কোন ধরনের obligation track হবে?
-   Deadline-এর কত দিন আগে warning দেখাবে?
-   Notify বলতে email, notification, নাকি dashboard status বোঝাবে?
-   Weekend বা holiday কীভাবে গণনা হবে?

AI এসব প্রশ্ন না করেই নিজের মতো সিদ্ধান্ত নিয়ে implementation তৈরি করতে
পারে। ফলে কোড দেখতে ঠিক হলেও business requirement ভুলভাবে বাস্তবায়িত হতে
পারে।

**মূল শিক্ষা:** Requirement-এর অস্পষ্টতা AI নিজে পূরণ করতে দেবে না। গুরুত্বপূর্ণ
business decision আগে নির্ধারণ করতে হবে।

## 2. Specification কীভাবে লিখতে হবে?

একটি ভালো specification তৈরির জন্য নিচের বিষয়গুলো বিবেচনা করতে হবে।

  অংশ                           উদ্দেশ্য
  ----------------------------- -----------------------------------
  Scope                         কাজের সীমানা নির্ধারণ
  In Scope                      কী কী implement হবে
  Out of Scope                  কী implement হবে না
  Actors                        কারা feature ব্যবহার করবে
  Inputs                        কী ধরনের data প্রয়োজন
  Outputs                       কী result পাওয়া যাবে
  Assumptions                   কোন বিষয়গুলো ধরে নেওয়া হচ্ছে
  Functional Requirements       System কী করবে
  Edge Cases                    বিশেষ পরিস্থিতিতে কী হবে
  Non-functional Requirements   Quality ও operational constraints

উদাহরণ হিসেবে, "deadline কাছাকাছি হলে warning দেখাবে" না লিখে বলা হয়েছে:

> The system shall expose an obligation as at risk when its effective
> due date is 3 or fewer working days from the current date in
> Europe/Amsterdam time.

এতে threshold, working-day calculation, timezone এবং observable result
পরিষ্কার হয়।

আরেকটি গুরুত্বপূর্ণ বিষয় হলো **Out of Scope**। যেমন automatic email delivery,
recurring obligations এবং role-based administration প্রথম feature-এর
বাইরে রাখা হয়েছে। এতে AI অপ্রয়োজনীয় feature যোগ করতে পারে না।

## 3. EARS-style Acceptance Criteria

শুধু functional requirement থাকলেই যথেষ্ট নয়। Requirement কীভাবে যাচাই করা
হবে, সেটিও স্পষ্ট করতে হবে।

ডকুমেন্টে EARS (Easy Approach to Requirements Syntax) ব্যবহার করা হয়েছে। এর
পাঁচটি pattern:

1.  **Ubiquitous:** System সবসময় কী করবে --- `The system shall...`
2.  **Event-driven:** কোনো event ঘটলে কী হবে --- `When...`
3.  **State-driven:** নির্দিষ্ট state থাকলে কী হবে --- `While...`
4.  **Unwanted behavior:** কোনো invalid condition হলে কী হবে ---
    `If... then...`
5.  **Optional feature:** নির্দিষ্ট feature enabled থাকলে কী হবে ---
    `Where...`

প্রতিটি Acceptance Criteria-তে থাকতে হবে:

-   কখন requirement প্রযোজ্য হবে।
-   কোন condition-এ প্রযোজ্য হবে।
-   System কী observable response দেবে।

প্রতিটি criterion-এর সঙ্গে test outline ও unique ID রাখলে test failure
থেকে সহজে বোঝা যায় কোন requirement ভঙ্গ হয়েছে।

## 4. Over-specification বনাম Under-specification

Specification লেখার সময় দুটি ভুল এড়িয়ে চলতে হবে।

-   **Over-specification:** Class name, loop, database query বা নির্দিষ্ট
    implementation algorithm বাধ্যতামূলক করে দেওয়া।
-   **Under-specification:** Business rule, threshold, timezone,
    expected output বা error behavior অস্পষ্ট রাখা।

সঠিক পদ্ধতি হলো:

**Business behavior ও observable result-এ নির্ভুল হতে হবে, কিন্তু
implementation detail-এ প্রয়োজনীয় স্বাধীনতা রাখতে হবে।**

যেমন ৩ working days-এর threshold নির্ধারণ করা জরুরি; কিন্তু সেটি loop দিয়ে
নাকি database query দিয়ে calculate হবে, তা specification-এ বাধ্যতামূলক
করার প্রয়োজন নেই।

## 5. Specification থেকে Implementation Plan

Approved specification সরাসরি AI-কে দিয়ে implement না করিয়ে প্রথমে একটি
reviewable plan তৈরি করতে হবে।

প্রস্তাবিত workflow:

1.  **Specify:** Approved requirements ও acceptance criteria তৈরি।
2.  **Clarify:** অস্পষ্টতা ও missing business decisions চিহ্নিত করা।
3.  **Plan:** Architecture, dependencies, risks ও affected files
    নির্ধারণ।
4.  **Tasks:** ছোট, independently reviewable implementation tasks তৈরি।
5.  **Implement & Verify:** কোড, টেস্ট এবং specification compliance
    যাচাই।

Plan-এ থাকা উচিত:

-   Architecture ও project structure।
-   কোন component বা file পরিবর্তন হবে।
-   Dependencies ও implementation order।
-   Risk ও unresolved decisions।
-   কোন Acceptance Criteria দিয়ে কাজটি যাচাই হবে।

প্রতিটি task ছোট এবং independently reviewable হওয়া উচিত। একটি task-এর
implementation শেষ হলেই সেটিকে verified বলা যাবে না; verification
evidence প্রয়োজন।

### GitHub Spec Kit ও GitHub Copilot

ডকুমেন্টে GitHub Spec Kit এবং GitHub Copilot ব্যবহার করে workflow দেখানো
হয়েছে। তবে Spec Kit-ই SDD-এর একমাত্র পদ্ধতি নয়। একই workflow plain
Markdown specification এবং ভালোভাবে লেখা AI prompt দিয়েও অনুসরণ করা যায়।

## 6. AI দিয়ে Implementation করার সঠিক পদ্ধতি

AI-কে একবারে পুরো feature implement করতে না দিয়ে নির্দিষ্ট task দিতে হবে।

প্রতিটি implementation prompt-এ উল্লেখ করা উচিত:

-   কোন approved specification পড়তে হবে।
-   কোন task implement করতে হবে।
-   কোন Acceptance Criteria পূরণ করতে হবে।
-   কোন architecture ও coding constraints অনুসরণ করতে হবে।
-   কী কী test তৈরি করতে হবে।
-   কোনো ambiguity থাকলে implementation বন্ধ করে প্রশ্ন করতে হবে।
-   Scope-এর বাইরে কোনো পরিবর্তন করা যাবে না।

**গুরুত্বপূর্ণ:** AI-generated code একটি proposal, approved implementation
নয়।

## 7. Generated Code কীভাবে Verify করবেন?

শুধু build successful বা test passing হলেই implementation সঠিক প্রমাণ হয়
না।

চারটি verification step:

1.  **Diff review:** অপ্রয়োজনীয় পরিবর্তন, scope creep বা invented defaults
    আছে কি না।
2.  **Acceptance Criteria traceability:** প্রতিটি criterion-এর
    implementation ও test evidence আছে কি না।
3.  **Automated testing:** Unit, integration ও focused tests চালানো।
4.  **Runtime verification:** API response এবং বাস্তব observable behavior
    পরীক্ষা করা।

Test-কে শুধু endpoint কাজ করছে কি না তা পরীক্ষা করতে দেওয়া যাবে না;
expected business behavior-ও যাচাই করতে হবে।

## 8. Brownfield Project-এ SDD

আগে থেকে চালু থাকা application-এ নতুন feature যোগ করার সময় শুধু নতুন
requirement লিখলেই হবে না। পুরোনো behavior-ও বুঝতে হবে।

প্রথমে existing application-এর বিষয়গুলো বিশ্লেষণ করতে হবে:

-   API endpoints ও request/response formats।
-   Database ও persistence behavior।
-   Existing tests।
-   Undocumented business rules।
-   Soft-delete behavior।
-   Client compatibility requirements।

এসব বিশ্লেষণ করে `brownfield-context.md`-এ সংরক্ষণ করা যেতে পারে।

এরপর specification-এ দুটি বিষয় রাখতে হবে:

1.  নতুন feature কী করবে।
2.  পুরোনো application-এর কোন behavior অপরিবর্তিত রাখতে হবে।

উদাহরণ: পুরোনো API-তে date format ও error response আদর্শ না হলেও existing
clients সেগুলোর ওপর নির্ভরশীল। তাই নতুন feature যোগ করার সময় সেগুলো পরিবর্তন
না করার সিদ্ধান্ত নেওয়া হতে পারে।

**শিক্ষা:** Existing behavior evidence হিসেবে বিবেচনা করতে হবে, কিন্তু
সেটিকে স্বয়ংক্রিয়ভাবে desired future behavior ধরে নেওয়া যাবে না।

Brownfield validation-এ নতুন feature কাজ করছে কি না, পাশাপাশি পুরোনো
behavior ঠিক আছে কি না---দুটিই পরীক্ষা করতে হবে।

## 9. Requirement পরিবর্তন হলে কী করবেন?

Development চলাকালে requirement পরিবর্তন হওয়া স্বাভাবিক। কিন্তু শুধু Copilot
chat-এ নতুন instruction দিয়ে implementation করানো উচিত নয়।

ডকুমেন্টে একটি escalated obligation-এ `Acknowledged` state যোগ করার উদাহরণ
দেখানো হয়েছে। পরিবর্তনগুলো এই ক্রমে করা হয়েছে:

1.  Decision Log
2.  Specification
3.  Implementation Plan
4.  Tasks
5.  Implementation
6.  Tests
7.  Verification

Decision log-এ শুধু গৃহীত সিদ্ধান্ত নয়, rejected alternatives-ও সংরক্ষণ করা
হয়েছে। এতে ভবিষ্যতে একই প্রশ্ন আবার উঠলে সিদ্ধান্তের কারণ জানা যায়।

## 10. Specification একটি Living Team Artifact

Specification শুধু AI prompt নয়; এটি পুরো দলের জন্য version-controlled
documentation।

একটি ভালো living specification-এর বৈশিষ্ট্য:

-   Git-এ version controlled।
-   Pull Request-এর মাধ্যমে reviewed।
-   Requirement, implementation ও test-এর মধ্যে traceability।
-   Business decision পরিবর্তন হলে update করা।
-   নতুন developer বা AI agent যেন chat history ছাড়াই project বুঝতে পারে।

Product Owner business intent approve করবেন; developer plan ও tasks-এর
দায়িত্ব নেবেন; পুরো দল specification সম্পর্কে অবগত থাকবে।

------------------------------------------------------------------------

## 11. ASP.NET Core Clean Architecture + CQRS Project-এ প্রয়োগ

MVC 5 থেকে ASP.NET Core migration-এর ক্ষেত্রে এই পদ্ধতিটি ব্যবহার করা যেতে
পারে। যেমন Term, Category এবং Attendee feature-গুলো আলাদাভাবে migrate
করা।

  SDD Artifact          Project-এ প্রয়োগ
  --------------------- ------------------------------------------
  Scope                 কোন MVC 5 feature migrate হবে
  Requirements          Existing business rules
  Acceptance Criteria   Expected API behavior
  Brownfield Context    Existing DBHelper, Dapper ও API behavior
  Technical Plan        Clean Architecture layer ও CQRS design
  Tasks                 Feature-wise implementation
  Tests                 Unit, Integration ও Regression tests
  Verification          Existing behavior বনাম migrated behavior

### প্রস্তাবিত documentation structure

``` text
docs/
└── specs/
    ├── shared/
    │   ├── architecture.md
    │   ├── coding-standards.md
    │   └── brownfield-context.md
    │
    └── features/
        ├── term/
        │   ├── requirements.md
        │   ├── acceptance-criteria.md
        │   ├── plan.md
        │   └── tasks.md
        │
        ├── category/
        │   ├── requirements.md
        │   ├── acceptance-criteria.md
        │   ├── plan.md
        │   └── tasks.md
        │
        └── attendee/
            ├── requirements.md
            ├── acceptance-criteria.md
            ├── plan.md
            └── tasks.md
```

প্রতিটি feature-এর জন্য আলাদা specification রাখুন। বিদ্যমান Dapper ও
DBHelper behavior না বুঝে EF Core বা অন্য data-access technology-তে
পরিবর্তন করবেন না।

------------------------------------------------------------------------

## 12. চূড়ান্ত শিক্ষা

ডকুমেন্টটির সবচেয়ে গুরুত্বপূর্ণ পাঁচটি takeaway:

1.  **Define intent before generating code:** আগে requirement ও business
    rules পরিষ্কার করুন।
2.  **Control scope:** AI-কে নিজের মতো feature বা business decision যোগ
    করতে দেবেন না।
3.  **Plan before implementation:** Architecture, dependency ও task আগে
    review করুন।
4.  **Verify against specifications:** Build ও passing tests যথেষ্ট নয়;
    expected behavior যাচাই করুন।
5.  **Keep specifications alive:** Requirement পরিবর্তনের সঙ্গে decision,
    spec, plan, task ও test আপডেট করুন।

> **এক কথায়:** SDD হলো AI-কে বেশি স্বাধীনতা দেওয়া নয়; বরং AI-এর কাজকে
> স্পষ্ট specification, ছোট task, traceability এবং verification-এর মাধ্যমে
> নিয়ন্ত্রণ করা।
