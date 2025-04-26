using System;
using System.Threading;
using System.Threading.Tasks;

namespace MDiator
{
    public class ErrorHandlingMiddleware<TRequest, TResponse> : IMediatorPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                return await next();
            }
            catch (Exception ex)
            {
                // Log the exception here
                Console.WriteLine($"Error occurred: {ex.Message}");
                throw;
            }
        }
    }
} 