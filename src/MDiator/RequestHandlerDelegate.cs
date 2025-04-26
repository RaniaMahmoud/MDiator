using System.Threading;
using System.Threading.Tasks;

namespace MDiator
{
    public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();
} 