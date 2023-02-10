namespace DeciderDemo.Entities.Participant;

public record ParticipantIdentity(string UserName)
    #if NET7_0_OR_GREATER
    : IParsable<ParticipantIdentity>
#endif
{
    public override string ToString() => UserName;

    public static implicit operator ParticipantIdentity(string s) => Parse(s, null);
    
    public static ParticipantIdentity Parse(string s, IFormatProvider? provider)
    =>
        TryParse(s, provider, out var pi) ? pi : throw new InvalidOperationException("Participant Identity must be a non-empty string");

    public static bool TryParse(string? s, IFormatProvider? provider, out ParticipantIdentity result)
    {
        result = null!;
        if (string.IsNullOrEmpty(s)) return false;
        result = new ParticipantIdentity(s);
        return true;
    }
}