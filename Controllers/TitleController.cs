using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "System, Sales")]
        [Route("all-titles")]
        public ActionResult<List<Title>> GetAllTitles()
        {
            var token = HttpContext.Request.Cookies.TryGetValue("RefreshToken", out string? cookieValue);
            if (token)
            {
                var userToken = cookieValue;

                var isOwner = CheckAccess(userToken, "Owner");
                var isAdmin = CheckAccess(userToken, "Admin");
                var isVPCD = CheckAccess(userToken, "VPCD");

                if (isOwner || isAdmin || isVPCD)
                {
                    List<Title> titles = new List<Title>();
                    titles = _dbContext.Title.ToList();
                    if (titles.Count() == 0) return NotFound("No titles available");
                    return Ok(titles);
                }
                else
                {
                    return Forbid("You doesn't have permission to access this resource");
                }
            }
            else
            {
                return Unauthorized("Login first to use this resource");
            }
        }

        [HttpGet]
        [Authorize(Roles = "System, Sales")]
        [Route("titles/{id}")]
        public ActionResult<List<Title>> GetTitle([FromHeader] int id)
        {
            var token = HttpContext.Request.Cookies.TryGetValue("RefreshToken", out string? cookieValue);
            if (token)
            {
                var userToken = cookieValue;

                var isOwner = CheckAccess(userToken, "Owner");
                var isAdmin = CheckAccess(userToken, "Admin");
                var isVPCD = CheckAccess(userToken, "VPCD");

                if (isOwner || isAdmin || isVPCD)
                {
                    var isExistingTitle = _dbContext.Title.FirstOrDefault(t => t.TitleCd == id);
                    if (isExistingTitle == null) return BadRequest("Title not exists");
                    return Ok(isExistingTitle);
                }
                else
                {
                    return Forbid("You doesn't have permission to access this resource");
                }
            }
            else
            {
                return Unauthorized("Login first to use this resource");
            }
        }

        [HttpPost]
        [Route("new-title")]
        [Authorize(Roles = "System")]
        public async Task<ActionResult<dynamic>> NewTitle([FromBody] TitleDto req)
        {
            var token = HttpContext.Request.Cookies.TryGetValue("RefreshToken", out string? cookieValue);
            if (token)
            {
                var userToken = cookieValue;

                var isOwner = CheckAccess(userToken, "Owner");
                var isAdmin = CheckAccess(userToken, "Admin");

                if (isOwner || isAdmin)
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
                else
                {
                    return Forbid("You doesn't have permission to access this resource");
                }
            }
            else
            {
                return Unauthorized("Login first to use this resource");
            }
        }

        [HttpPost]
        [Authorize(Roles = "System")]
        [Route("titles/{cd}")]
        public async Task<ActionResult<dynamic>> UpdateTitle([FromBody] TitleDto req)
        {
            var token = HttpContext.Request.Cookies.TryGetValue("RefreshToken", out string? cookieValue);
            if (token)
            {
                var userToken = cookieValue;

                var isOwner = CheckAccess(userToken, "Owner");
                var isAdmin = CheckAccess(userToken, "Admin");

                if (isOwner || isAdmin)
                {
                    var isExistingTitle = _dbContext.Title.FirstOrDefault(t => t.TitleName.ToUpper() == req.TitleName.ToUpper());
                    if (isExistingTitle == null) return BadRequest("Title is not available");
                    isExistingTitle.TitleName = req.TitleName;
                    isExistingTitle.TitleDescription = req.TitleDescription;
                    await _dbContext.SaveChangesAsync();

                    return CreatedAtAction(nameof(NewTitle), new { Title = req.TitleName }, isExistingTitle);
                }
                else
                {
                    return Forbid("You doesn't have permission to access this resource");
                }
            }
            else
            {
                return Unauthorized("Login first to use this resource");
            }
        }

        [HttpDelete]
        [Authorize(Roles = "System")]
        [Route("delete/{id}")]
        public async Task<dynamic> DeleteTitle([FromHeader] int id)
        {

            var token = HttpContext.Request.Cookies.TryGetValue("RefreshToken", out string? cookieValue);
            if (token)
            {
                var userToken = cookieValue;

                var isOwner = CheckAccess(userToken, "Owner");
                var isAdmin = CheckAccess(userToken, "Admin");
                var isVPCD = CheckAccess(userToken, "VPCD");

                if (isOwner || isAdmin || isVPCD)
                {
                    var isExistingTitle = _dbContext.Title.FirstOrDefault(t => t.TitleCd == id);
                    if (isExistingTitle == null) return BadRequest("Title is not available");

                    _dbContext.Title.Remove(isExistingTitle);
                    await _dbContext.SaveChangesAsync();
                    return Ok("Delete Title successfully");
                }
                else
                {
                    return Forbid("You doesn't have permission to access this resource");
                }
            }
            else
            {
                return Unauthorized("Login first to use this resource");
            }
        }

        [HttpPost]
        public bool CheckAccess(string token, string accessTitle)
        {
            // ...
            var user = _dbContext.User.FirstOrDefault(u => u.RefreshToken == token);

            if (user != null)
            {
                return user.Title.Contains(accessTitle);
            }
            else
            {
                return false;
            }
        }
    }
}