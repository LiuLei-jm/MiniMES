using MiniMES.Application.Common.Models;

namespace MiniMES.Api.Common.Exceptions;

public sealed class BusinessException : Exception
{
    public BusinessException(Error error) : base(error.Message) => Error = error;

    public Error Error { get; }
}
