namespace Gallerai.SharedKernel.Models;

public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException();

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);

    public TResult Match<TResult>(Func<TResult> onSuccess, Func<Error, TResult> onFailure)
    {
        return IsSuccess ? onSuccess() : onFailure(Error);
    }
}

public class Result<T> : Result
{
    protected Result(T? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public T Value => IsSuccess
        ? field!
        : throw new InvalidOperationException("The value of a failure result can not be accessed.");

    public static implicit operator Result<T>(T? value) => Create(value);

    public static implicit operator Result<T>(Error error) => new(default, false, error);

    public static Result<T> Success(T value) => new(value, true, Error.None);
    public static new Result<T> Failure(Error error) => new(default, false, error);

    public static Result<T> Create(T? value) =>
        value is not null ? Success(value) : Failure(Error.NullValue);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value) : onFailure(Error);
    }
}