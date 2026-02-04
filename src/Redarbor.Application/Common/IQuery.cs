using MediatR;

namespace Redarbor.Application.Common;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
