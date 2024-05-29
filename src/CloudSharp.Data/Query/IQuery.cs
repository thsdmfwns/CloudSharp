using FluentResults;

namespace CloudSharp.Data.Query;

public interface IQuery<T>
{
    public string DbConnectionString { get; init; }
    public ValueTask<Result<T>> Query();
}