using System.Runtime.CompilerServices;

namespace ShadcnBlazor.Docs.Components.Samples.AiChat.Services;

public class ChatService : IChatService
{
    public async IAsyncEnumerable<string> RunStreamingAsync(
        string _, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var rand = new Random();
        const int min = 2, max = 4;
        await Task.Delay(rand.Next(min, max + 1), cancellationToken);
        
        var hardcodedResponse = _exampleResponses[Random.Shared.Next(_exampleResponses.Length)];
        var words = hardcodedResponse.Split(' ');
        
        foreach (var word in words)
        {
            await Task.Delay(10, cancellationToken);
            yield return word + " ";
        }
    }
    
    private readonly string[] _exampleResponses = new[]
    {
        /*
"""
<thinking>
The user is asking about the best framework for building a web application. I should search for recent comparisons and performance benchmarks to give accurate information.
</thinking>

<search>
https://www.react.dev/learn
https://vuejs.org/guide/introduction.html
https://2023.stateofjs.com/en-US/libraries/front-end-frameworks/
https://bundlephobia.com/package/react@18.2.0
https://github.com/facebook/react/discussions
https://news.ycombinator.com/item?id=38934871
https://stackoverflow.com/questions/tagged/reactjs
https://www.patterns.dev/posts/react-performance
https://blog.logrocket.com/react-vs-vue-2024/
https://npmtrends.com/react-vs-vue
</search>

<text>
Based on current trends and performance data, here's what you should consider:

React remains the most popular choice with a larger ecosystem and job market. It has 220k+ GitHub stars and is backed by Meta. The virtual DOM provides excellent performance for most applications.

Vue.js offers a gentler learning curve and more opinionated structure. It's lighter (around 33KB gzipped vs React's 45KB) and has excellent documentation. The composition API in Vue 3 provides similar capabilities to React hooks.

For your decision, consider: team expertise, project complexity, and long-term maintenance needs.
</text>
""",

"""
<text>
I'll help you find the best running shoes for marathon training.
</text>

<thinking>
The user wants running shoe recommendations for marathons. I should search for expert reviews, compare popular models, and check current pricing and availability.
</thinking>

<search>
https://www.runnersworld.com/gear/best-running-shoes-2024
https://www.nike.com/running-shoes
https://www.adidas.com/us/running-shoes
https://www.hoka.com/en/us/marathon-shoes
https://www.brooksrunning.com/en_us/marathon-training-shoes
</search>

<text>
Based on current reviews, here are the top marathon shoes for 2024:

**Nike Vaporfly 3** - The gold standard for racing, used by elite marathoners worldwide. Features carbon fiber plate and ZoomX foam for maximum energy return.

**Adidas Adizero Adios Pro 3** - Excellent alternative with EnergyRods technology. Slightly more stable than the Vaporfly.
</text>

<thinking>
I should also search for pricing information and user reviews to give a complete picture. Let me look for comparison articles and current deals.
</thinking>

<search>
https://www.runrepeat.com/nike-vaporfly-3-review
https://www.runningwarehouse.com/adidas-adizero-adios-pro-3
https://www.reddit.com/r/running/comments/marathon_shoe_recommendations
https://www.amazon.com/running-shoes-marathon/s?k=marathon+running+shoes
</search>

<text>
**Price Range:**
- Nike Vaporfly 3: $250-$275
- Adidas Adios Pro 3: $225-$250

**Key Considerations:**
- Both require a break-in period of 20-30 miles
- Not recommended for daily training (use neutral trainers instead)
- Best for runners targeting sub-4 hour marathon times

For training, consider pairing these with a durable daily trainer like the Brooks Ghost 15 or Nike Pegasus 40.
</text>
""",
*/

"""
<thinking>
The user wants me to generate a longer, more realistic example of the streaming response format they're building. They want to see substantial `text` and `thinking` blocks as they would appear in a real scenario. This will help them build their component that displays these streamed XML blocks.

I should pick a technical question that naturally requires deep reasoning and a comprehensive answer. The SignalR vs SSE question is perfect because it has multiple dimensions to consider: performance, complexity, maintainability, browser support, scalability, deployment concerns, developer experience, and cost.

Let me structure this thoughtfully:
1. Start with thinking through the problem space
2. Search for relevant resources
3. Provide detailed analysis in text
4. More thinking as I consider additional angles
5. More searching for specific details
6. Final comprehensive recommendation in text

I'll make the thinking blocks show genuine reasoning - weighing trade-offs, considering edge cases, thinking about the user's specific context (Blazor), and building toward a recommendation. The text blocks should be thorough but conversational, covering all the key points someone would need to make this decision.
</thinking>

<search>
https://learn.microsoft.com/en-us/aspnet/core/signalr/introduction
https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/signalr
https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events
https://github.com/dotnet/aspnetcore/tree/main/src/SignalR
https://web.dev/eventsource-basics/
</search>

<text>
Let me break down the SignalR vs Server-Sent Events decision for real-time chat in Blazor comprehensively.

**SignalR Overview:**
SignalR is Microsoft's real-time communication library built into ASP.NET Core. It provides full-duplex (bidirectional) communication between client and server. The key advantage is its transport negotiation - it automatically chooses the best available transport: WebSockets (preferred), Server-Sent Events (fallback), or Long Polling (last resort).

For Blazor specifically, SignalR has first-class support. Blazor Server actually runs entirely on SignalR - the whole component model, DOM updates, and event handling flow through a SignalR connection called the "circuit." This means you already have SignalR infrastructure in place if you're using Blazor Server.

**Server-Sent Events (SSE) Overview:**
SSE is a browser standard for server-to-client streaming over HTTP. It's simpler than WebSockets - just an HTTP connection that stays open and streams text data. The browser's EventSource API handles reconnection automatically. However, SSE is unidirectional - server pushes to client only.

**Critical Consideration for Chat:**
Chat applications require bidirectional communication. Users need to both send messages (client-to-server) and receive messages (server-to-client). This is where the fundamental difference matters:

- **SignalR:** Single persistent connection handles both directions. Call `await Clients.All.SendAsync("ReceiveMessage", user, message)` from server, call `await hubConnection.InvokeAsync("SendMessage", user, message)` from client. Clean, symmetrical API.

- **SSE:** You'd need TWO separate channels:
  1. SSE stream for receiving messages (server-to-client)
  2. HTTP POST endpoints for sending messages (client-to-server)
  
This dual-channel approach adds complexity in connection management, error handling, and ensuring messages stay synchronized.
</text>

<thinking>
I should dig deeper into the performance and scalability implications. Both technologies have different resource consumption patterns that matter at scale.

SignalR with WebSockets keeps a persistent TCP connection per client. This means:
- Memory overhead per connection (connection state, buffers)
- Thread pool resources for managing connections
- But very low latency for messages (no HTTP overhead per message)
- Efficient binary protocol (MessagePack option)

SSE keeps an HTTP connection open per client:
- Similar memory overhead for the open connection
- But every client-to-server message requires a new HTTP request (overhead)
- Text-based protocol (less efficient than MessagePack)
- Browser limit of 6 concurrent connections per domain (can be a gotcha)

For Blazor WASM specifically, I need to consider that the client runs in the browser. WASM doesn't have the built-in SignalR circuit like Blazor Server does, so you'd be adding SignalR explicitly for chat. But WASM already needs some kind of API communication, so adding SignalR isn't adding a completely new dependency - you'd need HTTP endpoints anyway.

I should also consider authentication and authorization. SignalR integrates cleanly with ASP.NET Core auth - you can use [Authorize] attributes on hub methods, access HttpContext.User, and validate on each invocation. With SSE, auth is trickier because you're dealing with two separate channels.

Let me search for more specific performance data and real-world experiences.
</thinking>

<search>
https://learn.microsoft.com/en-us/aspnet/core/signalr/scale
https://stackoverflow.com/questions/tagged/signalr+blazor
https://github.com/dotnet/aspnetcore/issues?q=is%3Aissue+signalr+performance
https://learn.microsoft.com/en-us/aspnet/core/signalr/security
https://www.npgsql.org/doc/replication.html
https://redis.io/docs/manual/pubsub/
</search>

<text>
**Performance & Scalability Deep Dive:**

**Connection Overhead:**
Both SignalR (WebSockets) and SSE maintain persistent connections, so memory overhead is comparable - roughly 10-50KB per connection depending on buffer sizes and framework overhead. However, the message throughput characteristics differ significantly:

- **SignalR:** Once the WebSocket is established, messages flow with minimal overhead. No HTTP headers, no request/response ceremony. Just the message payload. MessagePack binary serialization makes this even more efficient than JSON.

- **SSE:** Receiving messages is efficient (streaming over the persistent HTTP connection), but SENDING messages requires a full HTTP POST for each message. This means every chat message you send incurs: DNS lookup (cached usually), TCP handshake (can be reused with keep-alive), TLS handshake (can be resumed), HTTP headers, request/response cycle. This adds latency and server load.

**Scaling Horizontally:**
For chat, you typically need a backplane when scaling to multiple servers - when User A on Server 1 sends a message, User B on Server 2 needs to receive it.

- **SignalR:** Built-in backplane support via Redis, Azure SignalR Service, or SQL Server. You add a NuGet package, configure the connection string, and it handles message distribution across servers. Well-tested, production-ready.

- **SSE:** No built-in backplane. You'd implement your own pub/sub system (Redis PUBLISH/SUBSCRIBE, RabbitMQ, etc.) to broadcast messages to all server instances. More custom code, more potential for bugs.

**Connection Limits & Browser Constraints:**
This is a critical gotcha with SSE - browsers limit concurrent HTTP connections to 6 per domain (HTTP/1.1) or about 100 (HTTP/2, but varies). If a user has 6 SSE connections open (maybe multiple tabs, or your app uses SSE for multiple features), the 7th connection blocks. WebSockets don't count against this limit.

**Blazor-Specific Considerations:**

For **Blazor Server:**
- You already have a SignalR connection (the circuit). Adding chat via the same SignalR connection or a separate hub is trivial.
- Using SSE would mean adding a completely separate technology stack for essentially the same purpose (server-to-client push).
- **Recommendation here: SignalR is the obvious choice.** You're already using it, why add SSE?

For **Blazor WASM:**
- You're making HTTP calls to APIs anyway, so you'll have HTTP infrastructure.
- SignalR adds a new library to your client bundle (~60KB gzipped for the JavaScript client), but it's a one-time cost.
- SSE is browser-native (no library needed), but you still need to write the client-side logic to handle the EventSource, reconnection, message parsing, etc.
- **Key question:** Do you have other real-time needs beyond chat? If yes, SignalR's versatility wins. If chat is the ONLY real-time feature and you want minimal bundle size, SSE might be considered, but...
</text>

<thinking>
Wait, I should reconsider the SSE argument more fairly. Let me think about scenarios where SSE might actually be preferable:

1. **Extreme simplicity requirement:** If you're building a read-only notification feed (not bidirectional chat), SSE is genuinely simpler. No hub configuration, no connection management complexity, just open an EventSource and listen.

2. **Infrastructure constraints:** Some corporate firewalls or proxies block WebSockets but allow long-lived HTTP connections. SSE might work where SignalR's WebSocket transport fails (though SignalR would fall back to SSE anyway in this case).

3. **Serverless environments:** Azure Functions, AWS Lambda with HTTP streaming - SSE might be easier to implement than SignalR in these constrained environments.

But for chat specifically? The bidirectional requirement really undermines SSE's case. Let me think through a concrete example:

**Chat with SignalR:**
```csharp
// Server Hub
public async Task SendMessage(string user, string message)
{
    await Clients.All.SendAsync("ReceiveMessage", user, message);
}

// Client
await hubConnection.InvokeAsync("SendMessage", username, messageText);
hubConnection.On<string, string>("ReceiveMessage", (user, message) => { /* display */ });
```

**Chat with SSE:**
```csharp
// Server - SSE endpoint
[HttpGet("chat/stream")]
public async Task Stream()
{
    Response.ContentType = "text/event-stream";
    // Complex logic to subscribe to chat events, stream them, handle disconnects...
}

// Server - POST endpoint
[HttpPost("chat/send")]
public async Task SendMessage([FromBody] ChatMessage msg)
{
    // Publish to all SSE streams somehow (custom pub/sub)
}

// Client
var eventSource = new EventSource("/chat/stream");
eventSource.onmessage = (e) => { /* parse and display */ };

// To send:
await fetch("/chat/send", { method: "POST", body: JSON.stringify(msg) });
```

The SSE approach requires significantly more custom plumbing. You need to implement message broadcasting yourself, handle connection tracking, manage reconnection logic, synchronize the two separate channels.

Actually, I should also consider developer experience and ecosystem maturity. SignalR has been around since 2013, it's battle-tested, there are tons of Stack Overflow answers, the documentation is comprehensive, and it's actively maintained by Microsoft. SSE is a browser standard but there's less .NET-specific guidance and fewer ready-made solutions for common problems.

One more consideration: monitoring and debugging. SignalR integrates with ASP.NET Core logging, Application Insights, and has browser dev tools support. You can see connection states, trace messages, diagnose failures. With a custom SSE implementation, you'd build this yourself.

Let me also think about the authentication story more carefully. With SignalR:
- Authenticate during connection negotiation
- Token automatically sent with each WebSocket frame
- Can re-authenticate with access token refresh
- Context.User available in hub methods

With SSE:
- Authenticate the initial SSE connection (pass token as query param or header)
- For POST requests, authenticate separately (typically Authorization header)
- Need to synchronize auth state across two channels
- Token refresh more complex

I think I have enough to make a strong recommendation now.
</thinking>

<search>
https://learn.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz
https://azure.microsoft.com/en-us/products/signalr-service/
https://learn.microsoft.com/en-us/aspnet/core/blazor/security/
https://github.com/dotnet/aspnetcore/blob/main/src/SignalR/docs/specs/HubProtocol.md
</search>

<text>
**Developer Experience & Maintenance:**

**SignalR Advantages:**
- **Mature ecosystem:** Tons of documentation, Stack Overflow answers, community packages (SignalR for background services, typed hubs, testing utilities)
- **Official Microsoft support:** Actively maintained, regular updates, security patches
- **Integrated tooling:** Works seamlessly with ASP.NET Core middleware, logging, dependency injection, authentication
- **Debugging:** Built-in logging, browser DevTools support, easy to trace connection issues
- **Deployment:** Azure SignalR Service available for massive scale without managing infrastructure yourself

**SSE Advantages:**
- **Simplicity for one-way scenarios:** If you genuinely only need server-to-client push (notifications, live updates), SSE is dead simple
- **No client library needed:** EventSource is built into browsers, smaller client bundle
- **HTTP-friendly:** Works through most proxies, no special firewall rules needed
- **Standard protocol:** Not tied to a specific framework or vendor

**Authentication & Authorization:**

This is where SignalR really shines for chat:

```csharp
// SignalR - Clean auth story
[Authorize]
public class ChatHub : Hub
{
    public async Task SendMessage(string message)
    {
        var user = Context.User.Identity.Name;
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
    
    // Can also authorize specific methods
    [Authorize(Roles = "Moderator")]
    public async Task DeleteMessage(string messageId) { }
}
```

With SSE, you'd need to:
1. Authenticate the SSE connection (token in query string or custom header - query string can leak in logs)
2. Authenticate each POST request separately (Authorization header)
3. Ensure the user authenticated in SSE matches the user in POST requests
4. Handle token refresh for both channels independently

**My Recommendation:**

**Use SignalR if:**
- ✅ You're building actual chat (bidirectional communication) - **This is you**
- ✅ You're using Blazor Server (already have SignalR)
- ✅ You need robust scaling (built-in backplane support)
- ✅ You value developer productivity and ecosystem maturity
- ✅ You might add other real-time features later (notifications, live updates, collaborative editing)
- ✅ You want first-class authentication/authorization integration
- ✅ You need reliable delivery, automatic reconnection, and connection state management

**Consider SSE if:**
- ⚠️ You only need one-way server-to-client push (not chat)
- ⚠️ You're building for a constrained environment where SignalR isn't available
- ⚠️ Client bundle size is absolutely critical and you can't afford 60KB for SignalR
- ⚠️ You're very comfortable building custom infrastructure for message broadcasting, reconnection, auth synchronization

**For your Blazor chat application, SignalR is the clear winner.** The bidirectional requirement, Blazor's existing SignalR integration (especially in Server mode), mature ecosystem, and built-in scaling support make it the obvious choice. The added complexity of implementing chat over SSE (dual channels, custom pub/sub, auth synchronization) isn't justified by any meaningful benefit for your use case.

**Concrete Next Steps:**
1. Add SignalR to your project (already there in Blazor Server, `dotnet add package Microsoft.AspNetCore.SignalR.Client` for WASM)
2. Create a ChatHub with SendMessage/ReceiveMessage methods
3. Configure connection in your Blazor component
4. For scaling: Add Redis backplane (`Microsoft.AspNetCore.SignalR.StackExchangeRedis`) or use Azure SignalR Service
5. Implement proper error handling and reconnection logic

The only scenario where I'd recommend SSE for chat is if you have some bizarre constraint that absolutely prohibits SignalR - and even then, I'd question whether that constraint is real or perceived.
</text>
""",
    };
}