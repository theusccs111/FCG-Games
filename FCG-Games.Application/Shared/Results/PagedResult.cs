using System.Diagnostics.CodeAnalysis;

namespace FCG_Games.Application.Shared.Results
{
    public class PagedResult
    {
        protected PagedResult(bool isSuccess, Error error, Pagination? pagination)
        {
            switch (isSuccess)
            {
                case true when error != Error.None:
                case false when error == Error.None:
                    throw new InvalidOperationException();

                default:
                    IsSuccess = isSuccess;
                    Error = error;
                    break;
            }

            Pagination = pagination;
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }
        public Pagination? Pagination { get; set; }

        public static PagedResult<T> Success<T>(T value, Pagination pagination) => new(value, true, Error.None, pagination);
        public static PagedResult<T> Failure<T>(Error error) => new(default, false, error, null);

        public static PagedResult<T> Create<T>(T? value, Pagination pagination) =>
            value is not null ? Success(value, pagination) : Failure<T>(Error.NullValue);
    }

    public class PagedResult<T> : PagedResult
    {
        private readonly T? _value;

        protected internal PagedResult(T? value, bool isSuccess, Error error, Pagination? pagination) : base(isSuccess, error, pagination)
            => _value = value;

        [NotNull]

        public T Value => _value! ?? throw new InvalidOperationException("O resultado não tem valor");

    }
}
