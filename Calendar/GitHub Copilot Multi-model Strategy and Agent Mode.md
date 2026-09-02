অবশ্যই। আপনার দেওয়া নোট/কোর্স কনটেন্টের মূল বিষয় হলো **GitHub Copilot-এর Agent Mode, Subagents, Multi-Model Strategy, Autopilot এবং Self-Healing Workflow**। নিচে সহজ বাংলায় পুরো বিষয়টির সারাংশ দিলাম।

## 🔹 ১. GitHub Copilot-এর তিনটি Mode

Copilot Chat-এ মূলত তিন ধরনের কাজের ধরন আছে:

- **Ask Mode** → সাধারণ ChatGPT-এর মতো প্রশ্ন করলে উত্তর দেয় এবং আপনার codebase সম্পর্কে উত্তর দিতে পারে।
- **Agent Mode** → নিজে code edit করতে পারে, command চালাতে পারে, test চালাতে পারে, web search করতে পারে ইত্যাদি।
- **Plan Mode** → বড় কাজ শুরু করার আগে বিস্তারিত implementation plan তৈরি করে।

**মূল শিক্ষা:**  
বড় কাজ সরাসরি Agent দিয়ে শুরু না করে আগে Plan → Review → তারপর Agent/Autopilot ব্যবহার করা নিরাপদ।

---

# 🔹 ২. Agent সবসময় সঠিক নয়

Agent autonomous হলেও তার output **অবশ্যই verify করতে হবে**।

উদাহরণে Copilot Pluralsight-এর course URL বের করার জন্য script তৈরি করেছিল। John Papa-এর ক্ষেত্রে ঠিকমতো কাজ করলেও Maaike van Putten-এর ক্ষেত্রে URL slug সম্পর্কে ভুল assumption করেছিল।

👉 তাই:

> **AI-generated code কখনোই blindly trust করা যাবে না।**

Human developer-এর মতোই AI-এর কাজও review এবং test করতে হবে।

---

# 🔹 ৩. Agent Permissions এবং Autopilot

Agent বিভিন্ন permission level-এ কাজ করতে পারে।

### Default Approval

কিছু action automatically approve হবে, কিছু ক্ষেত্রে আপনার confirmation চাইবে।

### Allow All

সব tool/action automatically approve হবে।

### Autopilot

এটি আরও autonomous:

- সব tool call automatically approve
- error হলে নিজে retry
- user input প্রয়োজন হলে নিজেই সিদ্ধান্ত নেওয়ার চেষ্টা
- task complete না হওয়া পর্যন্ত কাজ চালিয়ে যায়

অর্থাৎ:

**Normal Agent**

`Prompt → Act → Observe → Decide → STOP → Ask User`

**Autopilot**

`Prompt → Act → Observe → Decide → Act → Observe → Decide → ...`

⚠️ **সতর্কতা:** Allow All/Autopilot বিপজ্জনক হতে পারে, কারণ agent file modify/delete বা অন্যান্য destructive action করতে পারে।

---

# 🔹 ৪. কেন Subagent দরকার?

বড় project-এ একটি Agent-এর context window দ্রুত বড় হয়ে যায়।

যেমন:

```
Main Agent
   ↓
100 files
   ↓
Many search results
   ↓
Many intermediate decisions
   ↓
Large context
   ↓
Important information gets lost
```

এখানে **Subagent** ব্যবহার করা হয়।

প্রতিটি subagent নিজের আলাদা context window নিয়ে নির্দিষ্ট কাজ করবে।

উদাহরণ:

```
                Main Agent
                    │
        ┌───────────┼───────────┐
        ↓           ↓           ↓
   UI Agent    Auth Agent   Security Agent
        │           │           │
      HTML/CSS   Authentication  Security
```

এর ফলে:

- Context ছোট থাকে
- Token কম লাগে
- কাজ parallel করা যায়
- Agent-এর focus বাড়ে
- Main agent-এর context পরিষ্কার থাকে

---

# 🔹 ৫. Subagent-এর বিভিন্ন Role

- Implementation Agent 
- Verification Subagent
- Research Subagent

কাজ অনুযায়ী subagent আলাদা করা যায়।

### Implementation Agent

নির্দিষ্ট specification থেকে code তৈরি করবে।

উদাহরণ:

```
Create User API
        ↓
Implementation Agent
        ↓
UserController + Service + Repository
```

### Verification Agent

Code ঠিক আছে কিনা পরীক্ষা করবে।

- Test চালাবে
- Output check করবে
- Architecture review করবে
- Bug detect করবে

