using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PeepAcross.Engine.Manager
{
    public interface IPeepManager
    {
        Task Build();
        Task<bool> ParseArguments(string[] args);
    }
}
