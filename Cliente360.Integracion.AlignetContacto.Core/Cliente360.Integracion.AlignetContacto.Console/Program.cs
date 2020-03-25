using Cliente360.Config;
using Cliente360.Integracion.AlignetContacto.Core;
using System;

namespace Cliente360.Integracion.AlignetContacto.Core.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            new Operation().RunOperation("Cliente360.Integracion.AlignetContacto.Core.Console", () =>
            {
                new AlignetContactoManager().Start();
            });
            System.Console.Read();
        }
    }
}
