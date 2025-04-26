using System;
using System.Threading;
using System.Threading.Tasks;

namespace MDiator
{
    public class ValidationMiddleware<TRequest, TResponse> : IMediatorPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            // Perform validation here
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return await next();
        }
    }
} 