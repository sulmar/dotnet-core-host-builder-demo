using System.Threading.Tasks;

namespace dotnet_core_host_builder_demo
{
    public class MyRequestService : IRequestService
    {
        public Task<string> Send(string content)
        {
            return Task.FromResult($"{content} OK");
        }
    }

}
