---
title: "Spec-Driven Development with Spec-Kit: Transforming a Legacy ASP.NET Application into a Modern Spring Boot API Using Agentic AI"
source: "https://medium.com/@shoubhikghosh83/spec-driven-development-with-spec-kit-transforming-a-legacy-asp-net-2913746bd9f1"
author:
  - "[[Shoubhik Ghosh]]"
published: 2026-03-11
created: 2026-11-07
description: "Recently, there has been a lot of buzz in the developer community around spec-driven development and tools like Spec-Kit."
tags:
  - "clippings"
---
## Introduction

Recently, there has been a lot of buzz in the developer community around **spec-driven development** and tools like Spec-Kit.

Spec-driven development is an approach where **structured specifications become the central artifact of the development process**. Instead of jumping directly into coding, the workflow starts by clearly defining the functionality in a structured spec. That specification is then used to generate architecture plans, implementation tasks, tests, and eventually the code itself.

This approach is powerful because it brings several benefits:

- **Clarity** — requirements are explicitly defined before development begins
- **Traceability** — specs, plans, and tasks stay connected to the implementation
- **Consistency** — AI agents follow the spec rather than guessing requirements
- **Productivity** — developers spend less time translating requirements into code

With the rise of **AI coding agents**, spec-driven development is becoming even more interesting. Instead of using AI only for small code snippets, tools like Spec-Kit allow developers to use AI across the **entire development lifecycle**.

When I started hearing about Spec-Kit, I became curious. Rather than just reading about it, I decided to try it on a real problem: **modernizing a small legacy ASP.NET application** using a spec-driven, agentic workflow.

In this article, I will walk through the experiment and show how I used Spec-Kit to **reverse engineer the legacy code, extract the business rules, and generate a modern Spring Boot API**.

