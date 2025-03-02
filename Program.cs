using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TareaFinalPt2
{
    internal class Program
    {
        static async Task Main()
        {
            List<Task> Entrega = new List<Task>();
            var ciudadesEntrega = new List<string> { "Santo Domingo", "Santiago", "Bonao", "Jarabacoa", "Punta Cana" };
            string primeraRutaCompletada = null;

            var cts = new CancellationTokenSource();

            foreach (var ciudad in ciudadesEntrega)
            {
                
                Task taskCiudad = Task.Run(() => IniciarEntrega(ciudad,cts.Token));
                Entrega.Add(taskCiudad);
            }


            var primeraCompletada = await Task.WhenAny(Entrega);
            primeraRutaCompletada = ciudadesEntrega[Entrega.IndexOf(primeraCompletada)];


            await Task.WhenAll(Entrega);
            Console.WriteLine($"La primera ruta completada: {primeraRutaCompletada}");
            Console.WriteLine("Todas las rutas han sido completadas");
             
            Console.ReadKey();
        }

        static async Task IniciarEntrega(string ciudad, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine($"Iniciando entrega en {ciudad}");
                await Task.Delay(5000, cancellationToken); 

                
                var verificacion = Task.Factory.StartNew(() => VerificarPaquetes(ciudad, cancellationToken), TaskCreationOptions.AttachedToParent);
                var asignacion = Task.Factory.StartNew(() => AsignarConductor(ciudad, cancellationToken), TaskCreationOptions.AttachedToParent);

                
                await Task.WhenAll(verificacion, asignacion);

                
                await verificacion.ContinueWith(t =>
                {
                    Console.WriteLine($"Paquetes verificados en {ciudad} sin problemas");
                }, TaskContinuationOptions.OnlyOnRanToCompletion);

                await asignacion.ContinueWith(t =>
                {
                    Console.WriteLine($"Chofer asignado en {ciudad} sin problemas");
                }, TaskContinuationOptions.OnlyOnRanToCompletion);

                
                if (ciudad == "Jarabacoa")
                {
                    throw new Exception("Error: No se puede entregar en Jarabacoa: Paquete Dañado");
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la entrega de {ciudad}: {ex.Message}");
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        static void VerificarPaquetes(string ciudad, CancellationToken cancellationToken)
        {
            Task.Delay(2000).Wait(); 
            Console.WriteLine($"Verificando paquetes en {ciudad}");
        }

        static void AsignarConductor(string ciudad, CancellationToken cancellationToken)
        {
            Task.Delay(1500).Wait(); 
            Console.WriteLine($"Asignando conductor en {ciudad}");
        }
    }
}