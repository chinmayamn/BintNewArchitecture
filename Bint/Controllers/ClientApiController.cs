using Bint.Repository;
using Microsoft.AspNetCore.Mvc;
namespace Bint.Controllers
{
    [Produces("application/json")]
    [Route("api/Client")]
    public class ClientApiController : Controller
    {
        private IClientRepository _clientRepository;
        public ClientApiController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
    }
}