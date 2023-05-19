using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Unilever.v1.Database.config;
using Unilever.v1.Models.Surveys;

namespace Unilever.v1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SurveyController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UnileverDbContext _dbContext;

        public SurveyController(IConfiguration configuration, UnileverDbContext dbContext)
        {
            _config = configuration;
            _dbContext = dbContext;
        }

        [HttpPost]
        [Route("new")]
        public async Task<ActionResult<dynamic>> CreateSurvey(SurveyDto req)
        {
            Survey survey = new Survey();
            survey.SurveyTitle = req.SurveyTitle;
            survey.Status = req.Status;

            _dbContext.Add(survey);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateSurvey), new { Role = req.SurveyTitle }, survey);
        }

        [HttpGet]
        [Route("survey/{id}")]
        public ActionResult<dynamic> GetSurvey(int id)
        {
            var survey = _dbContext.Survey.FirstOrDefault(s => s.SurveyCd == id);
            if (survey == null) return NotFound("No available survey");
            return Ok(survey);
        }

        [HttpGet]
        [Route("all")]
        public ActionResult<dynamic> GetAllSurvey(int id)
        {
            var surveys = _dbContext.Survey.ToList();
            if (surveys.Count() == 0) return NotFound("No available survey");
            return Ok(surveys);
        }

        [HttpPut]
        [Route("survey/{id}")]
        public ActionResult<dynamic> UpdateSurvey(int id, SurveyDto req)
        {
            var survey = _dbContext.Survey.FirstOrDefault(s => s.SurveyCd == id);
            if (survey == null) return NotFound("No available survey");

            survey.SurveyTitle = req.SurveyTitle;
            survey.Status = req.Status;
            return CreatedAtAction(nameof(UpdateSurvey), new { Role = req.SurveyTitle }, survey);
        }
    }
}