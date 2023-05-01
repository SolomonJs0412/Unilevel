using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Unilever.v1.Database.config;
using Unilever.v1.Models.AreaConf;
using Unilever.v1.Models.Http.HttpReq;
using Unilever.v1.Models.Http.HttpRes;
using Unilever.v1.Models.UserConf;

namespace Unilever.v1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AreaController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UnileverDbContext _dbContext;

        public AreaController(IConfiguration configuration, UnileverDbContext dbContext)
        {
            _config = configuration;
            _dbContext = dbContext;
        }

        [HttpPost]
        [Route("news")]
        public async Task<ActionResult<Area>> CreateNewArea(AreaDto area)
        {
            List<string> users = new List<string>();
            users.Add(area.Users);
            string userJs = JsonConvert.SerializeObject(users);
            var newArea = new Area();
            newArea.AreaCd = area.AreaCd;
            newArea.AreaName = area.AreaName;
            newArea.Users = userJs;
            newArea.Distributors = area.Distributors;


            _dbContext.Add(newArea);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateNewArea), new { Area = area.AreaCd }, newArea);
        }

        [HttpGet]
        [Route("areas")]
        public async Task<ActionResult<List<Area>>> ShowAllArea()
        {
            var areas = await _dbContext.Area.ToListAsync();
            if (areas.Count == 0) return NotFound("No available areas");
            return Ok(areas);
        }

        [HttpGet]
        [Route("areas/{id}")]
        public async Task<ActionResult<Area>> ShowAreaWithId(string id)
        {
            var area = _dbContext.Area.FirstOrDefault(a => a.AreaCd == id);
            if (area == null) return NotFound("No available areas");
            return Ok(area);
        }

        [HttpGet]
        [Route("area-user/{id}")]
        public async Task<ActionResult<List<Area>>> GetAllAreaUsers(string id)
        {
            var isExistArea = _dbContext.Area.FirstOrDefault(a => a.AreaCd == id);
            if (isExistArea == null) return NotFound("Can't found any area with this code");

            var AreaUsers = await _dbContext.User.Where(u => u.AreaCdFK == id).ToListAsync();
            if (AreaUsers.Count == 0) return NotFound("No one in that area");

            List<AreaUsersRes> res = new List<AreaUsersRes>();
            foreach (User user in AreaUsers)
            {
                AreaUsersRes filterUser = new AreaUsersRes();
                filterUser.AreaCdFK = user.AreaCdFK;
                filterUser.UserCd = user.UserCd;
                filterUser.Email = user.Email;
                filterUser.Name = user.Name;
                filterUser.Status = user.Status;
                filterUser.Role = user.Role;
                filterUser.Reporter = user.Reporter;
                res.Add(filterUser);
            }

            return Ok(res);
        }

        [HttpPut]
        [Route("updates/{id}")]
        public async Task<ActionResult<Area>> Update(string id, [FromBody] AreaUpdateReq area)
        {
            var isExistArea = _dbContext.Area.FirstOrDefault(a => a.AreaCd == id);
            if (isExistArea == null) return NotFound("Not found any area with this code!");
            isExistArea.AreaName = area.AreaName;
            isExistArea.Distributors = area.Distributors;
            isExistArea.Users = area.Users;
            await _dbContext.SaveChangesAsync();
            return Ok(isExistArea);
        }

        [HttpDelete]
        [Route("areas/{id}")]
        public async Task<ActionResult<dynamic>> Delete(string id)
        {
            var isExistArea = _dbContext.Area.FirstOrDefault(a => a.AreaCd == id);
            if (isExistArea == null) return NotFound("Not found any area with this code!");

            _dbContext.Area.Remove(isExistArea);
            await _dbContext.SaveChangesAsync();
            return Ok("Deleted successfully");
        }




        private string ConvertStringToJson(List<string> users)
        {
            string json = JsonConvert.SerializeObject(users);
            return json;
        }
    }
}