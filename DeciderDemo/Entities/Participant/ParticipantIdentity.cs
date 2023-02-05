namespace DeciderDemo.Entities.Participant;

public record ParticipantIdentity(string UserName)
{
    public override string ToString() => UserName;
    
    public static ParticipantIdentity Parse(string value) =>
        !string.IsNullOrEmpty(value)
            ? new ParticipantIdentity(value)
            : throw new InvalidOperationException("Participant Identity must be a non-empty string");

    public static implicit operator ParticipantIdentity(string s) => Parse(s);
}