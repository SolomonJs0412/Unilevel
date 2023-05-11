using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Unilever.v1.Common.SearchService;

namespace Unilever.v1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {

        private readonly IConfiguration _config;
        private readonly ISearchService _search;

        public SearchController(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _config = configuration;
            _search = serviceProvider.GetService<ISearchService>();
        }

        [HttpGet("search/users")]
        public async Task<ActionResult<List<object>>> Search([FromHeader] string searchTerm)
        {
            var query = "'%" + searchTerm + "%'";
            var results = await _search.SearchUser(query);
            return Ok(results);
        }
    }
}