using System.Text.RegularExpressions;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ShadcnBlazor.Docs.Components.Samples.MarkdownEditor.Components;

public partial class MarkdownEditor : ComponentBase
{
    [Inject]
    public required IJSRuntime JSRuntime { get; set; }

    private enum ViewType
    {
        Edit,
        Preview
    }

    private const string ToolbarButtonClass = "h-7 w-7 p-0 text-muted-foreground hover:text-foreground";

    private readonly string _textAreaId = $"markdown-editor-{Guid.NewGuid():N}";
    private string _markdown = string.Empty;
    private string _previewHtml = string.Empty;
    private string _lastPreviewMarkdown = string.Empty;
    private ViewType _viewType = ViewType.Edit;
    private int _textAreaHeightPx = 140;

    private string GetTabClass(ViewType viewType)
    {
        var baseClasses = "h-full px-4 text-sm font-medium transition-colors";
        return viewType == _viewType
            ? $"{baseClasses} border-b-2 border-primary text-foreground"
            : $"{baseClasses} text-muted-foreground hover:text-foreground";
    }

    private async Task SwitchView(ViewType viewType)
    {
        if (_viewType == viewType)
        {
            return;
        }

        if (viewType == ViewType.Preview)
        {
            _textAreaHeightPx = await JSRuntime.InvokeAsync<int>("shadcnMarkdownEditor.getTextAreaHeight", _textAreaId);
            BuildPreview();
        }

        _viewType = viewType;
    }

    private void BuildPreview()
    {
        if (_markdown == _lastPreviewMarkdown)
        {
            return;
        }

        _previewHtml = Markdown.ToHtml(_markdown ?? string.Empty);
        _lastPreviewMarkdown = _markdown;
    }

    private async Task HeaderButtonPressed()
    {
        var info = await GetTextAreaInfo();
        if (info is null)
        {
            return;
        }

        var startPos = info.Start;
        while (startPos > 0 && !IsWhiteSpaceOrZero(info.Value[startPos - 1]))
        {
            startPos--;
        }

        var newText = $"{info.Value[..startPos]} ### {info.Value[startPos..]}";
        await ApplyTextAreaUpdate(newText, startPos + 5, startPos + 5);
    }

    private async Task BoldButtonPressed()
    {
        await FormatTextAreaSelection(
            (beforeSelection, selection, afterSelection) => $"{beforeSelection}**{selection}**{afterSelection}",
            positions => positions.StartPos + 2,
            positions => positions.EndPos + 2);
    }

    private async Task ItalicButtonPressed()
    {
        await FormatTextAreaSelection(
            (beforeSelection, selection, afterSelection) => $"{beforeSelection}_{selection}_{afterSelection}",
            positions => positions.StartPos + 1,
            positions => positions.EndPos + 1);
    }

    private async Task QuoteButtonPressed()
    {
        var info = await GetTextAreaInfo();
        if (info is null)
        {
            return;
        }

        var startPos = info.Start;
        var endPos = info.End;

        if (startPos != endPos)
        {
            AdjustPositionsForSelection(info.Value, ref startPos, ref endPos);
            var newText = $"{info.Value[..startPos]}\n\n> {info.Value[startPos..endPos]}\n\n{info.Value[endPos..]}";
            await ApplyTextAreaUpdate(newText, startPos + 4, endPos + 4);
            return;
        }

        while (startPos > 0 && !IsWhiteSpaceOrZero(info.Value[startPos - 1]))
        {
            startPos--;
        }

        var fallbackText = $"{info.Value[..startPos]}\n\n> {info.Value[startPos..]}";
        await ApplyTextAreaUpdate(fallbackText, startPos + 4, startPos + 4);
    }

    private async Task CodeButtonPressed()
    {
        await FormatTextAreaSelection(
            (beforeSelection, selection, afterSelection) => $"{beforeSelection}`{selection}`{afterSelection}",
            positions => positions.StartPos + 1,
            positions => positions.EndPos + 1);
    }

    private async Task LinkButtonPressed()
    {
        await FormatTextAreaSelection(
            (beforeSelection, selection, afterSelection) => $"{beforeSelection}[{selection}](url){afterSelection}",
            positions => positions.EndPos + 3,
            positions => positions.EndPos + 6);
    }

