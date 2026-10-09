using MiniMES.Api.Common.Exceptions;
using MiniMES.Application.Common.Models;

namespace MiniMES.Api.Common.Extensions;

public static class ResultExtensions
{
    public static void ThrowIfFailure(this Result result)
    {
        if (result.IsFailure)
        {
            throw new BusinessException(result.Error);
        }
    }

    public static T GetValue<T>(this Result<T> result)
    {
        result.ThrowIfFailure();
        return result.Value!;
    }
}
