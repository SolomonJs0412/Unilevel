using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Unilever.v1.Database.config;
using Unilever.v1.Models.Http.HttpReq;
using Unilever.v1.Models.Task;

namespace Unilever.v1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskResourceController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _config;
        private readonly UnileverDbContext _dbContext;

        public TaskResourceController(IWebHostEnvironment environment, IConfiguration configuration, UnileverDbContext dbContext)
        {
            _environment = environment;
            _config = configuration;
            _dbContext = dbContext;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(_environment.ContentRootPath, "uploads", fileName);

            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok("File uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error uploading file: {ex.Message}");
            }
        }

        //demo - i want to upload resource to S3, but i don't have enough time
        //this route just demo my code
        [HttpPost]
        [Route("new")]
        public async Task<ActionResult<dynamic>> CreatesRsc(ResourceReq req)
        {
            Resource resource = new Resource();
            resource.TaskId = req.TaskId;
            resource.TypeOfResource = req.TypeOfResource;
            resource.SourceFile = req.SourceFile;
            _dbContext.Add(resource);
            await _dbContext.SaveChangesAsync();
            return Ok(resource);
        }
    }
}