    private async Task NumberedListButtonPressed()
    {
        await FormatTextAreaLine(
            slice => $"1. {slice}",
            startPos => startPos + 3,
            @"^\d+. ");
    }

    private async Task UnorderedListButtonPressed()
    {
        await FormatTextAreaLine(
            slice => $"- {slice}",
            startPos => startPos + 2,
            "^- ");
    }

    private async Task TaskListButtonPressed()
    {
        await FormatTextAreaLine(
            slice => $"- [ ] {slice}",
            startPos => startPos + 6,
            @"^- \[[ x]\]");
    }

    private delegate string FormatSlicesFunc(string beforeSelection, string selection, string afterSelection);

    private async Task FormatTextAreaSelection(
        FormatSlicesFunc formatSlicesFunc,
        Func<(int StartPos, int EndPos), int> newSelectionStartPosFunc,
        Func<(int StartPos, int EndPos), int> newSelectionEndPosFunc)
    {
        var info = await GetTextAreaInfo();
        if (info is null)
        {
            return;
        }

        var startPos = info.Start;
        var endPos = info.End;

        if (startPos != endPos)
        {
            AdjustPositionsForSelection(info.Value, ref startPos, ref endPos);
        }
        else
        {
            TrySelectAdjacentWord(info.Value, ref startPos, ref endPos);
        }

        var newText = formatSlicesFunc(info.Value[..startPos], info.Value[startPos..endPos], info.Value[endPos..]);
        await ApplyTextAreaUpdate(
            newText,
            newSelectionStartPosFunc((startPos, endPos)),
            newSelectionEndPosFunc((startPos, endPos)));
    }

    private async Task FormatTextAreaLine(
        Func<string, string> formatSlice,
        Func<int, int> newCursorPosFunc,
        string regexPattern)
    {
        var info = await GetTextAreaInfo();
        if (info is null)
        {
            return;
        }

        var startPos = info.Start;
        while (startPos > 0 && !IsNewLineOrLineFeed(info.Value[startPos - 1]))
        {
            startPos--;
        }

        string modifiedText;
        int cursorPosition;
        if (Regex.IsMatch(info.Value[startPos..], regexPattern))
        {
            modifiedText = Regex.Replace(info.Value[startPos..], regexPattern, "");
            cursorPosition = startPos;
        }
        else
        {
            modifiedText = formatSlice(info.Value[startPos..]);
            cursorPosition = newCursorPosFunc(startPos);
        }

        var newText = $"{info.Value[..startPos]}{modifiedText}";
        await ApplyTextAreaUpdate(newText, cursorPosition, cursorPosition);
    }

    private async Task<TextAreaInfo?> GetTextAreaInfo()
    {
        return await JSRuntime.InvokeAsync<TextAreaInfo?>("shadcnMarkdownEditor.getTextAreaInfo", _textAreaId);
    }

    private async Task ApplyTextAreaUpdate(string newText, int selectionStart, int selectionEnd)
    {
        _markdown = newText;
        await JSRuntime.InvokeVoidAsync(
            "shadcnMarkdownEditor.setTextAreaValueAndSelection",
            _textAreaId,
            newText,
            selectionStart,
            selectionEnd);
    }

    private static void AdjustPositionsForSelection(string text, ref int startPos, ref int endPos)
    {
        while (startPos < text.Length && IsWhiteSpaceOrZero(text[startPos]))
        {
            startPos++;
        }

        endPos = Math.Max(startPos, endPos - 1);
        while (endPos > 0 && IsWhiteSpaceOrZero(text[endPos]))
        {
            endPos--;
        }

        endPos = Math.Min(text.Length, endPos + 1);
    }

    private static void TrySelectAdjacentWord(string text, ref int startPos, ref int endPos)
    {
        while (startPos > 0 && !IsWhiteSpaceOrZero(text[startPos - 1]))
        {
            startPos--;
        }

        while (endPos < text.Length && !IsWhiteSpaceOrZero(text[endPos]))
        {
            endPos++;
        }
    }

    private static bool IsWhiteSpaceOrZero(char character)
    {
        return character == '\0' || char.IsWhiteSpace(character);
    }

    private static bool IsNewLineOrLineFeed(char character)
    {
        return character is '\n' or '\r';
    }

    private sealed class TextAreaInfo
    {
        public string Value { get; set; } = string.Empty;
        public int Start { get; set; }
        public int End { get; set; }
    }
}

