using System.Threading;
using System.Threading.Tasks;

namespace MDiator
{
    public interface IMediatorPipelineBehavior<TRequest, TResponse>
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next);
    }
} 