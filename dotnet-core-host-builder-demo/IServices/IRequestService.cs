using System.Threading.Tasks;

namespace dotnet_core_host_builder_demo
{
    public interface IRequestService
    {
        Task<string> GetResponse(string request);
    }

    
}
