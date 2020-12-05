using PeepAcross.Engine.Models;
using System.Threading.Tasks;

namespace PeepAcross.Engine.Controller
{
    class PeepController
    {
        private IPeepClient _peepClient;

        public PeepController(IPeepClient peepClient)
        {
            _peepClient = peepClient;
        }

        public async Task Invoke(PeepParameter peepParameter)
        {
            await _peepClient.Connect(peepParameter);
        }
    }
}
