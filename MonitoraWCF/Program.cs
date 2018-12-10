using MonitoraWCF.Helper;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MonitoraWCF
{

    class Program
    {
        static void Run()
        {
            try
            {
                LoopTime.Run();
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static void Main(string[] args)
        {
            var service = new UsuarioService.UsuarioServiceClient();
            var Address = service.Endpoint.Address.ToString();
            string AddressZuated = "https://serviceshom.compline.com.br/SecurityService/UsuarioaService.svc";

            var check = new CheckService(Address);
            check.StatusServico();

            if(check.Status == HttpStatusCode.OK)
            {
                var tarefa = new Tarefa();
                tarefa.Run();
            }
            else
            {
                Console.WriteLine($@"Status do serviço: {check.Status}");
                Console.ReadKey();
            }
        }
    }


    public class Tarefa
    {
        private Stopwatch watch = new Stopwatch();
        private long counter = 0;

        private async void Request(int x)
        {
                Console.WriteLine($@"REQ {x}: INICIADA");
                try
                {
                    UsuarioService.UsuarioServiceClient service = new UsuarioService.UsuarioServiceClient();

                    //assigning the output value from service Response       
                    var usuario = await service.CapturarUsuarioAsync("uira.oliveira");
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                var c = Interlocked.Increment(ref counter);
                Console.WriteLine($@"REQ {x}: COMPLETADA ({((double)c / (double)watch.ElapsedMilliseconds) * 1000d}/s)");
        }

        public void Run()
        {
            //  while true; do sleep 1; netstat - an | grep 10.16.2.13 | wc - l; done;
            var parallelism = 10000;

            System.Net.ServicePointManager.DefaultConnectionLimit = parallelism;
            
            watch.Start();

            Thread.Sleep(1);
            
            Parallel.ForEach(
                Enumerable.Range(0, int.MaxValue),
                new ParallelOptions() {
                    TaskScheduler = new LimitedConcurrencyLevelTaskScheduler(parallelism),
                    MaxDegreeOfParallelism = parallelism
                },
                x => this.Request(x)
            );

            try
            {
                //creating the object of WCF service client       
                UsuarioService.UsuarioServiceClient service = new UsuarioService.UsuarioServiceClient();

                //assigning the output value from service Response       
                var usuario = service.CapturarUsuario("uira.oliveira");
                //Console.Clear();
                //Console.WriteLine();
                //assigning the output value to the lable to show user       
                Console.WriteLine(string.Format("Resposta do server {0}, Server Acessível", "https://servicescxs.compline.com.br/securityService/UsuarioService.svc"));
            }
            catch (Exception ex)
            {
                Console.Write("Não foi possivel a comunicação com o server");
                throw ex;
            }
        }
    }
}
