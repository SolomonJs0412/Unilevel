using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public ActionResult<List<Role>> GetAllRoles()
        {
            List<Role> roles = new List<Role>();
            roles = _dbContext.Role.ToList();
            if (roles.Count() == 0) return NotFound("No roles available");
            return Ok(roles);
        }

        [HttpGet]
        [Route("roles/{id}")]
        public ActionResult<List<Role>> GetRole([FromHeader] int id)
        {
            var isExistingRole = _dbContext.Role.FirstOrDefault(r => r.RoleCd == id);
            if (isExistingRole == null) return BadRequest("Role not exists");
            return Ok(isExistingRole);
        }

        [HttpPost]
        [Route("new-role")]
        public async Task<ActionResult<dynamic>> NewRole([FromBody] RoleDto req)
        {
            var isExistingRole = _dbContext.Role.FirstOrDefault(r => r.Title.ToUpper() == req.Title.ToUpper());
            if (isExistingRole != null) return BadRequest("Role already exists");
            Role role = new Role();
            role.Title = req.Title;
            _dbContext.Add(role);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(NewRole), new { Role = req.Title }, role);
        }

        [HttpPost]
        [Route("roles/{cd}")]
        public async Task<ActionResult<dynamic>> UpdateRole([FromBody] RoleDto req)
        {
            var isExistingRole = _dbContext.Role.FirstOrDefault(t => t.Title.ToUpper() == req.Title.ToUpper());
            if (isExistingRole == null) return BadRequest("Role is not available");
            isExistingRole.Title = req.Title;
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(UpdateRole), new { Role = req.Title }, isExistingRole);
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public dynamic DeleteRole([FromHeader] int id)
        {
            var isExistingTitle = _dbContext.Role.FirstOrDefault(t => t.RoleCd == id);
            if (isExistingTitle == null) return BadRequest("Role is not available");
            return Ok("Delete Role successfully");
        }
    }
}