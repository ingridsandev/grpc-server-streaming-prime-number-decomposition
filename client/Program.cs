using Grpc.Core;
using Prime;
using System;
using System.Threading.Tasks;

namespace client
{
    class Program
    {
        const string target = "127.0.0.1:50051";

        static async Task Main(string[] args)
        {
            try
            {
                Channel channel = new Channel(target, ChannelCredentials.Insecure);

                await channel.ConnectAsync().ContinueWith((task) =>
                {
                    if (task.Status == TaskStatus.RanToCompletion)
                        Console.WriteLine("The client connected successfully");
                });

                var client = new PrimeNumberService.PrimeNumberServiceClient(channel);

                Console.WriteLine("type the number ");
                var number = Console.ReadLine();
                
                var response = client.PrimeNumberDecomposition(new PrimeNumberDecompositionRequest()
                {
                    Number = Convert.ToInt32(number)
                }, deadline: DateTime.UtcNow.AddMilliseconds(500));

                while (await response.ResponseStream.MoveNext())
                {
                    Console.WriteLine(response.ResponseStream.Current.PrimeFactor);
                    await Task.Delay(200);
                }

                channel.ShutdownAsync().Wait();
                Console.ReadLine();
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.DeadlineExceeded)
            {
                Console.WriteLine($"The communication exceeded the deadline");
            }
            catch (RpcException e)
            {
                Console.WriteLine($"StatusCode: {e.Status.StatusCode} | Detail: {e.Status.Detail}");
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong.");
            }
        }
    }
}
