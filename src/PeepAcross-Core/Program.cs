using PeepAcross.Engine;
using System;
using System.Threading.Tasks;

namespace PeepAcrossCore
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await EntryPoint.Run(args);

            Console.ReadKey();
        }
    }
}