![](https://miro.medium.com/v2/resize:fit:1100/format:webp/1*lqwRKkBMap0938peUvuT1g.png)

## Usecase Details

For this demo, I took a simple ASP.NET MVC legacy application that calculates insurance premiums. The application captures:  
*Customer Name  
Customer Address  
Insurance Type (Auto, Medical, House)*  
The premium calculation logic is embedded in the controller and applies business rules such as:  
Base premium per insurance type (Auto = 5000, Medical = 7000, House = 10000)  
5% discount for customer names longer than 10 characters  
10% surcharge if the address contains “Metro”  
The legacy code is available on GitHub: [https://github.com/shoubhikghosh83/premium-calculator-legacy](https://github.com/shoubhikghosh83/premium-calculator-ws-speckit/tree/main/premium-calculator-legacy)

Please refer to Github for complete codebase.

[https://github.com/shoubhikghosh83/premium-calculator-ws-speckit/tree/main](https://github.com/shoubhikghosh83/premium-calculator-ws-speckit/tree/main)

## Approach

To explore **spec-driven development**, I followed a structured workflow using Spec-Kit and an AI coding agent.

The idea was to start from a **legacy codebase**, extract its functionality, and gradually move toward a **modern Spring Boot service** through specifications and structured planning.

The overall approach looked like this:

```cs
Legacy ASP.NET Application
            │
            ▼
Reverse Engineering
(Extract Business Rules & Workflow)
            │
            ▼
Business Rule Documentation
(BRE.md)
            │
            ▼
Define Engineering Standards
(speckit.constitution)
            │
            ▼
Create Feature Specification
(speckit.specify → spec.md)
            │
            ▼
Clarify Missing Requirements
(speckit.clarify)
            │
            ▼
Generate Implementation Plan
(speckit.plan → plan.md)
            │
            ▼
Generate Development Tasks
(speckit.tasks → tasks.md)
            │
            ▼
AI-Assisted Implementation
(speckit.implement)
            │
            ▼
Build, Test & Validation
(mvn clean install)
```

This workflow allowed the process to move in a **structured and traceable way**:

1. **Understand the legacy system**
2. **Convert understanding into a specification**
3. **Generate a plan and tasks**
4. **Use AI to implement the modern service**

This is the core idea behind **spec-driven development**.

## Installing Spec-Kit and Preparing the Project Structure

To start the experiment, the first step was to install Spec-Kit and prepare a workspace where both the **legacy application and the modernization artifacts** could coexist.

### Installing Spec-Kit

Spec-Kit uses **uv** for package management. If `uv` is not installed, install it first.

```cs
pip install uv
```

Once `uv` is installed, Spec-Kit can be initialized inside project repository.

```cs
uv tool install specify-cli --from git+https://github.com/github/spec-kit.git
```

This installs the command:

```cs
specify
```
![](https://miro.medium.com/v2/resize:fit:1400/format:webp/1*QF-6rpN62Mf-MhIgMFGDRg.png)

### Defining the Project Structure

For this demo, I wanted to clearly separate three things:

1. **Legacy code**
2. **Specifications and planning artifacts**
3. **Modernized implementation**

So I organized the repository with the following structure:

```cs
project-root
│
├── premium-calculator-legacy
│   └── (ASP.NET WebForms application)
│
├── premium-calculation-modernization
│   └── (Spring Boot implementation generated later)
│
├── specs
│   └── (spec.md, plan.md, tasks.md generated by Spec-Kit)
│
└── .specify
    ├── memory
    ├── scripts
    └── templates
```

## Initialize Speckit

Now initialize spec-kit in the workspace.

```cs
specify init .
```
![](https://miro.medium.com/v2/resize:fit:1400/format:webp/1*Mpcf576ssugU9yXtemqloA.png)

Select the AI assistance. In my case I go with Copilot.

![](https://miro.medium.com/v2/resize:fit:1400/format:webp/1*hA95AwjlVVbPAo7w2iPFHA.png)

![](https://miro.medium.com/v2/resize:fit:1400/format:webp/1*PlsYSxl5woIBhzTXsRgObQ.png)

This creates:

```cs
.specify/
   memory/
   specs/
   tasks/
```

These folders store:

1. memory: project knowledge
2. specs: functional specifications
3. tasks: development tasks

## Generating Business Rule Documentation (BRE)

Once the workspace and folder structure were ready, the next step was to **understand the legacy ASP.NET application and extract its business rules**.

The goal of this step was to create a **Business Rule Extraction document (BRE.md)** that explains how the premium calculation works.

For this Demo I used Intellij IDE. In the IntelliJ Copilot Chat Window, you can see Spec-kit commands already loaded

![](https://miro.medium.com/v2/resize:fit:1100/format:webp/1*LHZ3OFfQMkKlRmKljfxTRw.png)

### Reverse Engineering the Legacy Code

I selected speckit.specify and provided the below prompt

```cs
Feature: Premium Calculator BRE (Business Rule Extraction)

Context:
The folder \`/premium-calculator-legacy\` contains a legacy ASP.NET WebForms application that implements a Premium Calculator.

Goal:
Reverse engineer the legacy application to extract the business rules and functional behavior of the Premium Calculator.

Instructions:
Analyze the ASP.NET codebase located in \`/premium-calculator-legacy\`, including:

* ASPX pages (.aspx)
* Code-behind files (.cs)
* Backend classes and helper utilities
* Any validation or calculation logic related to premium computation

From the analysis, identify and document:

1. **User Workflow**

   * Steps a user follows to calculate a premium
   * Page navigation and user interactions

2. **Input Parameters**

   * Fields entered by the user
   * Data types and validation rules

3. **Business Rules**

   * Conditions affecting premium calculation
   * Discount rules
   * Surcharge rules
   * Any validation logic

4. **Premium Calculation Logic**

   * Base premium rules
   * Mathematical formulas used
   * Order of rule execution

5. **Data Flow**

   * How data moves between:

     * UI (ASPX pages)
     * Code-behind logic
     * Backend services or classes

Output:
Create a documentation file named **\`BRE.md\`** that provides a structured description of the Premium Calculator functionality.

The document should include the following sections:

* Feature Overview
* User Workflow
* Input Parameters
* Business Rules
* Premium Calculation Logic
* Data Flow Architecture
* Identified Dependencies (if any)

The objective of this document is to clearly capture the **Business Rule Engine (BRE)** behavior of the legacy system so it can later be used for modernization into a Spring Boot API.
```
![](https://miro.medium.com/v2/resize:fit:1400/format:webp/1*V2iA1G0wwIwvuWl3BgKXkw.gif)

BRE.md

```cs
Business Rule Extraction (BRE) Document
Premium Calculator Legacy System Analysis
Document Version: 1.0
Analysis Date: March 8, 2026
Source System: Legacy ASP.NET MVC Premium Calculator
Target System: Spring Boot API (Future Modernization)

1. Feature Overview
The Premium Calculator is a web-based insurance application that computes insurance premiums based on:

Insurance Type (Auto, Medical, or House)
Customer Characteristics (Name length, Address location)
The system applies base premiums and conditional business rules (discounts and surcharges) to generate the final premium amount. The application stores all submitted applications in memory for display.

Key Attributes:

Single-page submission form
Immediate premium calculation upon submission
Application persistence within session lifetime
Three insurance types with distinct pricing models
2. User Workflow
Primary User Flow: Calculate and Submit Insurance Premium
Access Application

User navigates to the Insurance Application page (/Insurance/Index)
Page displays an empty form and list of previously submitted applications
Enter Customer Information

User enters Customer Name (text field)
User enters Customer Address (text field)
User selects Insurance Type from dropdown menu (Auto, Medical, or House)
Submit Application

User clicks "Submit" button
Form posts to /Insurance/Submit endpoint with POST method
Validation & Calculation (Backend)

System validates all three fields are provided
If validation fails: user is redirected to form page with error message
If validation passes: system calculates premium based on business rules
System creates application record with calculated premium
View Results

User is redirected to Insurance Application page
New application appears in the submitted applications table
Table displays: ID, Customer Name, Address, Insurance Type, Premium, Creation Date
User can submit additional applications by repeating steps 2-5
Error Handling Path
If any required field is missing:

Error message: "All fields are required."
User is redirected to form (form data is lost)
If insurance type is invalid (edge case):

Error message: "Invalid insurance type."
User is redirected to form
3. Input Parameters
Parameter Field Name Data Type Required Validation Rules UI Component
Customer Name customerName String Yes Non-empty, no length limit enforced at input Text input field
Customer Address customerAddress String Yes Non-empty, no length limit enforced at input Text input field
Insurance Type insuranceType String (Enum) Yes Must be one of: "Auto Insurance", "Medical Insurance", "House Insurance" Dropdown select
Notes:

Frontend provides dropdown (no free-text entry for insurance type)
Validation is server-side only (no client-side validation)
All parameters are passed via form POST with application/x-www-form-urlencoded content type
Parameter names are case-sensitive in the controller action
4. Business Rules
4.1 Base Premium by Insurance Type
The system assigns a base premium based on the selected insurance type before applying any modifiers.

Insurance Type Base Premium Business Rationale
Auto Insurance ₹5,000 Standard automotive coverage base rate
Medical Insurance ₹7,000 Higher base due to unpredictable medical costs
House Insurance ₹10,000 Highest base reflecting property value protection
Rule ID: BR-001
Execution Order: Step 1 (Applied first, before modifiers)
Implementation: Direct assignment in if-else chain

4.2 Customer Name Length Discount
Rule ID: BR-002
Condition: customerName.Length > 10
Action: Apply 5% discount on the current premium
Formula: Premium = Premium - (Premium × 0.05)
Business Rationale: Names longer than 10 characters receive a premium discount (possibly loyalty or bulk-customer incentive)
Execution Order: Step 2 (Applied after base premium, before location surcharge)

Examples:

Base premium: ₹5,000 | Name: "Christopher" (11 chars) → Discount applied: ₹5,000 - ₹250 = ₹4,750
Base premium: ₹5,000 | Name: "John Smith" (10 chars, excluding space) → No discount (not > 10)
Base premium: ₹5,000 | Name: "Robert James" (12 chars) → Discount applied: ₹5,000 - ₹250 = ₹4,750
4.3 Metro Location Surcharge
Rule ID: BR-003
Condition: customerAddress.Contains("Metro")
Action: Apply 10% surcharge on the current premium
Formula: Premium = Premium + (Premium × 0.10)
Business Rationale: Metropolitan locations incur higher premiums due to increased risk factors (traffic density, crime rates, etc.)
Execution Order: Step 3 (Applied after name discount)
Case Sensitivity: Case-sensitive string matching (only "Metro" with capital M matches)

Examples:

Current premium: ₹4,750 | Address: "123 Metro Center, City" → Surcharge applied: ₹4,750 + ₹475 = ₹5,225
Current premium: ₹5,000 | Address: "123 metro center, city" (lowercase) → No surcharge
Current premium: ₹5,000 | Address: "456 Metropolitan Avenue" → No surcharge (contains "Metropol" not "Metro" as substring)
Current premium: ₹5,000 | Address: "Metro Station, Downtown" → Surcharge applied: ₹5,000 + ₹500 = ₹5,500
4.4 Rule Combination Logic
Multiple Rules Can Apply: Both BR-002 and BR-003 can apply to the same application.

Execution Sequence:

Assign base premium (BR-001)
Apply name discount if applicable (BR-002)
Apply metro surcharge if applicable (BR-003)
Modifier Interaction: Each rule applies to the already-modified premium from previous rules (compounding effect).

Example Calculation Chain:

Customer: "Christopher Washington" (21 chars)
Address: "456 Metro Street, Mumbai"
Type: "Auto Insurance"

Step 1: Base Premium = ₹5,000 (Auto Insurance)
Step 2: Name > 10? Yes (21 chars) → Premium = ₹5,000 - ₹250 = ₹4,750
Step 3: Address contains "Metro"? Yes → Premium = ₹4,750 + ₹475 = ₹5,225

Final Premium: ₹5,225
5. Premium Calculation Logic
5.1 Calculation Algorithm
FUNCTION CalculatePremium(insuranceType, customerName, customerAddress)
    
    // Step 1: Determine Base Premium
    IF insuranceType == "Auto Insurance" THEN
        basePremium = 5000
    ELSE IF insuranceType == "Medical Insurance" THEN
        basePremium = 7000
    ELSE IF insuranceType == "House Insurance" THEN
        basePremium = 10000
    ELSE
        RAISE ERROR "Invalid insurance type"
    END IF
    
    currentPremium = basePremium
    
    // Step 2: Apply Name Length Discount
    IF LENGTH(customerName) > 10 THEN
        discount = currentPremium * 0.05
        currentPremium = currentPremium - discount
    END IF
    
    // Step 3: Apply Metro Location Surcharge
    IF customerAddress CONTAINS "Metro" THEN
        surcharge = currentPremium * 0.10
        currentPremium = currentPremium + surcharge
    END IF
    
    RETURN currentPremium
    
END FUNCTION
5.2 Mathematical Formulas
Name Discount Formula:

DiscountedPremium = BasePremium × (1 - 0.05)
                  = BasePremium × 0.95
Metro Surcharge Formula:

SurchargedPremium = PreviousPremium × (1 + 0.10)
                  = PreviousPremium × 1.10
Combined Formula (when both apply):

FinalPremium = BasePremium × 0.95 × 1.10
             = BasePremium × 1.045
5.3 Premium Range by Type
Insurance Type Min Premium Max Premium Notes
Auto ₹4,750 ₹5,225 Base ₹5,000: -5% to +10.5%
Medical ₹6,650 ₹7,315 Base ₹7,000: -5% to +10.5%
House ₹9,500 ₹10,450 Base ₹10,000: -5% to +10.5%
6. Data Flow Architecture
6.1 Request Flow Diagram
┌─────────────────┐
│   User Browser  │
└────────┬────────┘
         │ 1. GET /Insurance/Index
         ▼
┌─────────────────────────────────┐
│  InsuranceController.Index()    │
│  - Returns view with list       │
│  - Populates InsuranceTypes     │
└────────┬────────────────────────┘
         │ 2. Renders Index.cshtml
         ▼
┌─────────────────────────────────┐
│   Views/Insurance/Index.cshtml  │
│  - Form with 3 input fields     │
│  - Displays submitted apps      │
└─────────────────────────────────┘
         │ 3. User fills form & clicks Submit
         │    POST /Insurance/Submit
         ▼
┌──────────────────────────────────┐
│ InsuranceController.Submit()     │
│  - Validates inputs             │
│  - Calculates premium (BR-001-3)│
│  - Creates InsuranceApplication │
│  - Stores in _applications list │
└────────┬─────────────────────────┘
         │ 4. Redirect to Index
         ▼
┌──────────────────────────────────┐
│ InsuranceController.Index()      │
│  - Returns updated list          │
└────────┬─────────────────────────┘
         │ 5. Renders updated view
         ▼
┌─────────────────┐
│   User Browser  │
│ (sees new app)  │
└─────────────────┘
6.2 Data Structure Flow
HTML Form Input
    │
    ├─ customerName: string
    ├─ customerAddress: string
    └─ insuranceType: string
    
    │ (HTTP POST)
    ▼
    
InsuranceController.Submit(string, string, string)
    │
    ├─ [Validation Layer]
    │   └─ Check: All fields non-empty
    │
    ├─ [Business Logic Layer]
    │   ├─ Base Premium Assignment (BR-001)
    │   ├─ Name Discount Application (BR-002)
    │   └─ Metro Surcharge Application (BR-003)
    │
    ├─ [Model Creation]
    │   └─ new InsuranceApplication {
    │       Id: int,
    │       CustomerName: string,
    │       CustomerAddress: string,
    │       InsuranceType: string,
    │       Premium: decimal,
    │       CreatedDate: DateTime
    │     }
    │
    └─ [Storage Layer]
        └─ _applications.Add(application)
        └─ Return RedirectToAction("Index")
        
Index() retrieves _applications and passes to view
    │
    ▼
View iterates InsuranceApplication models
    │
    └─ Renders HTML table with all applications
6.3 Data Persistence Model
Current Implementation:

In-memory static list: static List<InsuranceApplication> _applications
Thread-unsafe storage (no locking mechanism)
Data persists only during application lifetime
Data is lost when application restarts or recycles
Limitations:

Single-instance only (not suitable for scaled deployment)
No database backing
No user session isolation
All users see all applications
7. Key Entities
7.1 InsuranceApplication Model
Entity Name: InsuranceApplication
Storage Location: Models/InsuranceApplication.cs
Primary Key: Id (integer, auto-increment via _idCounter)

Property Type Nullable Notes
Id int No Auto-generated, starts at 1, increments per submission
CustomerName string No (validated) User-provided, no length constraint
CustomerAddress string No (validated) User-provided, contains location data for surcharge logic
InsuranceType string No (validated) One of three values: "Auto Insurance", "Medical Insurance", "House Insurance"
Premium decimal No Calculated value, result of business rules BR-001 through BR-003
CreatedDate DateTime No System timestamp, set to DateTime.Now at submission time
Relationships: None (standalone entity)

8. Identified Dependencies & Constraints
8.1 Technical Dependencies
Dependency Version Usage Criticality
ASP.NET MVC (unspecified) Web framework for controller/view pattern Critical
.NET Framework (unspecified) Runtime and base libraries (System, System.Web.Mvc) Critical
Linq (unspecified) List operations (implicitly used) Medium
8.2 Business Logic Constraints
Constraint Impact Notes
Base premiums are fixed values Premium calculation must use ₹5000/7000/10000 Hard-coded, no configuration available
Discount rate is fixed at 5% Name length discount cannot be parameterized Hard-coded in BR-002
Surcharge rate is fixed at 10% Metro location surcharge cannot be parameterized Hard-coded in BR-003
Name length threshold is 10 characters Discount boundary is fixed Hard-coded, no flexibility
Metro location is exact substring match Location matching is case-sensitive Hard-coded, no fuzzy matching
Insurance types are fixed set Only 3 types supported; no extensibility Hard-coded in controller and view
8.3 System Constraints & Risks
Risk Severity Description
Data Loss on Restart High In-memory storage with no persistence layer means all data is lost when application restarts
Thread Safety High Static list with no synchronization mechanisms; concurrent requests may cause data corruption
No Authentication High Any user can view all submitted applications; no user isolation or access control
Limited Scalability High In-memory storage unsuitable for production or multi-instance deployment
No Audit Trail Medium No logging of calculation steps or rule application for debugging/compliance
Hardcoded Business Rules Medium Rules embedded in code; changing rules requires code changes and redeployment
Input Validation Gaps Medium No validation of name/address length; no regex validation for address format
Silent Failures Low Empty form redirects without clear error messaging; user may not understand what went wrong
9. Calculation Examples & Test Cases
Example 1: Auto Insurance - No Modifiers
Input:

Customer Name: "John"
Customer Address: "123 Elm Street"
Insurance Type: "Auto Insurance"
Calculation:

Base Premium (Auto) = ₹5,000
Name Length = 4 chars → Not > 10 → No discount
Address contains "Metro"? No → No surcharge
Output: Premium = ₹5,000

Example 2: Medical Insurance - With Name Discount
Input:

Customer Name: "Christopher"
Customer Address: "456 Oak Avenue"
Insurance Type: "Medical Insurance"
Calculation:

Base Premium (Medical) = ₹7,000
Name Length = 11 chars → > 10 → Apply 5% discount
Discount amount = ₹7,000 × 0.05 = ₹350
Premium after discount = ₹7,000 - ₹350 = ₹6,650
Address contains "Metro"? No → No surcharge
Output: Premium = ₹6,650

Example 3: House Insurance - With Metro Surcharge
Input:

Customer Name: "Alex"
Customer Address: "789 Metro Center, Mumbai"
Insurance Type: "House Insurance"
Calculation:

Base Premium (House) = ₹10,000
Name Length = 4 chars → Not > 10 → No discount
Address contains "Metro"? Yes → Apply 10% surcharge
Surcharge amount = ₹10,000 × 0.10 = ₹1,000
Premium after surcharge = ₹10,000 + ₹1,000 = ₹11,000
Output: Premium = ₹11,000

Example 4: Auto Insurance - With Both Modifiers
Input:

Customer Name: "Alexandra Thompson"
Customer Address: "321 Metro Boulevard, Downtown"
Insurance Type: "Auto Insurance"
Calculation:

Base Premium (Auto) = ₹5,000
Name Length = 17 chars → > 10 → Apply 5% discount
Discount amount = ₹5,000 × 0.05 = ₹250
Premium after discount = ₹5,000 - ₹250 = ₹4,750
Address contains "Metro"? Yes → Apply 10% surcharge
Surcharge amount = ₹4,750 × 0.10 = ₹475
Premium after surcharge = ₹4,750 + ₹475 = ₹5,225
Output: Premium = ₹5,225

Example 5: Edge Case - Exact Name Length of 10
Input:

Customer Name: "Robert Lee" (exactly 10 characters, excluding space)
Customer Address: "555 Main St"
Insurance Type: "Medical Insurance"
Calculation:

Base Premium (Medical) = ₹7,000
Name Length = 10 chars → Not > 10 (condition is strictly >) → No discount
Address contains "Metro"? No → No surcharge
Output: Premium = ₹7,000

10. Assumptions & Design Decisions
Assumption Rationale Impact
Premium amounts are in Indian Rupees (₹) Legacy system appears to use currency symbol and rates consistent with INR Price display and calculations assume INR denomination
Discount is calculated on base premium first, then surcharge applies Order of operations follows code sequence: discount then surcharge Modifiers compound correctly
Name length includes all characters except spaces (standard string.Length behavior in C#) C# string.Length counts Unicode characters Character boundary at exactly 10 chars (> 10 means 11+)
"Metro" substring match is case-sensitive C#'s string.Contains() is case-sensitive by default Address must contain capital "M" for surcharge to apply
Premium is calculated in decimal with no rounding Money operations use C# decimal type (128-bit precision) No precision loss or rounding errors expected
One application per form submission Single submit action processes one application Batch submissions not supported
Invalid insurance type check is defensive System validates against only three known types Prevents erroneous premium calculations
All users are treated equally No role-based access control or user differentiation All users see all applications
11. Comparison to Business Requirements
System Capabilities:

✅ Calculates premiums based on insurance type
✅ Applies discounts based on customer characteristics
✅ Applies surcharges based on location
✅ Displays calculation results
✅ Maintains audit trail of submissions
System Limitations:

❌ No user authentication or authorization
❌ No data persistence across application restarts
❌ No configuration management for business rules
❌ No detailed calculation breakdown for users
❌ No ability to modify or cancel applications
12. Modernization Readiness Assessment
For Spring Boot API Migration:
Extracted Business Rules (Ready for Implementation):

BR-001: Base Premium Assignment ✅
BR-002: Name Length Discount (5%, threshold > 10) ✅
BR-003: Metro Location Surcharge (10%, case-sensitive) ✅
Required Enhancements:

Database persistence (replace in-memory storage)
Authentication & Authorization layer
Input validation framework
Business rule engine abstraction
API request/response models
Error handling and logging
Unit tests for calculation logic
Configuration management for rates and thresholds
Critical Findings for Migration:

Business rules are straightforward and clearly implemented
No complex state management or workflows
Suitable for RESTful API exposure
Recommend separating business logic into service layer (separate from API layer)
Document Summary
This BRE document comprehensively captures the legacy Premium Calculator's business logic, enabling a modernized Spring Boot API implementation that preserves all existing rules and behaviors while improving architecture, scalability, and maintainability.

Key Deliverables:

✅ User workflow documented
✅ Input parameters and validation rules identified
✅ All business rules (BR-001, BR-002, BR-003) explicitly documented
✅ Premium calculation logic with examples
✅ Data flow architecture described
✅ Key entities and relationships identified
✅ Dependencies and constraints catalogued
✅ Test cases provided for validation
```

## Defining the Engineering Constitution

At this stage, the **Business Rule Extraction (BRE)** was complete and I had a clear understanding of how the premium calculation works in the legacy system.

The next step in the spec-driven workflow was to define the **engineering guidelines** that the AI agent should follow while generating the modern implementation.

In Spec-Kit, this is done using the **constitution file**. The constitution acts as a **set of engineering principles and project standards** that guide how the system should be implemented.

### Creating the Constitution File

I selected the `**/speckit.constitution**` command and provided a prompt describing the project guidelines.

The goal of this step was to define:

- the **target technology stack**
- the **project structure**
- coding and testing standards
- architectural guidelines for the generated code
```cs
Project: Premium Calculation Modernization

Context:
The repository contains a legacy ASP.NET WebForms application used for reference and analysis.

A new modern backend service will be implemented separately without modifying the legacy system.

All new source code, project artifacts, and implementation files MUST be created inside the directory:

/premium-calculation-modernization

The constitution defines the engineering standards, architecture principles, and technical structure for the modernization project.

---

1. Repository Structure

All generated code must be placed under the following project directory:

premium-calculation-modernization

The project must follow the standard Maven + Spring Boot layout:

premium-calculation-modernization/

src/
src/main/
src/test/

src/main/java/
src/main/resources/

Application source code must be placed inside:

src/main/java

Configuration files must be placed inside:

src/main/resources

Test code must be placed inside:

src/test/java

---

2. Package Organization

The Java source structure should follow a layered architecture.

Recommended package structure:

controller
service
repository
entity
dto
config
exception

Example structure:

src/main/java/com/example/premiumcalculator/

controller
service
repository
entity
dto
config
exception

---

3. Architecture Principles

The system must follow a layered architecture pattern:

Controller → Service → Repository → Database

Responsibilities:

Controller
Handles HTTP requests and responses only.

Service
Contains application logic and orchestration.

Repository
Handles persistence using Spring Data JPA.

Entity
Represents domain models mapped to database tables.

Controllers must remain thin and must not contain business logic.

---

4. Technology Stack

Backend Framework: Spring Boot 3
Programming Language: Java 22
Build Tool: Maven

Persistence:

Spring Data JPA

Database:

H2 (for development and demonstration)

API Style:

RESTful APIs
JSON request and response format

---

5. API Design Standards

All APIs must follow REST conventions.

Rules:

* Use proper HTTP methods (GET, POST, PUT, DELETE)
* Use JSON request and response bodies
* Use clear endpoint naming

Example endpoint style:

/api/premium
/api/applications

Controllers must only manage request/response handling.

---

6. Testing Standards

All business logic must be tested.

Testing structure:

src/test/java

Test types:

Unit Tests
Integration Tests

Requirements:

* Service layer must have unit tests.
* API endpoints must have integration tests.

Target test coverage:

Minimum 80% for service layer logic.

---

7. Security Guidelines

All API inputs must be validated.

Requirements:

* Validate request payloads
* Prevent SQL injection through parameterized queries
* Handle errors safely
* Avoid logging sensitive information

---

8. Performance Requirements

The system must maintain efficient API response times.

Targets:

API response time < 500 ms
Lightweight service execution for core operations.

---

Output

Generate a project constitution that defines:

* architecture principles
* engineering standards
* project structure
* development practices

Ensure that all future generated artifacts and code follow the defined project structure under:

/premium-calculation-modernization
```
![](https://miro.medium.com/v2/resize:fit:1400/format:webp/1*sfJLeyxjGKkVZHmDtNt7bA.gif)

constitution.md file will be generated under.specify/memory folder.

## Creating the Feature Specification

With the **BRE completed** and the **engineering constitution defined**, the next step in the workflow was to generate the **feature specification**.

In Spec-Kit, the specification is the central artifact that describes what the system should do in a structured way. This specification later drives the implementation plan, development tasks, and code generation.

### Generating the Specification

Using IntelliJ IDEA with the AI agent, I selected the `**/speckit.specify**` command.

Since the BRE document already contained the extracted business logic, the goal here was to **convert that understanding into a formal feature specification**.

Prompt used:

```cs
Feature: Auto Insurance Premium Calculation

Context:
The legacy Premium Calculator system supports multiple insurance types.
For the first modernization step, only the Auto insurance premium calculation capability should be specified.

Goal:
Create a feature specification for calculating **Auto insurance premiums**.

User Story:

As a customer
I want to submit my name, address, and insurance type
So that the system can calculate my Auto insurance premium.

Scope:

Include:

* Auto insurance premium calculation
* Base premium rule for Auto insurance
* Name length discount rule (5% discount if name length > 10)
* Metro address surcharge rule (10% surcharge if address contains "Metro")
* Input validation for required fields

Exclude:

* Medical insurance premium calculation
* House insurance premium calculation
* Application persistence
* Application retrieval or listing

Acceptance Scenarios:

1. Base premium calculation for Auto insurance
2. Premium calculation with name discount
3. Premium calculation with metro surcharge
4. Premium calculation with both modifiers
5. Validation errors when required fields are missing

Requirements:

The system must:

* Accept customerName, customerAddress, and insuranceType as inputs
* Validate that all fields are provided
* Verify insuranceType is "Auto"
* Assign base premium of ₹5,000
* Apply 5% discount when name length > 10
* Apply 10% surcharge when address contains "Metro"
* Apply rules sequentially: base → discount → surcharge
* Return the calculated premium

Output:

Generate a **feature specification (\`spec.md\`)** under the \`specs/\` directory describing the Auto Insurance Premium Calculation capability.
```

After executing the command, Spec-Kit generated a `**spec.md**` file inside the `specs` directory.

```cs
specs/
 └── 02-auto-insurance-calculation
     └── spec.md
```

This document contained:

- the **feature overview**
- **user stories**
- **functional requirements**
- **input parameters**
- **expected system behavior**

For this demo, the specification focused on implementing the **Auto Insurance Premium Calculation API**, which takes customer details and calculates the premium based on predefined business rules.

![](https://miro.medium.com/v2/resize:fit:1400/format:webp/1*_YrTW1B1b8byYAE6-nyN_Q.gif)

## User Stories Generated in the Specification

**User Story 1 — Calculate Premium for Auto Insurance**  
The system should calculate the premium for Auto insurance based on customer details and predefined base premium values.

**User Story 2 — Apply Discount Based on Customer Name Length**  
If the customer name exceeds a defined length, the system should apply a percentage discount to the calculated premium.

**User Story 3 — Apply Location-Based Surcharge**  
If the customer address contains certain keywords (for example *Metro*), the system should apply an additional surcharge to the premium.

**User Story 4 — Support Multiple Insurance Types**  
The system should support predefined insurance types such as Auto, House, and Medical, each having its own base premium.

**User Story 5 — Validate Input Parameters**  
The system should validate incoming request parameters such as customer name, address, and insurance type before performing the calculation.

## Clarifying Requirements Before Planning

Even though the specification is ready its important to ensure that the specification does not contain ambiguities or missing details.

Spec-Kit provides a step called **clarification**, where the AI agent reviews the generated specification and asks questions if something is unclear.

### Running the Clarification Step

Using IntelliJ IDEA, I executed the command: /speckit.clarify

Prompt used

```cs
Context:
The feature specification is located at:

specs/02-auto-insurance-calculation/spec.md

Goal:
Review the specification and identify any ambiguities, missing requirements, or assumptions before implementation planning.

Check for:
- unclear business rules
- missing input validation requirements
- incomplete API contract
- unclear calculation logic
- missing edge cases
- missing error handling
- missing response structure

Instructions:
Ask clarification questions where the specification is incomplete or ambiguous.
Focus on questions that would affect implementation.

Output:
List clarification questions grouped by:
- API design
- Business rules
- Validation
- Edge cases
- Data model
```
![](https://miro.medium.com/v2/resize:fit:1400/format:webp/1*BV9Fi9HneHiUFxDBNi35fg.gif)

It generates the following questions

Q1: API Contract Details What should the API endpoint, request/response formats, and HTTP status codes be? The current spec is silent on these implementation-critical details.  
Q2: Whitespace Validation Should names/addresses containing only whitespace characters be considered valid or invalid? This affects input sanitization logic.  
Q3: Character Restrictions What characters are allowed in customer names and addresses? Should special characters, numbers, or non-ASCII characters be permitted?  
Q4: Maximum Input Lengths Are there maximum length limits for customer names and addresses? Without limits, very long inputs could cause performance issues.  
Q5: Error Response Structure What should the error response format be? Should it include error codes, field names, or just plain text messages?  
Q6: Insurance Type Case Sensitivity Is insurance type validation case-sensitive? Can “AUTO”, “auto”, or “Auto Insurance” be accepted, or must it be exactly “Auto”?  
Q7: Multiple “Metro” Handling If an address contains “Metro” multiple times, should the surcharge be applied once or multiple times?

I amended a section in spec.md to clarify these questions

```cs
Clarifications
API Contract
Endpoint: POST /api/v1/premium/calculate

Request:

name
address
insuranceType
Response:

premium
Validation Rules
name must not be empty or whitespace
address must not be empty or whitespace
name max length: 100
address max length: 250
insuranceType max length: 20
Insurance Type Handling
Insurance type comparison is case-insensitive.

Surcharge Rule
If address contains the word "Metro", a surcharge of ₹500 is applied once.

Error Response Format
Errors should follow this structure:

{ "error": "VALIDATION_ERROR", "message": "...", "field": "..." }
```

## Generating the Implementation Plan

Once the clarification step was completed and all open questions were addressed, the specification was ready to move to the next stage: **implementation planning**.

In Spec-Kit, the planning step converts the functional specification into a **technical implementation plan**. This plan acts as the blueprint for how the system will be designed and built.

### Running the Plan Command

Using IntelliJ IDEA, I executed speckit.plan

## Get Shoubhik Ghosh’s stories in your inbox

Join Medium for free to get updates from this writer.

Prompt Used

```cs
Context:
The feature specification is located at:

specs/02-auto-insurance-calculation/spec.md

The specification has already been clarified and contains the API contract,
validation rules, and business logic.

Goal:
Generate a technical implementation plan for building the Auto Insurance Premium Calculation API.

Technology Stack:
- Java
- Spring Boot
- REST API architecture
- JUnit for testing

Project Location:
The implementation must be created under the folder:

premium-calculation-modernization

Required Project Structure:

premium-calculation-modernization/
 └── src
     ├── main
     │   ├── java
     │   │   └── com.insurance.premium
     │   │        ├── controller
     │   │        ├── service
     │   │        ├── rules
     │   │        ├── model
     │   │        └── exception
     │   └── resources
     └── test

Plan Requirements:
The plan should include:

1. Architecture overview
2. REST API design
3. Package structure
4. Controller responsibilities
5. Service layer design
6. Business rule implementation (base premium, discounts, surcharge)
7. Data models (request/response DTOs)
8. Validation strategy
9. Error handling strategy
10. Unit testing strategy

Output:
Generate plan.md inside:

specs/02-auto-insurance-calculation/
```

At this stage, the AI agent analyzes several inputs:

- the **specification (**`**spec.md**`**)**
- the **clarified requirements**
- the **engineering constitution**
- the **target technology stack**

Based on this information, it generates a structured `**plan.md**` file inside the same specification folder.

```cs
specs/
 └── 02-auto-insurance-calculation
     ├── spec.md
     ├── plan.md
```
![](https://miro.medium.com/v2/resize:fit:1400/format:webp/1*OMPDVu5krTJsR3in499e3Q.gif)

## What the Plan Contains

The generated **plan.md** document describes the **technical approach** for implementing the feature. It typically includes:

- **System architecture**
- **Technology stack**
- **API design approach**
- **project structure**
- **testing strategy**

## Generating Development Tasks

With the implementation plan ready, the next step was to break the work into **actionable development tasks**.

In Spec-Kit, this is done using the **task generation step**, which converts the architectural plan into **concrete development activities**.

### Running the Task Generation Command

Using IntelliJ IDEA, I executed speckit.tasks

Prompt Used

```cs
Generate implementation tasks from the plan located in:

specs/02-auto-insurance-calculation/plan.md

Tasks should cover:
- Spring Boot project setup
- Controller implementation
- Validation logic
- Premium calculation service
- Exception handling
- Unit tests

Tasks should be broken into small developer tasks with clear file targets.

Output the tasks file under:

specs/02-auto-insurance-calculation/tasks.md
```

The AI agent analyzed the following artifacts:

- **spec.md** — functional requirements and user stories
- **plan.md** — architecture and implementation strategy
- **constitution** — engineering standards and project structure

Based on these inputs, it generated a `**tasks.md**` file.

```cs
specs/
 └── 02-auto-insurance-calculation
     ├── spec.md
     ├── plan.md
     └── tasks.md
```
![](https://miro.medium.com/v2/resize:fit:1400/format:webp/1*4oNAckaK1BtT2cxNFPRRIw.gif)

The **tasks.md** file contains a detailed breakdown of all development work required to implement the feature.

The file looks as below

![](https://miro.medium.com/v2/resize:fit:1400/format:webp/1*zLWICQ9d_ulCDv2qVnNEIA.png)

![](https://miro.medium.com/v2/resize:fit:1400/format:webp/1*2LChE8R9AGxetIDhkUl34g.png)

## Implementing the Code from Tasks

With the tasks generated, the final step was to **implement the actual code**.

In Spec-Kit, this is typically done using the **implementation step**, where the AI agent reads the generated tasks and starts building the system.

However, in this case, the **task list contained 38 tasks**. Generating the entire implementation in one go is usually **not a good idea**. Large code generations can introduce errors, make debugging difficult, and reduce control over the implementation process.

Instead, I decided to implement the system **phase by phase**.

**Iterative Implementation Approach**

Rather than executing all tasks at once, I divided the implementation into **multiple smaller cycles**. In total, the development was completed in **seven implementation cycles**.

Each cycle followed the same pattern:

1. Select a group of tasks from `tasks.md`
2. Run the implementation command
3. Review the generated code
4. run the build and tests
5. fix issues if needed

The command invoked during each cycle was: speckit.implement

Prompt for each Phase

Phase 1

```cs
Implement Phase 1 tasks (T001–T005) from:

specs/02-auto-insurance-calculation/tasks.md

Tasks:
T001 - Create Maven Project Structure
T002 - Create pom.xml
T003 - Create Spring Boot Application entry point
T004 - Create application.properties
T005 - Create PremiumConstants class

Project root must be:

premium-calculation-modernization/

Follow the constitution guidelines.

Do NOT implement tasks beyond T005.
```

Phase 2

```cs
Implement Phase 2 tasks (T006–T011) from:

specs/02-auto-insurance-calculation/tasks.md

Create DTO and model classes only.

Do NOT implement business rules or services yet.
```

Phase 3

```cs
Implement Phase 3 tasks (T012–T015).

Create the rule engine components:

PremiumRule interface
NameDiscountRule
MetroSurchargeRule
PremiumCalculator

Follow clean architecture and dependency injection.
```

Phase 4

```cs
Implement Phase 4 tasks (T016–T020).

Create validators and exceptions.
Integrate with the existing model classes.
```

Phase 5

```cs
Implement Phase 5 tasks (T021–T023).

Create:

PremiumCalculationService
PremiumCalculationServiceImpl
PremiumCalculationController

Expose REST endpoint:

POST /api/v1/premium/calculate
```

Phase 6

```cs
Implement Phase 6 tasks (T024–T031).

Create unit tests for:

Rules
Validators
Service
Controller

Use JUnit 5 and Mockito.
```

Phase 7

```cs
Implement Phase 7 tasks (T032–T038).

Add code coverage, manual testing instructions, README documentation and final build verification.
```
![](https://miro.medium.com/v2/resize:fit:1400/format:webp/1*WExAQsybk7ZYJ-8YNwoHmA.gif)

Gradually, the application started taking shape. The system generated:

- the **Spring Boot project structure**
- **request and response models**
- **REST controllers**
- **service layer**
- **business rule logic**
- **validation components**
- **unit tests**

After every cycle, I validated the build using:

```cs
mvn clean install
```
![](https://miro.medium.com/v2/resize:fit:1324/format:webp/1*dSaCvQyMsImlOjtuTJPSBw.png)

If any compilation issues or test failures appeared, you can use same speckit.implement command to fix the error.

I have found some testcases were failing

![](https://miro.medium.com/v2/resize:fit:1400/format:webp/1*3UgEACTcOXjitBpUkhBpOw.png)

So I invoke command speckit.implement and run the below prompt

```cs
Implement fixes for failing unit tests in the Premium Calculator module.

Context:
Two unit tests are failing during mvn clean install.

Failing tests:
PremiumCalculatorTest.calculatesPremiumWithBothModifiers
expected: 225.00 but was: 475.00

PremiumCalculatorTest.calculatesPremiumWithNoModifiers
expected: 5000.00 but was: 5000

Reference documents:
specs/02-auto-insurance-calculation/spec.md
specs/02-auto-insurance-calculation/plan.md
specs/02-auto-insurance-calculation/tasks.md

Instructions:
1. Analyze the failing tests.
2. Identify whether the issue is in:
   - PremiumCalculator
   - NameDiscountRule
   - MetroSurchargeRule
3. Correct the implementation so that the tests pass.
4. Preserve the rule sequence defined in the plan.
5. Ensure BigDecimal values use proper scale (2 decimal places).

Do not modify test expectations unless they contradict the specification.
```

Agent was able to find the issues and fix those. It generates Summary file of the changes.

![](https://miro.medium.com/v2/resize:fit:1400/format:webp/1*cMQ7lcfSA8GskIN-p7EMDQ.png)

![](https://miro.medium.com/v2/resize:fit:1400/format:webp/1*mXEssGNi5e8cB3BJBmH60w.png)

![](https://miro.medium.com/v2/resize:fit:1400/format:webp/1*cmJmaMIibrQ-f7NaAS_64A.png)

## Running the Application

After completing the implementation cycles and resolving build issues, the next step was to **run the generated application and verify the output**.

I verified one of the API via Postman and it run successfully

![](https://miro.medium.com/v2/resize:fit:1400/format:webp/1*NJWujrUckpPmJM1lC-hnMA.png)

Lets check the quality of the code.

**Code Quality & Coverage**: I checked against Sonar and report looks great with ≥ 80% Test coverage (As mentioned in constitution file).

![](https://miro.medium.com/v2/resize:fit:1400/format:webp/1*LZ98YuNBkNzpRoldsnFhnA.png)

**Code Review:** I’ll break the review into **Architecture, Code Quality, Testing, API Design, and Production Readiness**.

### Architecture Review ⭐⭐⭐⭐⭐

The project structure follows **clean Spring Boot layering**, which is excellent.

Typical structure:

```cs
controller
service
validator
model
entity
dto
exception
util
```

👍 Good signs:

Proper separation of concerns  
Clear package organization  
Controller does not contain heavy logic  
Business rules moved to service

### Code Quality Review ⭐⭐⭐⭐☆

The code is readable and structured but has **typical AI-generated code smells**.

**Good**  
Classes are small  
Naming mostly clear  
Logic easy to follow  
Validation separated

**Issues**

**Too many utility-style methods  
**Some logic appears inside utility classes where it should belong to the domain service.

**Over-defensive validations  
**AI tends to generate excess validation layers.

**Domain model not strong enough  
**Entities are data containers rather than domain objects.

**API Design Review** ⭐⭐⭐⭐☆

The API Endpoint looks good.  
API Structure is well formated.

Improvements possible:

Add following endpoints for observability

```cs
/health
/version
/actuator
```

**Testing Review** ⭐⭐⭐⭐☆

AI generated

Unit tests  
Coverage rules  
Maven build validation

This is excellent.  
It enforced 80% coverage, which is a strong engineering practice and followed the guidelines specified in constitution file.  
However:  
Tests are mostly happy path.

Missing:  
boundary tests  
invalid characters  
whitespace inputs  
API integration tests

> **Many of these code quality improvements discussed, could be addressed by investing more effort in the constitution file. In a spec-driven workflow using Spec-Kit, the constitution acts as the engineering guideline for AI agents, defining coding standards, architecture patterns, validation rules, and testing expectations. The more detailed and precise these guidelines are, the higher the quality and consistency of the generated code will be.**

From an engineering perspective, the POC turned out to be quite promising. Starting from a legacy ASP.NET codebase and gradually moving through BRE extraction, specifications, planning, and task-driven implementation demonstrated how a **structured, spec-driven workflow can guide AI agents effectively**. The most impressive part was how the process enforced **clarity before coding**, allowing the agent to understand requirements, generate architecture, and then produce working code in phases. While manual review and refinement are still necessary, the approach clearly shows how such tools can **accelerate modernization efforts and improve developer productivity when used with proper guidelines**.

One important realization from the demo was around **productivity uplift**

![](https://miro.medium.com/v2/resize:fit:1400/format:webp/1*Yuciz8FcIZg7Wyqa3Ott-Q.png)

## Conclusion

This experiment showed that Spec-Kit is a powerful approach for bringing **structure and discipline into AI-assisted software development**. The spec → plan → tasks → implement workflow helps engineers guide AI in a much more controlled and traceable way compared to simple prompt-based coding.

However, the process also highlighted that **close human review is essential at every step**. Specifications, plans, and generated code all need validation and refinement from the developer to ensure correctness and good engineering practices.

In practice, the most effective model is a **one-to-one collaboration between the developer and the AI agent**, where the agent accelerates analysis, documentation, and code generation while the engineer provides direction, validation, and architectural judgment. When used this way, Spec-Kit can become a strong productivity tool rather than a fully automated development solution.