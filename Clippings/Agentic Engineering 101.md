---
title: "Agentic Engineering 101"
source: "https://craftbettersoftware.com/p/agentic-engineering-101?utm_campaign=post&utm_medium=web"
author:
  - "[[Daniel Moka]]"
published: 2026-08-12
created: 2026-09-24
description: "Prompt vs Context vs Harness vs Loop vs Graph Engineering"
tags:
  - "clippings"
---
![[The 5 Layers of Agentic Engineering.png]]
# 1. Prompt Engineering: Get One Call Right
![[Prompt Engineering.png]] 
# 2. Context Engineering: Get One Window Right 
![[Context Engineering.png]]
# 3. Harness Engineering: Get One Run Right
![[Harness Engineering.png]]
### Model কী করে?

Model চিন্তা করে:

> “এই সমস্যাটা কীভাবে solve করব?”

### Harness কী করে?

Harness নিশ্চিত করে:

> “Model যেন সঠিক context পায়, প্রয়োজনীয় tools ব্যবহার করতে পারে, নিরাপদে কাজ করে এবং কাজ শেষে result verify হয়।” 


# 4. Loop Engineering: Get One Goal Right 
![[4. Loop Engineering.png]] 
## 🔄 Loop Engineering কী? — সহজ বাংলায়

**Loop Engineering** হলো এমন একটি ব্যবস্থা যেখানে AI agent-কে শুধু একবার prompt দিয়ে থেমে না গিয়ে, **নিজে নিজে বারবার কাজ → যাচাই → ভুল ঠিক করা → আবার যাচাই** করার সুযোগ দেওয়া হয়—যতক্ষণ না নির্দিষ্ট completion condition পূরণ হয়।

সহজভাবে:

> **আপনি যদি প্রতিটি ধাপে AI-কে prompt দেন, তাহলে আপনি নিজেই loop।  
> Loop Engineering-এ সেই loop-টা agent-এর হাতে তুলে দেওয়া হয়।**

এক লাইনে:

> **Loop Engineering = মানুষ যে “prompt → inspect → prompt → inspect → fix” cycle manually করে, সেই cycle-কে একটি controlled, measurable, self-running agent loop-এ পরিণত করা।**

# 5. Graph Engineering: Get the Whole Workflow Right 
![[Graph Engineering.png]] 



Think of your agent as a junior engineer:

- Prompt engineering is **what you tell them.**
- Context engineering is **what you hand them.**
- Harness engineering is **where they work: tools, permissions, tests.**
- Loop engineering is **how they keep going without you.**
- Graph engineering is **which routes their work is allowed to take.**

Or simplier said:

- Prompt is the message.
- Context is the memory.
- Harness is the machine.
- Loop is the system.
- Graph is the map.

**One prompt, one window, one run, one goal, one workflow. Each layer out, you control more of the work.**


## A disciplined workflow might look like this:

- First, understand the problem manually. Clarify the requirements, constraints, and expected behavior.
- Second, decide the design direction. Identify where the change belongs, which components or services are involved, and what trade-offs matter.
- Third, use AI to help with implementation details, alternatives, test cases, documentation, or refactoring.
- Fourth, review the output as if it came from a junior developer who writes quickly but may not understand the entire system. 

For example, a team might define common instructions such as:

- Follow the existing project structure.
- Do not introduce new dependencies without explaining why.
- Prefer simple solutions over unnecessary abstractions.
- Include tests for success, failure, and edge cases.
- Do not place business logic in the UI layer.
- Use the existing error-handling pattern.
- Explain any assumptions before generating code.

 If that code does not follow clear architectural boundaries, the codebase can become inconsistent faster than ever. 

AI does not automatically understand your system's architecture. It may understand common patterns, but it does not know the full history of your codebase, your team's decisions, your production incidents, your domain constraints, or your long-term roadmap.

That is why teams need clear architecture rules.

- Where does business logic belong?
- How should data access work?
- What belongs in the UI layer?
- How are errors represented?
- How are modules structured?
- What dependencies are allowed?
- What patterns are discouraged?
- What testing strategy should be followed?

These decisions should not live only in the heads of senior engineers. They should be documented, enforced through code review, supported by lint rules where possible, and included in AI prompts and internal guidance. 

## Treat AI Output as a Draft, Not a Decision 

When reviewing AI-generated code, I like to ask several questions:

- Does this follow the existing architecture?
- Is the abstraction necessary?
- Are the names clear and domain-specific?
- Does the code handle failure cases?
- Are there hidden assumptions?
- Does this introduce a dependency?
- Is the implementation more complex than the problem requires?
- Would I be comfortable debugging this at `2 a.m.` during a production issue?

> AI can suggest. Developers must decide.

Code review should pay special attention to AI-prone issues:

- Unnecessary abstractions.
- Inconsistent patterns.
- New dependencies.
- Security-sensitive logic.
- Error handling gaps.
- Tests that assert implementation details.
- Code that looks correct but does not match the domain.
- Duplicated functionality that already exists elsewhere. 

## Documentation Becomes Easier, but Still Needs Truth

One of the most practical uses of AI is documentation. Developers often delay documentation because they are focused on implementation. AI can help summarize code, generate usage examples, explain configuration, and create first drafts of README updates.

This is valuable, but documentation has to be true.

AI-generated documentation can sound authoritative while being slightly wrong. It may describe behavior that the code does not actually support. It may omit limitations. It may create examples that compile but do not match production usage.

The same rule applies: documentation generated by AI is a draft. The developer must verify it.

Good documentation should explain not only what the code does, but also why it exists and how it should be used. AI can help with the “what.” Developers are often still needed for the “why.”

That “why” is especially important in architecture. Future developers need to know why a boundary exists, why a dependency was avoided, why a module was split, or why a simpler-looking option was rejected. These decisions come from human context.



## Practical Guidelines for Teams

Every team will use AI differently, but a few practical guidelines can help.

- Use AI for acceleration, not abdication. The developer remains responsible for the code.
- Provide context. Include architecture, constraints, existing patterns, and testing expectations in prompts.
- Keep changes small. AI-generated large pull requests are still large pull requests.
- Review everything. Do not merge code you cannot explain.
- Be careful with dependencies. Do not accept new packages casually.
- Ask AI for alternatives. Sometimes the first answer is not the best.
- Use AI to find edge cases. It can be helpful for test planning and risk identification.
- Document architectural decisions. AI can help write the documentation, but humans must provide the reasoning.
- Protect junior learning. Do not let AI become a shortcut around fundamentals.
- Create team conventions. AI usage should align with the way your team builds software.

These guidelines are not complicated, but they require consistency. The teams that benefit most from AI will not be the teams that generate the most code. They will be the teams that combine faster generation with strong review, clear architecture, and disciplined ownership.



#Prompt 