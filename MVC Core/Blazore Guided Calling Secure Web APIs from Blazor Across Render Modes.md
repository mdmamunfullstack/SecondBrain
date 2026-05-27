
Remember the three execution contexts:

- No directive = static SSR. Runs on the server during the HTTP request, returns plain HTML, no persistent connection.
- `@rendermode InteractiveServer` = runs on the server over a persistent SignalR circuit. Interactive, but still server-side.
- `@rendermode InteractiveWebAssembly` = runs inside the user's browser. Any secret in code that reaches this component is visible to anyone with DevTools.