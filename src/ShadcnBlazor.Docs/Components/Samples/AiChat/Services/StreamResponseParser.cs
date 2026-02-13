using System.Text;
using ShadcnBlazor.Docs.Components.Samples.AiChat.Models;

namespace ShadcnBlazor.Docs.Components.Samples.AiChat.Services;

public class StreamResponseParser(List<MessageComponent> components)
{
    private readonly StringBuilder _buffer = new();
    private ParseMode _mode = ParseMode.None;
    private MessageComponent? _activeComponent;

    public void AppendChunk(string chunk)
    {
        if (string.IsNullOrEmpty(chunk))
            return;

        _buffer.Append(chunk);

        var processedUpTo = 0;

        while (processedUpTo < _buffer.Length)
        {
            var tagStart = FindNextUnescapedChar(_buffer, '<', processedUpTo);

            if (tagStart < 0)
            {
                var keepTrailingEscape = _buffer[^1] == '\\';
                AppendTextIfInMode(_buffer, processedUpTo, _buffer.Length - processedUpTo, keepTrailingEscape);
                processedUpTo = keepTrailingEscape ? _buffer.Length - 1 : _buffer.Length;
                break;
            }

            if (_mode != ParseMode.None && tagStart > processedUpTo)
            {
                AppendTextIfInMode(_buffer, processedUpTo, tagStart - processedUpTo, keepTrailingEscape: false);
            }

            var tagEnd = FindNextUnescapedChar(_buffer, '>', tagStart + 1);
            if (tagEnd < 0)
            {
                processedUpTo = tagStart;
                break;
            }

            var tagToken = _buffer.ToString(tagStart + 1, tagEnd - tagStart - 1).Trim();
            ProcessTag(tagToken);

            processedUpTo = tagEnd + 1;
        }

        if (processedUpTo > 0)
            _buffer.Remove(0, processedUpTo);
    }

    private void ProcessTag(string tagToken)
    {
        if (string.IsNullOrWhiteSpace(tagToken))
            return;

        if (IsEndTag(tagToken, "thinking") || IsEndTag(tagToken, "search") || IsEndTag(tagToken, "text"))
        {
            ResetMode();
            return;
        }

        if (IsStartTag(tagToken, "thinking"))
        {
            SetMode(ParseMode.Thinking);
            return;
        }

        if (IsStartTag(tagToken, "search"))
        {
            SetMode(ParseMode.Search);
            return;
        }

        if (IsStartTag(tagToken, "text"))
        {
            SetMode(ParseMode.Text);
        }
    }

    private void SetMode(ParseMode mode)
    {
        _mode = mode;
        _activeComponent = CreateComponent(mode);
        components.Add(_activeComponent);
    }

    private void ResetMode()
    {
        if (_mode == ParseMode.Thinking && _activeComponent is ThinkingMessageComponent thinking)
        {
            var rand = new Random();
            const int min = 5, max = 10;
            thinking.ThinkingTime = rand.Next(min, max + 1);
        }
        
        _mode = ParseMode.None;
        _activeComponent = null;
    }

    private static MessageComponent CreateComponent(ParseMode mode)
    {
        return mode switch
        {
            ParseMode.Thinking => new ThinkingMessageComponent { Content = string.Empty },
            ParseMode.Search => new WebSearchMessageComponent { Content = string.Empty },
            ParseMode.Text => new TextMessageComponent { Content = string.Empty },
            _ => throw new InvalidOperationException("Cannot create component for None mode.")
        };
    }

    private void AppendTextIfInMode(StringBuilder buffer, int start, int length, bool keepTrailingEscape)
    {
        if (_mode == ParseMode.None || _activeComponent is null || length <= 0)
            return;

        var appendLength = length;
        if (keepTrailingEscape && buffer[start + length - 1] == '\\')
            appendLength -= 1;

        if (appendLength <= 0)
            return;

        var segment = buffer.ToString(start, appendLength);
        var normalized = UnescapeForContent(segment);

        AppendToComponent(_activeComponent, normalized);
    }

    private static void AppendToComponent(MessageComponent component, string content)
    {
        if (string.IsNullOrEmpty(content))
            return;

        switch (component)
        {
            case ThinkingMessageComponent thinking:
                thinking.Content += content;
                break;
            case WebSearchMessageComponent webSearch:
                webSearch.Content += content;
                break;
            case TextMessageComponent text:
                text.Content += content;
                break;
        }
    }

    private static string UnescapeForContent(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return value
            .Replace("\\\\", "\\")
            .Replace("\\<", "<")
            .Replace("\\>", ">");
    }

    private static bool IsStartTag(string tagToken, string name)
        => tagToken.Equals(name, StringComparison.OrdinalIgnoreCase);

    private static bool IsEndTag(string tagToken, string name)
        => tagToken.Equals("/" + name, StringComparison.OrdinalIgnoreCase);

    private static int FindNextUnescapedChar(StringBuilder buffer, char target, int startIndex)
    {
        for (var i = startIndex; i < buffer.Length; i++)
        {
            if (buffer[i] != target)
                continue;

            if (!IsEscaped(buffer, i))
                return i;
        }

        return -1;
    }

    private static bool IsEscaped(StringBuilder buffer, int index)
    {
        var backslashCount = 0;
        for (var i = index - 1; i >= 0 && buffer[i] == '\\'; i--)
            backslashCount++;

        return backslashCount % 2 == 1;
    }

    private enum ParseMode
    {
        None,
        Thinking,
        Search,
        Text
    }
}
