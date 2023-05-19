using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Unilever.v1.Common;
using Unilever.v1.Database.config;
using Unilever.v1.Models.UserConf;

namespace Unilever.v1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForFirstTimeRunThisApp : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UnileverDbContext _dbContext;

        public ForFirstTimeRunThisApp(IConfiguration configuration, UnileverDbContext dbContext)
        {
            _config = configuration;
            _dbContext = dbContext;
        }


        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<User>> Register(UserDto req)
        {

            string password = GenerateRandomString();
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            var PasswordTimeLife = CalPasswordTimeLife();
            var LastLogin = DateTime.Now;
            //create a new account
            var NewUser = new User(req.Name, req.Email, req.Title, req.AreaCd, req.Status, req.Role, req.Reporter, passwordHash, passwordSalt, PasswordTimeLife, LastLogin);
            _dbContext.Add(NewUser);
            await _dbContext.SaveChangesAsync();


            MailerService mailer = new MailerService();
            string recipient = req.Email;
            string subject = "Welcome to our site!";
            string body = $"Your login account information: \n Email: {req.Email} \n Password: {password}";
            mailer.SendMail(recipient, subject, body);

            return CreatedAtAction(nameof(Register), new { Username = req.Name }, NewUser);
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

        public static string GenerateRandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 16)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string CreatedToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("Key:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMonths(6),
                signingCredentials: cred
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA1())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private dynamic CalPasswordTimeLife()
        {
            int countMonth = _config.GetValue<int>("CountMonthForPasswordLifeTime:Time");
            var now = DateTime.Now;

            var result = now.AddMonths(countMonth);
            return result;
        }
    }
}