namespace Backend.Application.BackgroundJobs;

public interface IJobService
{
    Task EnqueueAsync<T>(T requestInput, string methodName, CancellationToken cancellationToken);
}