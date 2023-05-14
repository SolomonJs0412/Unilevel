using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "System, Sales")]
        public async Task<ActionResult<Area>> CreateNewArea(AreaDto area)
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
                    List<string> defaultValue = new List<string>();
                    defaultValue.Add("");
                    string userJs = JsonConvert.SerializeObject(defaultValue);
                    string distributorJs = JsonConvert.SerializeObject(defaultValue);
                    //check duplication area name
                    var isExistingArea = _dbContext.Area.FirstOrDefault(a => a.AreaName.ToUpper() == area.AreaName.ToUpper());
                    if (isExistingArea != null)
                    {
                        return BadRequest("Area name already exists");
                    }
                    var newArea = new Area();
                    newArea.AreaCd = area.AreaCd.ToUpper();
                    newArea.AreaName = area.AreaName.ToUpper();
                    newArea.Users = userJs;
                    newArea.Distributors = distributorJs;

                    _dbContext.Add(newArea);
                    await _dbContext.SaveChangesAsync();
                    return CreatedAtAction(nameof(CreateNewArea), new { Area = area.AreaCd }, newArea);
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
        [Route("areas")]
        [Authorize(Roles = "System, Sales")]
        public async Task<ActionResult<List<Area>>> ShowAllArea()
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
                    var areas = await _dbContext.Area.ToListAsync();
                    if (areas.Count == 0) return NotFound("No available areas");
                    return Ok(areas);
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
        [Route("areas/{id}")]
        [Authorize(Roles = "System, Sales")]
        public async Task<ActionResult<Area>> ShowAreaWithId(string id)
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
                    var area = _dbContext.Area.FirstOrDefault(a => a.AreaCd == id);
                    if (area == null) return NotFound("No available areas");
                    return Ok(area);
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
        [Route("area-user/{id}")]
        [Authorize(Roles = "System, Sales")]
        public ActionResult<List<Area>> GetAllAreaUsers(string id)
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

        [HttpPut]
        [Route("updates/{id}")]
        [Authorize(Roles = "System, Sales")]
        public async Task<ActionResult<Area>> Update(string id, [FromBody] AreaUpdateReq area)
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
                    var isExistArea = _dbContext.Area.FirstOrDefault(a => a.AreaCd == id);
                    if (isExistArea == null) return NotFound("Not found any area with this code!");
                    isExistArea.AreaName = area.AreaName;
                    isExistArea.Distributors = area.Distributors;
                    isExistArea.Users = area.Users;
                    await _dbContext.SaveChangesAsync();
                    return Ok(isExistArea);
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
        [Route("areas/{id}")]
        [Authorize(Roles = "System, Sales")]
        public async Task<ActionResult<dynamic>> Delete(string id)
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
                    var isExistArea = _dbContext.Area.FirstOrDefault(a => a.AreaCd == id);
                    if (isExistArea == null) return NotFound("Not found any area with this code!");

                    _dbContext.Area.Remove(isExistArea);
                    await _dbContext.SaveChangesAsync();
                    return Ok("Deleted successfully");
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