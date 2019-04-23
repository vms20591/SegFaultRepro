using ChakraCore.NET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SegFaultRepro
{
    class Program
    {
        void Run()
        {
            ChakraRuntime runtime = null;
            ChakraContext context = null;
            string code = "";

            try
            {
                runtime = ChakraRuntime.Create();
                context = runtime.CreateContext(true);

                // Expose .net objects to runtime
                var service = context.ServiceNode.GetService<IJSValueConverterService>();

                service.RegisterProxyConverter<Program>(
                (binding, instance, node) =>
                {
                    binding.SetMethod<string>("echo", (msg) => { Console.WriteLine(msg); });
                });

                context.GlobalObject.WriteProperty("runtime", new Program());

                // Load the code and run
                using (Stream stream = File.OpenRead(@"app.js"))
                {
                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        code = streamReader.ReadToEnd();
                    }
                }

                context.RunScript(code);
            }
            catch (Exception exp)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0} {1}", DateTime.UtcNow.ToString("o"), exp.Message);
                Console.ResetColor();
            }
        }
        static void Main(string[] args)
        {
            var allTasks = new List<Task>();

            try
            {
                for (int i = 0; i < 10; i++)
                {
                    var task = Task.Run(() =>
                    {
                        var program = new Program();
                        program.Run();
                    });

                    allTasks.Add(task);
                }

                Task.WaitAll(allTasks.ToArray());
                Console.WriteLine("All tasks done");
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