সাধারণত **code modify না করাই ভালো**।

### Research Agent

শুধু research করবে।

- Documentation পড়বে
- Files পড়বে
- External information সংগ্রহ করবে
- Structured summary দেবে

এর কাজ code পরিবর্তন করা নয়।

---

# 🔹 ৬. Task অনুযায়ী সঠিক Model নির্বাচন

সব কাজের জন্য সবচেয়ে powerful model ব্যবহার করা ভালো strategy নয়।

চারটি প্রশ্নের মাধ্যমে model নির্বাচন করা যায়:

### ১. Visual কিছু আছে?

Screenshot, diagram, UI design আছে?

→ থাকলে **multimodal model**

### ২. কত জায়গায় answer পরিবর্তন হবে?

একটি file → সাধারণ model যথেষ্ট।

অনেক file / interconnected code → **deep reasoning model**

### ৩. কত information একসাথে মনে রাখতে হবে?

কম context → fast model

অনেক related file/context → deep reasoning model

### ৪. ভুল answer-এর cost কত?

যদি ভুল সহজে ধরা পড়ে → cheap/fast model

যদি subtle bug production-এ চলে যেতে পারে → stronger reasoning model

---

# 🔹 ৭. Fast Model বনাম Deep Reasoning Model

একটি গুরুত্বপূর্ণ demonstration ছিল coin problem।

Fast model:

- তুলনামূলকভাবে ভুল answer দিয়েছে
- দীর্ঘ সময় নিয়েছে

Deep reasoning model:

- সঠিক combination পেয়েছে
- অনেক দ্রুত correct solution দিয়েছে

👉 **Lesson:**

> Fast model সবসময় faster নয় এবং expensive reasoning model সবসময় wasteful নয়।

কাজের complexity অনুযায়ী model নির্বাচন করতে হবে।

---

# 🔹 ৮. Auto Model Selection

Copilot-এর **Auto** mode নিজেই model নির্বাচন করে।

এটি মূলত দুটি বিষয় বিবেচনা করে:

```
Task Complexity
       +
Real-time Model Availability/Health
       ↓
   Best Model
```

Auto:

- Task অনুযায়ী model নির্বাচন করে
- System health বিবেচনা করে
- Organization policy মেনে চলে
- Plan/data-residency restrictions মেনে চলে
- Cache boundary-তে routing করে

তাই সাধারণ daily development-এর জন্য **Auto একটি ভালো default** হতে পারে।

---

# 🔹 ৯. কখন Auto Override করবেন?

সবসময় Auto ব্যবহার করার দরকার নেই।

Manual model নির্বাচন করুন যখন:

### Complex task আগে থেকেই জানা

আপনি জানেন পাঁচ লাইনের code-এর মধ্যে race condition আছে।

### Reproducibility দরকার

একই prompt দিয়ে benchmark/test করতে হলে।

### Compliance requirement আছে

Organization নির্দিষ্ট model ব্যবহার করতে বললে।

### নির্দিষ্ট premium model দরকার

Auto যে model-এ route করবে না, এমন model ইচ্ছাকৃতভাবে ব্যবহার করতে চাইলে।

আর গুরুত্বপূর্ণ বিষয়:

> **একটি conversation মাঝপথে model পরিবর্তন না করাই ভালো।**

এতে context/cache পুনর্গঠন হতে পারে এবং cost বাড়তে পারে।

---

# 🔹 ১০. Multi-Model + Subagent = আরও ভালো Architecture

এখন সবচেয়ে গুরুত্বপূর্ণ ধারণা:

**একটি task-এর জন্য একটি model ব্যবহার করার দরকার নেই।**

বরং:

```
              Orchestrator
          Powerful Reasoning Model
                    │
       ┌────────────┼────────────┐
       ↓            ↓            ↓
   Test Agent   Coding Agent  Verification
     Cheap         Cheap        Medium
```

### Orchestrator

- Task ভাঙবে
- Subagent নির্বাচন করবে
- কাজ coordinate করবে
- Result combine করবে

→ এখানে **strong reasoning model** ভালো।

### Implementation Agent

- Code লিখবে
- বেশি output generate করবে

→ **fast + reasonably capable model**

### Verification Agent

- Code/test carefully check করবে

→ **balanced model**

---

# 🔹 ১১. Custom Subagent

GitHub Copilot-এ reusable custom agent তৈরি করা যায়।

উদাহরণ:

```
.github/
   agents/
      create-data-model.agent.md
```

এর মধ্যে define করা যায়:

