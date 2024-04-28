using FluentResults;

namespace CloudSharp.Data.Dapper.Query;

public interface IQuery<T>
{
    public ValueTask<Result<T>> Query();
}