
namespace DeciderDemo.Entities.Conference.Commands;

public interface IConferenceCommand
{
    string User { get; }
}

public interface IWorkshopCommand : IConferenceCommand
{
}