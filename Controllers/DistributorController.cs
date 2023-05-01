using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Unilever.v1.Database.config;
using Unilever.v1.Models.DistributorConf;

namespace Unilever.v1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DistributorController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UnileverDbContext _dbContext;

        public DistributorController(IConfiguration configuration, UnileverDbContext dbContext)
        {
            _config = configuration;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("all-distributors")]
        public async Task<ActionResult<Distributor>> GetAllDistributors()
        {
            List<Distributor> allDistributors = _dbContext.Distributor.ToList();
            if (allDistributors.Count() == 0) return NotFound("There are no distributions available");
            return Ok(allDistributors);
        }
    }
}