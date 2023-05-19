using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Unilever.v1.Database.config;
using Unilever.v1.Models.Http.HttpReq;
using Unilever.v1.Models.Http.HttpRes;
using Unilever.v1.Models.Plan;
using Unilever.v1.Models.UserConf;

namespace Unilever.v1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UnileverDbContext _dbContext;

        public PlanController(IConfiguration configuration, UnileverDbContext dbContext)
        {
            _config = configuration;
            _dbContext = dbContext;
        }

        [HttpPost]
        [Route("new")]
        [Authorize(Roles = "System")]
        public async Task<ActionResult<dynamic>> NewPlan([FromBody] PlanDto req)
        {
            var token = HttpContext.Request.Cookies.TryGetValue("RefreshToken", out string? cookieValue);
            if (token)
            {
                var userToken = cookieValue;

                var isOwner = CheckAccess(userToken, "Owner");

                if (isOwner)
                {
                    var distributor = _dbContext.Distributor.FirstOrDefault(d => d.DistributorCd == req.DistributorCd);
                    if (distributor == null) return NotFound("We can found nay distributor");
                    var userRQ = _dbContext.User.FirstOrDefault(u => u.RefreshToken == userToken);

                    Plan plan = new Plan();
                    PlanDetail detail = new PlanDetail();


                    plan.TimeOfPlan = req.TimeOfPlan;
                    plan.DistributorCd = req.DistributorCd;
                    plan.Description = req.Description;
                    plan.Invited = ConvertStringToJson(req.Invited);
                    plan.StartDay = req.StartDay;
                    plan.EndDay = req.EndDay;
                    plan.UserReq = (userRQ != null) ? userRQ.UserCd : 1;

                    _dbContext.Add(plan);
                    await _dbContext.SaveChangesAsync();

                    var planCd = plan.PlanId;

                    detail.PlanCd = planCd;
                    _dbContext.Add(detail);
                    await _dbContext.SaveChangesAsync();

                    return CreatedAtAction(nameof(NewPlan), new { Area = plan.Status }, plan);

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
        [Route("plans/{id}")]
        [Authorize(Roles = "System")]
        public async Task<ActionResult<dynamic>> GetSinglePlan(int id)
        {
            var token = HttpContext.Request.Cookies.TryGetValue("RefreshToken", out string? cookieValue);
            if (token)
            {
                var userToken = cookieValue;

                var isOwner = CheckAccess(userToken, "Owner");
                var isAdmin = CheckAccess(userToken, "Admin");

                if (isOwner || isAdmin)
                {
                    PlanDetailRes res = new PlanDetailRes();

                    var plan = _dbContext.Plan.FirstOrDefault(p => p.PlanId == id);
                    var planDetail = _dbContext.PlanDetail.FirstOrDefault(p => p.PlanCd == id);
                    var user = _dbContext.User.FirstOrDefault(u => u.UserCd == plan.UserReq);

                    // var userReq = user.Title;
                    var userReq = "";
                    if (user != null)
                    {
                        userReq = user.Title;
                    }

                    var users = ConvertJsonToStringList(plan.Invited);
                    List<UserDto> userList = new List<UserDto>();
                    foreach (var ele in users)
                    {
                        var us = _dbContext.User.FirstOrDefault(u => u.Email == ele);
                        if (us != null)
                        {
                            UserDto acc = new UserDto();
                            acc.Email = us.Email;
                            acc.Title = us.Title;
                            acc.Name = us.Name;
                            acc.Status = us.Status;
                            userList.Add(acc);
                        }
                    }

                    res.PlanName = planDetail.PlanName;
                    res.Title = userReq;
                    res.Description = plan.Description;
                    res.Images = planDetail.Images;
                    res.UserAdd = userList;
                    res.Status = plan.Status;
                    res.DistributorCd = plan.DistributorCd;
                    res.Images = planDetail.Images;

                    return Ok(res);
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
            return Ok("Internal error occurred");
        }



        [HttpGet]
        [Route("plans")]
        [Authorize(Roles = "System")]
        public async Task<ActionResult<dynamic>> GetAllPLan()
        {
            var token = HttpContext.Request.Cookies.TryGetValue("RefreshToken", out string? cookieValue);
            if (token)
            {
                var userToken = cookieValue;

                var isOwner = CheckAccess(userToken, "Owner");
                var isAdmin = CheckAccess(userToken, "Admin");

                if (isOwner || isAdmin)
                {
                    var plans = _dbContext.Plan.ToList();
                    if (plans.Count == 0) return NotFound("No available plan");
                    return Ok(plans);
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
        [Authorize(Roles = "System")]
        public async Task<ActionResult<dynamic>> UpdatePlan(int id, UpdatePlanReq req)
        {
            var token = HttpContext.Request.Cookies.TryGetValue("RefreshToken", out string? cookieValue);
            if (token)
            {
                var userToken = cookieValue;

                var isOwner = CheckAccess(userToken, "Owner");
                var isAdmin = CheckAccess(userToken, "Admin");

                if (isOwner || isAdmin)
                {
                    var plan = _dbContext.Plan.FirstOrDefault(p => p.PlanId == id);
                    var planDetail = _dbContext.PlanDetail.FirstOrDefault(p => p.PlanCd == plan.PlanId);

                    if (plan == null) return NotFound("Can't find any plan with your id");
                    if (planDetail == null) return NotFound("Can't find detail of this plan");

                    planDetail.Images = req.Images;
                    planDetail.PlanName = req.Name;

                    await _dbContext.SaveChangesAsync();

                    return CreatedAtAction(nameof(UpdatePlan), new { PLan = planDetail.PlanName }, plan);
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
            return Ok("Internal error occurred");
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

        private List<string> ConvertJsonToStringList(string json)
        {
            if (json == "")
            {

            }
            List<string> stringList = JsonConvert.DeserializeObject<List<string>>(json);
            return stringList;
        }

        private string ConvertStringToJson(List<string> users)
        {
            string json = JsonConvert.SerializeObject(users);
            return json;
        }
    }
}