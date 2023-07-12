namespace Findex.TechnicalTest.Helpers;

public class Result
{
	protected Result(bool success, string error, ResultErrorType? errorType)
	{
		if (success && error != string.Empty)
			throw new InvalidOperationException();
		if (!success && error == string.Empty)
			throw new InvalidOperationException();
		Success = success;
		Error = error;
		ErrorType = errorType;
	}

	public bool Success { get; }
	public string Error { get; }
    public ResultErrorType? ErrorType { get; set; }
    public bool IsFailure => !Success;

	public static Result Fail(string message, ResultErrorType? errorType)
	{
		return new Result(false, message, errorType);
	}

	public static Result<T?> Fail<T>(string message, ResultErrorType? errorType)
	{
		return new Result<T?>(default, false, message, errorType);
	}

	public static Result Ok()
	{
		return new Result(true, string.Empty, null);
	}

	public static Result<T> Ok<T>(T value)
	{
		return new Result<T>(value, true, string.Empty, null);
	}
}

public class Result<T> : Result
{
	protected internal Result(T value, bool success, string error, ResultErrorType? errorType)
		: base(success, error, errorType)
	{
		Value = value;
	}

	public T Value { get; set; }
}

public enum ResultErrorType
{
	Exception,
	NotFound,
	BadRequest
}