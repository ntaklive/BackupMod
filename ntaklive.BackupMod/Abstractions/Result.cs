using System;

namespace ntaklive.BackupMod.Abstractions
{
    public class Result
    {
        public Result(string? errorMessage = null)
        {
            ErrorMessage = errorMessage;
        }

        public static Result Success()
        {
            return new Result(null);
        }

        public static Result Error(string? message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("The error message cannot be empty", nameof(message));
            }

            return new Result(message);
        }

        public string? ErrorMessage { get; }

        public bool IsSuccess => string.IsNullOrWhiteSpace(ErrorMessage);
    }

    public sealed class Result<T> : Result
        where T : class
    {
        public Result(string? errorMessage, T data) : base(errorMessage)
        {
            Data = data;
        }
    
        public static Result<T> Success(T data)
        {
            return new Result<T>(null, data);
        }

        public new static Result<T> Error(string? message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("The error message cannot be empty", nameof(message));
            }

            return new Result<T>(message, null!);
        }

        public T? Data { get; }
    }

    public sealed class ValueResult<T> : Result
    {
        public ValueResult(string? errorMessage, T data) : base(errorMessage)
        {
            Data = data;
        }
    
        public static ValueResult<T> Success(T data)
        {
            return new ValueResult<T>(null, data);
        }

        public new static ValueResult<T?> Error(string? message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("The error message cannot be empty", nameof(message));
            }

            return new ValueResult<T?>(message, default);
        }

        public T Data { get; }
    }
}