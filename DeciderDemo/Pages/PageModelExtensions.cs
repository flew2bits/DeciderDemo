using System.Text.Json;
using System.Xml;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeciderDemo.Pages;

public static class PageModelExtensions
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };
    public static IEnumerable<string> SerializeEvents(this PageModel model, IEnumerable<object> events)
        => events.Select(e => $"{e.GetType().Name}: {JsonSerializer.Serialize(e, Options)}").ToArray();
}