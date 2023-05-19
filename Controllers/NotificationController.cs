using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unilever.v1.Database.config;
using Unilever.v1.Models.Notification;

namespace Unilever.v1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UnileverDbContext _dbContext;

        public NotificationController(IConfiguration configuration, UnileverDbContext dbContext)
        {
            _config = configuration;
            _dbContext = dbContext;
        }

        [HttpPost]
        [Route("new")]
        public async Task<ActionResult<NotificationModel>> CreateNotification(NotificationDto req)
        {
            var token = HttpContext.Request.Cookies.TryGetValue("RefreshToken", out string? cookieValue);
            if (token)
            {
                var userToken = cookieValue;
                var sender = _dbContext.User.FirstOrDefault(u => u.RefreshToken == userToken);
                if (sender != null)
                {
                    NotificationModel notification = new NotificationModel();
                    notification.Title = req.Title;
                    notification.UserCd = req.UserCd;
                    notification.Message = req.Message;
                    notification.SenderName = sender.Name;

                    _dbContext.Add(notification);
                    await _dbContext.SaveChangesAsync();
                    return CreatedAtAction(nameof(CreateNotification), new { Area = req.Title }, notification);
                }
            }
            else
            {
                return Unauthorized("Login first to use this resource");
            }
            return Ok("Internal Server Error");
        }

        [HttpGet]
        [Route("get-all")]
        [Authorize(Roles = "System")]
        public ActionResult<NotificationModel> GetAllNOtification()
        {
            var token = HttpContext.Request.Cookies.TryGetValue("RefreshToken", out string? cookieValue);
            if (token)
            {
                var userToken = cookieValue;

                var isOwner = CheckAccess(userToken, "Owner");
                var isAdmin = CheckAccess(userToken, "Admin");

                if (isOwner || isAdmin)
                {
                    var notifications = _dbContext.Notification.ToList();
                    if (notifications.Count() == 0)
                    {
                        return NotFound("Can't find any notifications");
                    }
                    return Ok(notifications);
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
            return Ok("Internal Server error");
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