using PeepAcross.Engine.Manager;
using PeepAcross.Engine.Util;
using System;
using System.Threading.Tasks;

namespace PeepAcross.Engine
{
    public class EntryPoint
    {
        private static readonly IPeepManager _peepManager;

        static EntryPoint()
        {
            _peepManager = new PeepManager();
        }

        public static async Task Run(string[] args)
        {
            try
            {
                if (await _peepManager.ParseArguments(args))
                {
                    await _peepManager.Build();
                }
            }
            catch (Exception ex)
            {
                await Logger.ErrorMessage(ex.ToString());
            }
        }
    }
}
