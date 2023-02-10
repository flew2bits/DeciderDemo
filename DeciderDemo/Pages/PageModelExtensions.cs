using System.Text.Json;
using System.Xml;
using DateOnlyTimeOnly.AspNet.Converters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeciderDemo.Pages;

public static class PageModelExtensions
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true, Converters = { new DateOnlyJsonConverter(), new TimeOnlyJsonConverter()} };
    public static IEnumerable<string> SerializeEvents(this PageModel model, IEnumerable<object> events)
        => events.Select(e => $"{e.GetType().Name}: {JsonSerializer.Serialize(e, Options)}").ToArray();
}