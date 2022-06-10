namespace Qweree.Timez.WebApi.Domain.Projects;

public class Project
{
    public Project(Guid id, Guid userId, string projectName, DateTime createdAt, DateTime modifiedAt)
    {
        Id = id;
        UserId = userId;
        ProjectName = projectName;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }

    public Guid Id { get; }
    public Guid UserId { get; }
    public string ProjectName { get; }
    public DateTime CreatedAt { get; }
    public DateTime ModifiedAt { get; }
}