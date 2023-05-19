using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Unilever.v1.Database.config;
using Unilever.v1.Models.Http.HttpReq;
using Unilever.v1.Models.Http.HttpRes;
using Unilever.v1.Models.Task;

namespace Unilever.v1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IConfiguration _config;
        private readonly UnileverDbContext _dbContext;

        public TaskController(IConfiguration configuration, UnileverDbContext dbContext)
        {
            _config = configuration;
            _dbContext = dbContext;
            _connectionString = configuration.GetConnectionString("DefaultCons");
        }
        [HttpGet]
        [Route("all")]
        public ActionResult<dynamic> GetAllTask()
        {
            try
            {
                var tasks = _dbContext.Task.ToList();
                if (tasks.Count() == 0) return NotFound("Not have any task");
                return Ok(tasks);
            }
            catch (Exception e)
            {
                Console.WriteLine("Get all tasks failed" + e.Message);
            }
            return Ok("Internal error");
        }

        [HttpGet]
        [Route("all-tasks-plan/{id}")]
        public ActionResult<dynamic> GetAllTaskOnPlan(int id)
        {
            try
            {
                var tasks = _dbContext.Task.Where(t => t.PlanCd == id).ToList();
                return Ok(tasks);
            }
            catch (Exception e)
            {
                Console.WriteLine("Get all tasks failed" + e.Message);
            }
            return Ok("Internal error");
        }


        [HttpGet]
        [Route("task/{id}")]
        [Authorize(Roles = "System")]
        public ActionResult<dynamic> GetTask(int id)
        {
            try
            {
                var token = HttpContext.Request.Cookies.TryGetValue("RefreshToken", out string? cookieValue);
                if (token)
                {
                    var userToken = cookieValue;

                    var isOwner = CheckAccess(userToken, "Owner");
                    var isAdmin = CheckAccess(userToken, "Admin");

                    if (isOwner || isAdmin)
                    {
                        var task = _dbContext.Task.FirstOrDefault(t => t.TaskCd == id);
                        var taskDetails = _dbContext.TaskDetail.FirstOrDefault(d => d.TaskId == id);
                        if (task != null && taskDetails != null)
                        {
                            GetTaskRes res = new GetTaskRes();
                            res.TaskName = task.TaskName;
                            res.UserCd = task.UserCd;
                            res.UserAssigned = task.UserAssigned;
                            res.TaskDescription = task.TaskDescription;
                            res.StartDay = task.StartDay;
                            res.EndDay = task.EndDay;
                            res.ResourceFile = task.Resources;
                            res.Rating = taskDetails.Rating; // TODO

                            return Ok(res);
                        }
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
            catch (Exception e)
            {
                Console.WriteLine("Get all tasks failed" + e.Message);
            }
            return Ok("Internal error");
        }

        [HttpPost]
        [Route("new")]
        [Authorize(Roles = "System")]
        public async Task<ActionResult<TaskModel>> CreateTask(TaskCreateReq req)
        {
            var token = HttpContext.Request.Cookies.TryGetValue("RefreshToken", out string? cookieValue);
            if (token)
            {
                var userToken = cookieValue;

                var isOwner = CheckAccess(userToken, "Owner");
                var isAdmin = CheckAccess(userToken, "Admin");

                if (isOwner || isAdmin)
                {
                    var plan = _dbContext.PlanDetail.FirstOrDefault(p => p.PlanCd == req.PlanCd);
                    TaskModel task = new TaskModel();
                    task.TaskName = req.TaskName;
                    task.PlanCd = req.PlanCd;
                    task.PlanName = plan.PlanName;
                    task.UserAssigned = req.UserAssigned;
                    task.TaskDescription = req.TaskDescription;
                    task.StartDay = req.StartDay;
                    task.EndDay = req.EndDay;

                    _dbContext.Add(task);
                    await _dbContext.SaveChangesAsync();

                    TaskDetail detail = new TaskDetail();
                    detail.TaskId = task.TaskCd;
                    detail.Rating = 0;

                    _dbContext.Add(detail);
                    await _dbContext.SaveChangesAsync();

                    return CreatedAtAction(nameof(CreateTask), new { Role = req.TaskName }, task);
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
        [Route("update{id}")]
        [Authorize(Roles = "System")]
        public async Task<ActionResult<TaskModel>> UpdateTask(int id, TaskUpdateReq req)
        {
            var token = HttpContext.Request.Cookies.TryGetValue("RefreshToken", out string? cookieValue);
            if (token)
            {
                var userToken = cookieValue;

                var isOwner = CheckAccess(userToken, "Owner");
                var isAdmin = CheckAccess(userToken, "Admin");

                if (isOwner || isAdmin)
                {
                    var task = _dbContext.Task.FirstOrDefault(t => t.TaskCd == id);
                    var detail = _dbContext.TaskDetail.FirstOrDefault(d => d.TaskId == id);
                    if (task != null && detail != null)
                    {
                        var listSrc = ConvertListToJson(req.Resources);
                        task.TaskName = req.TaskName;
                        task.PlanCd = req.PlanCd;
                        task.PlanName = req.PlanName;
                        task.UserAssigned = req.UserAssigned;
                        task.TaskDescription = req.TaskDescription;
                        task.StartDay = req.StartDay;
                        task.EndDay = req.EndDay;
                        task.Resources = listSrc;

                        detail.Status = req.Status;

                        await _dbContext.SaveChangesAsync();

                        return CreatedAtAction(nameof(CreateTask), new { Role = req.TaskName }, task);
                    }

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
            return Ok("Internal server error");
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
        [HttpPut]
        public async Task<List<object>> GetAllComment(int searchTerm)
        {
            var searchQuery = @"SELECT * FROM [Comment] m
                                WHERE m.TaskCd == {0};";


            string queryString = String.Format(searchQuery, searchTerm);
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(queryString, connection);

            var results = new List<object>();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var result = new ExpandoObject() as IDictionary<string, object>;
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    result[reader.GetName(i)] = reader.GetValue(i);
                }
                results.Add(result);
            }

            return results;
        }

        private List<string> ConvertJsonToStringList(string json)
        {
            if (json == "")
            {

            }
            List<string> stringList = JsonConvert.DeserializeObject<List<string>>(json);
            return stringList;
        }

        private string ConvertListToJson(List<int> src)
        {
            string json = JsonConvert.SerializeObject(src);
            return json;
        }
    }
}