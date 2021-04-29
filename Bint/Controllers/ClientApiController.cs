using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Bint.Repository;
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