namespace DeciderDemo.Entities;

public class HttpContextEventMetadataProvider : IEventMetadataProvider
{
    private readonly IHttpContextAccessor _accessor;
    private const string UserName = nameof(UserName);
    public HttpContextEventMetadataProvider(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public string Category => "HttpContext";
    
    public IEnumerable<KeyValuePair<string, object>> GetValues()
    {
        var context = _accessor.HttpContext;
        if (context is null) yield return new KeyValuePair<string, object>(UserName, "SYSTEM");
        else yield return new KeyValuePair<string, object>(UserName, context.User.Identity?.Name ?? "anonymous");
    }
}