using PeepAcross.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeepAcrossFull
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
