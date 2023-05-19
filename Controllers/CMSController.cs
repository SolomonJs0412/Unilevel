using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Unilever.v1.Database.config;
using Unilever.v1.Models.CMS;
using Unilever.v1.Models.Http.HttpReq;

namespace Unilever.v1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CMSController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UnileverDbContext _dbContext;

        public CMSController(IConfiguration configuration, UnileverDbContext dbContext)
        {
            _config = configuration;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("all")]
        public ActionResult<List<CMSModel>> GetAll()
        {
            var cms = _dbContext.CMS.ToList();
            if (cms.Count == 0) return NotFound("Can not find nay CMS");
            return Ok(cms);
        }

        [HttpGet]
        [Route("cms/{id}")]
        public ActionResult<CMSModel> GetCMS(int id)
        {
            var cms = _dbContext.CMS.FirstOrDefault(c => c.CMSCd == id);
            if (cms == null) return NotFound("Can find any cms with this id");
            return Ok(cms);
        }

        [HttpPost]
        [Route("new")]
        public async Task<ActionResult<CMSModel>> CreateCMS(CMSCreateREq req)
        {
            var token = HttpContext.Request.Cookies.TryGetValue("RefreshToken", out string? cookieValue);
            if (token)
            {
                var userToken = cookieValue;
                var user = _dbContext.User.FirstOrDefault(u => u.RefreshToken == userToken);
                if (user != null)
                {
                    CMSModel cms = new CMSModel();
                    cms.Title = req.Title;
                    cms.HyperText = req.HyperText;
                    cms.BannerURL = req.BannerURL;
                    cms.Description = req.Description;
                    cms.UserCd = user.UserCd;
                    cms.Created = DateTime.Now;

                    _dbContext.Add(cms);
                    await _dbContext.SaveChangesAsync();

                    return CreatedAtAction(nameof(CreateCMS), new { Role = req.Title }, cms);
                }

            }
            else
            {
                return Unauthorized("Login first to use this resource");
            }
            return Ok("Internal Server Error");
        }

        [HttpPost]
        [Route("update/{id}")]
        public async Task<ActionResult<CMSModel>> UpdateCMS(int id, CMSDto req)
        {
            var token = HttpContext.Request.Cookies.TryGetValue("RefreshToken", out string? cookieValue);
            if (token)
            {
                var userToken = cookieValue;
                var cms = _dbContext.CMS.FirstOrDefault(c => c.CMSCd == id);
                if (cms != null)
                {
                    cms.Title = req.Title;
                    cms.HyperText = req.HyperText;
                    cms.BannerURL = req.BannerURL;
                    cms.Description = req.Description;
                    cms.Status = req.Status;
                    cms.Created = DateTime.Now;

                    await _dbContext.SaveChangesAsync();

                    return CreatedAtAction(nameof(CreateCMS), new { Role = req.Title }, cms);
                }

            }
            else
            {
                return Unauthorized("Login first to use this resource");
            }
            return Ok("Internal Server Error");
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<ActionResult<dynamic>> DeleteCMS(int id)
        {
            var cms = _dbContext.CMS.FirstOrDefault(c => c.CMSCd == id);
            if (cms == null) return NotFound("Can find any cms with this id");
            _dbContext.CMS.Remove(cms);
            await _dbContext.SaveChangesAsync();
            return Ok("Delete Title successfully");
        }
    }
}