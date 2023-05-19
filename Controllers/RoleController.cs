using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unilever.v1.Database.config;
using Unilever.v1.Models.RoleConf;

namespace Unilever.v1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UnileverDbContext _dbContext;

        public RoleController(IConfiguration configuration, UnileverDbContext dbContext)
        {
            _config = configuration;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("all-roles")]
        [Authorize(Roles = "System")]
        public ActionResult<List<Role>> GetAllRoles()
        {
            var token = HttpContext.Request.Cookies.TryGetValue("RefreshToken", out string? cookieValue);
            if (token)
            {
                var userToken = cookieValue;

                var isOwner = CheckAccess(userToken, "Owner");
                var isAdmin = CheckAccess(userToken, "Admin");

                if (isOwner || isAdmin)
                {
                    List<Role> roles = new List<Role>();
                    roles = _dbContext.Role.ToList();
                    if (roles.Count() == 0) return NotFound("No roles available");
                    return Ok(roles);
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
        [Authorize(Roles = "System")]
        [Route("roles/{id}")]
        public ActionResult<List<Role>> GetRole([FromHeader] int id)
        {
            var token = HttpContext.Request.Cookies.TryGetValue("RefreshToken", out string? cookieValue);
            if (token)
            {
                var userToken = cookieValue;

                var isOwner = CheckAccess(userToken, "Owner");
                var isAdmin = CheckAccess(userToken, "Admin");

                if (isOwner || isAdmin)
                {
                    var isExistingRole = _dbContext.Role.FirstOrDefault(r => r.RoleCd == id);
                    if (isExistingRole == null) return BadRequest("Role not exists");
                    return Ok(isExistingRole);
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
        [Route("new-role")]
        public async Task<ActionResult<dynamic>> NewRole([FromBody] RoleDto req)
        {
            var token = HttpContext.Request.Cookies.TryGetValue("RefreshToken", out string? cookieValue);
            if (token)
            {
                var userToken = cookieValue;

                var isOwner = CheckAccess(userToken, "Owner");
                var isAdmin = CheckAccess(userToken, "Admin");

                if (isOwner || isAdmin)
                {
                    var isExistingRole = _dbContext.Role.FirstOrDefault(r => r.Title.ToUpper() == req.Title.ToUpper());
                    if (isExistingRole != null) return BadRequest("Role already exists");
                    Role role = new Role();
                    role.Title = req.Title;
                    _dbContext.Add(role);
                    await _dbContext.SaveChangesAsync();

                    return CreatedAtAction(nameof(NewRole), new { Role = req.Title }, role);
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
        [Route("roles/{cd}")]
        public async Task<ActionResult<dynamic>> UpdateRole([FromBody] RoleDto req)
        {
            var token = HttpContext.Request.Cookies.TryGetValue("RefreshToken", out string? cookieValue);
            if (token)
            {
                var userToken = cookieValue;

                var isOwner = CheckAccess(userToken, "Owner");
                var isAdmin = CheckAccess(userToken, "Admin");

                if (isOwner || isAdmin)
                {
                    var isExistingRole = _dbContext.Role.FirstOrDefault(t => t.Title.ToUpper() == req.Title.ToUpper());
                    if (isExistingRole == null) return BadRequest("Role is not available");
                    isExistingRole.Title = req.Title;
                    await _dbContext.SaveChangesAsync();

                    return CreatedAtAction(nameof(UpdateRole), new { Role = req.Title }, isExistingRole);
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
        public dynamic DeleteRole([FromHeader] int id)
        {
            var token = HttpContext.Request.Cookies.TryGetValue("RefreshToken", out string? cookieValue);
            if (token)
            {
                var userToken = cookieValue;

                var isOwner = CheckAccess(userToken, "Owner");
                var isAdmin = CheckAccess(userToken, "Admin");

                if (isOwner || isAdmin)
                {
                    var isExistingTitle = _dbContext.Role.FirstOrDefault(t => t.RoleCd == id);
                    if (isExistingTitle == null) return BadRequest("Role is not available");
                    return Ok("Delete Role successfully");
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