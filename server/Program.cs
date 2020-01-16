using Google.Protobuf.Reflection;
using Grpc.Core;
using Grpc.Reflection;
using Grpc.Reflection.V1Alpha;
using Prime;
using System;
using System.Collections.Generic;
using System.IO;

namespace server
{
    class Program
    {
        const int Port = 50051;

        static void Main(string[] args)
        {
            Server server = null;

            try
            {
                server = new Server()
                {
                    Services = 
                    { 
                        PrimeNumberService.BindService(new PrimeNumberServiceImplementation()),
                        ServerReflection.BindService(new ReflectionServiceImpl(new List<ServiceDescriptor>() { PrimeNumberService.Descriptor }))
                    },
                    Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
                };

                server.Start();
                Console.WriteLine("The server is listening on the port: 50051");
                Console.ReadKey();
            }
            catch (IOException e)
            {
                Console.WriteLine($"Something went wrong - {e.Message}");
                throw;
            }
            finally
            {
                if (server != null)
                    server.ShutdownAsync().Wait();
            }
        }
    }
}
