using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Unilever.v1.Database.config;
using Unilever.v1.Models.Comment;

namespace Unilever.v1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UnileverDbContext _dbContext;

        public CommentController(IConfiguration configuration, UnileverDbContext dbContext)
        {
            _config = configuration;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("all")]
        public ActionResult<dynamic> GetAllComments()
        {
            var comments = _dbContext.Comment.ToList();
            if (comments.Count() == 0) return NotFound("Not have any comments");
            return Ok(comments);
        }

        [HttpPost]
        [Route("new")]
        public async Task<ActionResult<dynamic>> CreateComment(CommentDto req)
        {
            var token = HttpContext.Request.Cookies.TryGetValue("RefreshToken", out string? cookieValue);
            if (token)
            {
                var userToken = cookieValue;
                var userComment = _dbContext.User.FirstOrDefault(u => u.RefreshToken == userToken);
                if (userComment != null)
                {
                    Comment comment = new Comment();
                    comment.CommentContent = req.CommentContent;
                    comment.UserCommentId = userComment.UserCd;
                    comment.Rate = req.Rate;
                    comment.TaskCd = req.TaskCd;

                    _dbContext.Add(comment);
                    await _dbContext.SaveChangesAsync();
                    return CreatedAtAction(nameof(CreateComment), new { Comment = req.UserCommentId }, comment);
                }
            }
            else
            {
                return Unauthorized("Login first to use this resource");
            }
            return Ok("Internal server error");
        }
    }
}