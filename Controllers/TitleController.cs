using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Unilever.v1.Database.config;
using Unilever.v1.Models.Title;

namespace Unilever.v1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TitleController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UnileverDbContext _dbContext;

        public TitleController(IConfiguration configuration, UnileverDbContext dbContext)
        {
            _config = configuration;
            _dbContext = dbContext;
        }
        [HttpGet]
        [Route("all-titles")]
        public ActionResult<List<Title>> GetAllTitles()
        {
            List<Title> titles = new List<Title>();
            titles = _dbContext.Title.ToList();
            if (titles.Count() == 0) return NotFound("No titles available");
            return Ok(titles);
        }

        [HttpGet]
        [Route("titles/{id}")]
        public ActionResult<List<Title>> GetTitle([FromHeader] int id)
        {
            var isExistingTitle = _dbContext.Title.FirstOrDefault(t => t.TitleCd == id);
            if (isExistingTitle == null) return BadRequest("Title not exists");
            return Ok(isExistingTitle);
        }

        [HttpPost]
        [Route("new-title")]
        public async Task<ActionResult<dynamic>> NewTitle([FromBody] TitleDto req)
        {
            var isExistingTitle = _dbContext.Title.FirstOrDefault(t => t.TitleName.ToUpper() == req.TitleName.ToUpper());
            if (isExistingTitle != null) return BadRequest("Title already exists");
            Title title = new Title();
            title.TitleName = req.TitleName;
            title.TitleDescription = req.TitleDescription;
            _dbContext.Add(title);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(NewTitle), new { Title = req.TitleName }, title);
        }

        [HttpPost]
        [Route("titles/{cd}")]
        public async Task<ActionResult<dynamic>> UpdateTitle([FromBody] TitleDto req)
        {
            var isExistingTitle = _dbContext.Title.FirstOrDefault(t => t.TitleName.ToUpper() == req.TitleName.ToUpper());
            if (isExistingTitle == null) return BadRequest("Title is not available");
            isExistingTitle.TitleName = req.TitleName;
            isExistingTitle.TitleDescription = req.TitleDescription;
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(NewTitle), new { Title = req.TitleName }, isExistingTitle);
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public dynamic DeleteTitle([FromHeader] int id)
        {
            var isExistingTitle = _dbContext.Title.FirstOrDefault(t => t.TitleCd == id);
            if (isExistingTitle == null) return BadRequest("Title is not available");
            return Ok("Delete Title successfully");
        }
    }
}