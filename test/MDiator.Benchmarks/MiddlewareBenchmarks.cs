using BenchmarkDotNet.Attributes;
using MDiator.Benchmarks.Events;
using MDiator.Benchmarks.Requests;
using Microsoft.Extensions.DependencyInjection;
using MDiator;
using System.Threading;

namespace MDiator.Benchmarks;

[MemoryDiagnoser]
public class MiddlewareBenchmarks
{
    private readonly IMediator _mediatorWithMiddleware;
    private readonly IMediator _mediatorWithoutMiddleware;
    private readonly ShortRequest _shortRequest;
    private readonly LongRequest _longRequest;
    private readonly ShortEvent _shortEvent;
    private readonly LongEvent _longEvent;
    private readonly ErrorRequest _errorRequest;

    public MiddlewareBenchmarks()
    {
        var servicesWithMiddleware = new ServiceCollection();
        var servicesWithoutMiddleware = new ServiceCollection();
        var assembly = typeof(MiddlewareBenchmarks).Assembly;

        // Setup with middleware
        servicesWithMiddleware
            .AddMDiator(assembly)
            .AddMediatorPipelineBehaviors();

        // Setup without middleware
        servicesWithoutMiddleware
            .AddMDiator(assembly);

        var providerWithMiddleware = servicesWithMiddleware.BuildServiceProvider();
        var providerWithoutMiddleware = servicesWithoutMiddleware.BuildServiceProvider();

        _mediatorWithMiddleware = providerWithMiddleware.GetRequiredService<IMediator>();
        _mediatorWithoutMiddleware = providerWithoutMiddleware.GetRequiredService<IMediator>();

        _shortRequest = new ShortRequest();
        _longRequest = new LongRequest();
        _shortEvent = new ShortEvent();
        _longEvent = new LongEvent();
        _errorRequest = new ErrorRequest();
    }

    // Request Benchmarks
    [Benchmark(Baseline = true)]
    public async Task ShortRequest_WithoutMiddleware() => await _mediatorWithoutMiddleware.Send(_shortRequest);

    [Benchmark]
    public async Task ShortRequest_WithMiddleware() => await _mediatorWithMiddleware.Send(_shortRequest);

    [Benchmark]
    public async Task LongRequest_WithoutMiddleware() => await _mediatorWithoutMiddleware.Send(_longRequest);

    [Benchmark]
    public async Task LongRequest_WithMiddleware() => await _mediatorWithMiddleware.Send(_longRequest);

    [Benchmark]
    public async Task ErrorRequest_WithoutMiddleware() => await _mediatorWithoutMiddleware.Send(_errorRequest);

    [Benchmark]
    public async Task ErrorRequest_WithMiddleware() => await _mediatorWithMiddleware.Send(_errorRequest);

    // Event Benchmarks
    [Benchmark]
    public async Task ShortEvent_WithoutMiddleware() => await _mediatorWithoutMiddleware.Publish(_shortEvent);

    [Benchmark]
    public async Task ShortEvent_WithMiddleware() => await _mediatorWithMiddleware.Publish(_shortEvent);

    [Benchmark]
    public async Task LongEvent_WithoutMiddleware() => await _mediatorWithoutMiddleware.Publish(_longEvent);

    [Benchmark]
    public async Task LongEvent_WithMiddleware() => await _mediatorWithMiddleware.Publish(_longEvent);
}

public class ValidationMiddleware<TRequest, TResponse> : IMediatorPipelineBehavior<TRequest, TResponse>
    where TRequest : IMDiatorRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        // Simulate validation logic
        await Task.Delay(1, cancellationToken);
        return await next();
    }
}

public class ErrorHandlingMiddleware<TRequest, TResponse> : IMediatorPipelineBehavior<TRequest, TResponse>
    where TRequest : IMDiatorRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        try
        {
            return await next();
        }
        catch (Exception)
        {
            // Simulate error handling logic
            await Task.Delay(1, cancellationToken);
            throw;
        }
    }
}

public class ErrorRequest : IMDiatorRequest<string>
{
    public string Value { get; set; } = "error";
}

public class ErrorRequestHandler : IMDiatorHandler<ErrorRequest, string>
{
    public Task<string> Handle(ErrorRequest request, CancellationToken cancellationToken)
    {
        throw new Exception("Test error");
    }
} 