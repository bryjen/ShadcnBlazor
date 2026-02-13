using System.Runtime.CompilerServices;

namespace ShadcnBlazor.Docs.Components.Samples.AiChat.Services;

public class ChatService
{
    public async IAsyncEnumerable<string> RunStreamingAsync(
        string _, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var hardcodedResponse = _exampleResponses[Random.Shared.Next(_exampleResponses.Length)];
        var words = hardcodedResponse.Split(' ');
        
        foreach (var word in words)
        {
            await Task.Delay(150, cancellationToken);
            yield return word + " ";
        }
    }
    
    private readonly string[] _exampleResponses = new[]
    {
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
    };
}