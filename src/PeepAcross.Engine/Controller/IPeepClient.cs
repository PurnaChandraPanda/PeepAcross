using PeepAcross.Engine.Models;
using System.Threading.Tasks;

namespace PeepAcross.Engine.Controller
{
    public interface IPeepClient
    {
        Task Connect(PeepParameter peepParameter);
    }
}