- Agent name
- Description
- Argument hint
- Allowed tools
- Instructions
- Specific model

যেমন:

```
create-data-model
        ↓
Read specification
        ↓
Create model class
        ↓
Edit files
```

এখানে শুধু প্রয়োজনীয় tools দেওয়াই ভালো।

**Principle: Least privilege**

যে agent-এর web access দরকার নেই তাকে web access দেওয়ার দরকার নেই।

---

# 🔹 ১২. Self-Healing Loop

Autopilot-এর একটি গুরুত্বপূর্ণ behavior হলো **self-healing**।

এটি আলাদা কোনো magical error-recovery system নয়।

বাস্তবে:

```
Run command
     ↓
Error
     ↓
Observe error
     ↓
Reason about error
     ↓
Fix
     ↓
Run again
     ↓
New error?
     ↓
Fix again
```

উদাহরণে agent:

- Python command ঠিক করেছে
- Missing pytest install করেছে
- Missing httpx install করেছে
- Logic error ঠিক করেছে
- Lint error ঠিক করেছে
- Unused import remove করেছে
- Script permission issue ঠিক করেছে

শেষে test pass করেছে।

---

# 🔹 ১৩. Self-Healing-এর সবচেয়ে বড় সমস্যা: Spiral

Agent কখনো কখনো loop-এর মধ্যে আটকে যেতে পারে।

Signs:

```
Command A
   ↓
Same Error
   ↓
Command A
   ↓
Same Error
   ↓
Edit
   ↓
Undo Edit
   ↓
Edit Again
```

এতে:

- কাজ শেষ হয় না
- Token/credit খরচ বাড়ে
- সময় নষ্ট হয়

### কী করবেন?

**১. Ctrl+C / Stop**

**২. Maximum continuation limit দিন**

**৩. AI credit limit দিন**

**৪. Prompt precise করুন**

খারাপ:

> Make it better.

ভালো:

> Make the test suite pass without changing any assertions.

**৫. Out-of-scope clearly define করুন।**

**৬. বড় কাজ হলে আগে Plan করুন।**

---

# 🔹 ১৪. সবচেয়ে ভালো Autonomous Workflow

পুরো course-এর ধারণাগুলো একত্র করলে workflow দাঁড়ায়:

```
              REQUIREMENT
                   ↓
              PLAN MODE
                   ↓
          Human reviews plan
                   ↓
             ORCHESTRATOR
                   ↓
       ┌───────────┼───────────┐
       ↓           ↓           ↓
     Tests      Backend       Frontend
   Subagent    Subagent      Subagent
       ↓           ↓           ↓
       └───────────┼───────────┘
                   ↓
             Verification
                   ↓
              Run Tests
                   ↓
              Fix Errors
                   ↓
          Verification Again
                   ↓
              Git Commit
                   ↓
             Human Review
```

---

# 🎯 একজন .NET Senior Developer হিসেবে আপনার জন্য মূল শিক্ষা

আপনার মতো বড় .NET project-এ এই approach-টি এভাবে ব্যবহার করা যায়:

```
                    Main Agent
                 Strong Reasoning
                       │
              ┌────────┼────────┐
              ↓        ↓        ↓
          Test Agent  API Agent  DB Agent
              │        │        │
           xUnit    ASP.NET     EF Core
                     Web API    Migration
              │        │        │
              └────────┼────────┘
                       ↓
                Verification Agent
                       ↓
                 Build + Test
                       ↓
                Code Review
```

### মনে রাখার মতো ৭টি principle

1. **Small task → Fast model**
2. **Complex reasoning → Strong model**
3. **Large task → Subagents**
4. **Different task → Different model**
5. **Verification আলাদা agent দিয়ে করুন**
6. **Autopilot-এর আগে precise exit condition দিন**
7. **AI autonomous হলেও Human-in-the-loop অবশ্যই রাখুন**

সবচেয়ে গুরুত্বপূর্ণ takeaway হলো:

> **AI agent-কে শুধু “code writer” হিসেবে নয়, বরং একজন orchestrator হিসেবে ব্যবহার করুন—যে বড় কাজকে ছোট, focused এবং independently verifiable কাজে ভাগ করে বিভিন্ন উপযুক্ত model/subagent-এর কাছে পাঠায়।**



Implement the "create user" flow from specs/users.md. First, use a
subagent to create unit tests for all specifications. Then run
another subagent to implement any failing tests. Run these two
subagents in a loop until all tests succeed and all code is covered.

![[GitHub Copilot Multi-model Strategy and Agent Mode.png]]