using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PeepAcross.Engine.Util
{
    class ArgumentParser
    {
        public static Task<string> OnUsage()
        {
            StringBuilder sbuilder = new StringBuilder();
            sbuilder.AppendLine("Usage: PeepAcross-Full [-sql]");
            sbuilder.AppendLine("                       [-httpclient]");

            return Task.FromResult(sbuilder.ToString());
        }

        public static Task<Tuple<string, IDictionary<string, string>>> Parse(string[] arguments)
        {
            var parseResult = new ConcurrentDictionary<string, string>();

            for (int i = 1; i < arguments.Length - 1; i = i + 2)
            {
                parseResult.TryAdd(arguments[i].TrimStart('-'), arguments[i + 1]);
            }

            var parsedResult = new Tuple<string, IDictionary<string, string>>(
                                        arguments[0].TrimStart('-'), 
                                        parseResult);

            return Task.FromResult(parsedResult);
        }
    }
}
