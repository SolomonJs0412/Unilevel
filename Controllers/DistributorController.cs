using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        [Authorize(Roles = "System, Sales")]
        public ActionResult<Distributor> GetAllDistributors()
        {
            List<Distributor> allDistributors = _dbContext.Distributor.ToList();
            if (allDistributors.Count() == 0) return NotFound("There are no distributions available");
            return Ok(allDistributors);
        }

        [HttpGet]
        [Route("distributor/{id}")]
        [Authorize(Roles = "System, Sales, Distributor")]
        public ActionResult<Distributor> GetAllDistributor(int id)
        {
            var distributor = _dbContext.Distributor.FirstOrDefault(d => d.DistributorCd == id);
            if (distributor == null) return NotFound("There are no distributions available");

            return Ok(distributor);
        }

        [HttpPost]
        [Route("new")]
        [Authorize(Roles = "System, Sales")]
        public async Task<ActionResult<Distributor>> CreateNewDistributor(DistributorDto req)
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
                    //check duplication area name
                    var isExistingArea = _dbContext.Distributor.FirstOrDefault(d => d.Name.ToUpper() == req.Name.ToUpper());
                    if (isExistingArea != null)
                    {
                        return BadRequest("Distributor have already exists");
                    }
                    var AreaCd = req.AreaCd;
                    var isExistArea = _dbContext.Area.SingleOrDefault(a => a.AreaCd == AreaCd);
                    if (isExistArea == null)
                    {
                        return BadRequest("Not available Area");
                    }

                    var distributors = ConvertJsonToStringList(isExistArea.Distributors);
                    distributors.Add(req.Email);
                    isExistArea.Distributors = ConvertStringToJson(distributors);

                    var newDistributor = new Distributor();
                    newDistributor.Name = req.Name;
                    newDistributor.Address = req.Address;
                    newDistributor.Phone = req.Phone;
                    newDistributor.Email = req.Email;
                    newDistributor.AreaCd = req.AreaCd;
                    newDistributor.DistributorUsers = userJs;
                    newDistributor.SaleSUPCd = req.SaleSUPCd;

                    _dbContext.Add(newDistributor);
                    await _dbContext.SaveChangesAsync();

                    return CreatedAtAction(nameof(CreateNewDistributor), new { Area = req.Name }, newDistributor);
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
        [Route("update/{id}")]
        [Authorize(Roles = "System, Sales, Distribution")]
        public async Task<ActionResult<dynamic>> UpdateDistributor(DistributorDto req, int id)
        {
            var distributor = _dbContext.Distributor.FirstOrDefault(d => d.DistributorCd == id);
            if (distributor == null) return NotFound("There are no distributions available");

            var isExistingArea = _dbContext.Distributor.FirstOrDefault(d => d.Name.ToUpper() == req.Name.ToUpper());
            if (isExistingArea != null)
            {
                return BadRequest("Distributor have already exists");
            }
            var AreaCd = req.AreaCd;
            var isExistArea = _dbContext.Area.SingleOrDefault(a => a.AreaCd == AreaCd);
            if (isExistArea == null)
            {
                return BadRequest("Not available Area");
            }

            var distributors = ConvertJsonToStringList(isExistArea.Distributors);
            distributors.Add(req.Email);
            isExistArea.Users = ConvertStringToJson(distributors);

            distributor.Name = req.Name;
            distributor.Address = req.Address;
            distributor.Phone = req.Phone;
            distributor.Email = req.Email;
            distributor.AreaCd = req.AreaCd;
            distributor.SaleSUPCd = req.SaleSUPCd;

            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(UpdateDistributor), new { Area = req.Name }, distributor);
        }

        [HttpDelete]
        [Route("delete/{id}")]
        [Authorize(Roles = "System, Sales")]
        public async Task<ActionResult<dynamic>> DeleteDistributor(int id)
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
                    var isExistDistributor = _dbContext.Distributor.FirstOrDefault(d => d.DistributorCd == id);
                    if (isExistDistributor == null) return NotFound("Not found any area with this code!");

                    _dbContext.Distributor.Remove(isExistDistributor);

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