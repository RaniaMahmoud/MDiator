using MDiator.SampleApp.Events;
using MDiator.SampleApp.Requests;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MDiator.SampleApp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            // ✅ Setup DI
            var services = new ServiceCollection();
            services.AddMDiator(typeof(Program).Assembly);
            var provider = services.BuildServiceProvider();
            var mediator = provider.GetRequiredService<IMediator>();

            Console.WriteLine("Testing Middleware with CreateUserCommand");
            Console.WriteLine("----------------------------------------");
            
            // Test 1: Normal request
            try
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(2000);
                var result = await mediator.Send(new CreateUserCommand { UserName = "Mostafa" }, cts.Token);
                Console.WriteLine($"✅ Success: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }

            // Test 2: Null request (ValidationMiddleware should catch this)
            try
            {
                CreateUserCommand nullCommand = null;
                var result = await mediator.Send(nullCommand);
                Console.WriteLine($"✅ Success: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error (Expected): {ex.Message}");
            }

            // Test 3: Invalid request (ErrorHandlingMiddleware should catch this)
            try
            {
                var result = await mediator.Send(new CreateUserCommand { UserName = "Error" });
                Console.WriteLine($"✅ Success: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error (Expected): {ex.Message}");
            }

            Console.WriteLine("\nTesting Middleware with OrderCreatedEvent");
            Console.WriteLine("----------------------------------------");
            
            // Test 4: Normal event
            try
            {
                await mediator.Publish(new OrderCreatedEvent { OrderId = 123 });
                Console.WriteLine("✅ Event published successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }

            // Test 5: Null event (ValidationMiddleware should catch this)
            try
            {
                OrderCreatedEvent nullEvent = null;
                await mediator.Publish(nullEvent);
                Console.WriteLine("✅ Event published successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error (Expected): {ex.Message}");
            }
        }
    }
}
