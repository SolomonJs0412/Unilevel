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
            List<string> defaultValue = new List<string>();
            defaultValue.Add("");
            string userJs = JsonConvert.SerializeObject(defaultValue);
            string distributorJs = JsonConvert.SerializeObject(defaultValue);
            var newArea = new Area();
            newArea.AreaCd = area.AreaCd;
            newArea.AreaName = area.AreaName;
            newArea.Users = userJs;
            newArea.Distributors = distributorJs;


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

            var userList = ConvertJsonToStringList(isExistArea.Users);
            List<AreaUsersRes> users = new List<AreaUsersRes>();

            foreach (var user in userList)
            {
                var AreaUser = _dbContext.User.FirstOrDefault(u => u.Email == user);
                if (AreaUser == null) continue;
                AreaUsersRes res = new AreaUsersRes();
                res.UserCd = AreaUser.UserCd;
                res.Name = AreaUser.Name;
                res.Email = AreaUser.Email;
                res.Role = AreaUser.Role;
                users.Add(res);
            }

            return Ok(users);
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

        private List<string> ConvertJsonToStringList(string json)
        {
            List<string> stringList = JsonConvert.DeserializeObject<List<string>>(json);
            return stringList;
        }
    }
}