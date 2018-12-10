using System;
using System.Timers;

namespace MonitoraWCF.Helper
{
    public class LoopTime
    {
        protected static Timer _timer;
        private static int _minutos;
        private static int contador = 0;

        public static void Run()
        {
            
                _minutos = 1;
            Console.WriteLine("..:: Verificando disponibilidade do endpoint iniciando em  {0} com intervalos de {1} minutos::..", DateTime.Now, _minutos);

            _timer = new Timer();
            _timer.AutoReset = false;
            _timer.Interval = (_minutos * 1) * 1000; // Intervalo em milésimos
            _timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            _timer.Enabled = true;
            _timer.Start();
        }

        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            contador++;
            _timer.Stop();
            Console.WriteLine("..:: Tentativa:  {0:00000} ::..", contador);

            DateTime nowTime = DateTime.Now;

            //Tarefa.run();

            _timer.Start();
        }

    }
}
