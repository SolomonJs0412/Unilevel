using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Unilever.v1.Database.config;
using Unilever.v1.Models.Http.HttpReq;
using Unilever.v1.Models.Question;

namespace Unilever.v1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UnileverDbContext _dbContext;

        public QuestionController(IConfiguration configuration, UnileverDbContext dbContext)
        {
            _config = configuration;
            _dbContext = dbContext;
        }

        [HttpPost]
        [Route("new")]
        public async Task<ActionResult<List<QuestionCreateReq>>> NewQuestions(List<QuestionCreateReq> req, int surveyCd)
        {
            try
            {
                foreach (var r in req)
                {
                    QuestionModel question = new QuestionModel();
                    question.QuestionTitle = r.Title;
                    question.Answers = ConvertStringToJson(r.Answer);
                    question.SurveyCd = surveyCd;
                    question.CorrectAnswer = ConvertStringToJson(r.CorrectAnswer);
                    question.isHasMoreCorrectAnswer = r.isHasMoreCorrectAnswer;
                    _dbContext.Add(question);

                    await _dbContext.SaveChangesAsync();
                }
                return StatusCode(201, "Success");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(502, "");
            }
        }

        [HttpGet]
        [Route("get-by-survey-id/{id}")]
        public async Task<ActionResult<List<QuestionModel>>> GetQuestionBySurvey(int id)
        {
            try
            {
                var questions = _dbContext.Question.Where(q => q.SurveyCd == id).ToList();
                if (questions.Count() == 0) return NotFound("Can find any question");
                return Ok(questions);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(502, "");
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