namespace Claims.Application.Abstractions;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}