using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Unilever.v1.Common;
using Unilever.v1.Database.config;
using Unilever.v1.Models.Http.HttpRes.User;
using Unilever.v1.Models.Token;
using Unilever.v1.Models.UserConf;

namespace Unilever.v1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UnileverDbContext _dbContext;

        public AuthController(IConfiguration configuration, UnileverDbContext dbContext)
        {
            _config = configuration;
            _dbContext = dbContext;
        }

        public static User user = new User();

        [HttpGet]
        [Route("users")]
        public ActionResult<AllUsersRes> GetAllUsers()
        {
            List<AllUsersRes> res = new List<AllUsersRes>();
            var users = _dbContext.User.ToList();
            foreach (User u in users)
            {
                AllUsersRes user = new AllUsersRes();
                user.UserCd = u.UserCd;
                user.Name = u.Name;
                user.Email = u.Email;
                user.Title = u.Title;
                user.AreaCd = _dbContext.Area.FirstOrDefault(a => a.AreaCd == u.AreaCd).AreaName;
                user.Status = u.Status;
                user.Reporter = u.Reporter;
                res.Add(user);
            }
            return Ok(res);
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<User>> Register(UserDto req)
        {
            var AreaCd = req.AreaCd;
            var isExistArea = _dbContext.Area.SingleOrDefault(a => a.AreaCd == AreaCd);
            if (isExistArea == null)
            {
                return BadRequest("Not available Area");
            }

            var isExistTitle = _dbContext.Title.SingleOrDefault(t => t.TitleName.ToUpper() == req.Title.ToUpper());
            if (isExistTitle == null)
            {
                return BadRequest("Not available Title, create one?");
            }

            //add new account's email to Area which have this account
            var users = ConvertJsonToStringList(isExistArea.Users);
            users.Add(req.Email);
            isExistArea.Users = ConvertStringToJson(users);

            string password = GenerateRandomString();
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            //create a new account
            var NewUser = new User(req.Name, req.Email, req.Title, req.AreaCd, req.Status, req.Role, req.Reporter, passwordHash, passwordSalt);
            _dbContext.Add(NewUser);
            await _dbContext.SaveChangesAsync();


            MailerService mailer = new MailerService();
            string recipient = req.Email;
            string subject = "Welcome to our site!";
            string body = $"Your login account information: \n Email: {req.Email} \n Password: {password}";
            mailer.SendMail(recipient, subject, body);

            return CreatedAtAction(nameof(Register), new { Username = req.Name }, user);
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginModel req)
        {
            var userDb = _dbContext.User.SingleOrDefault(u => u.Email == req.username);
            if (userDb == null)
            {
                return NotFound("Not found");
            }

            if (!VerifyPassword(req.password, userDb.PasswordHash, userDb.PasswordSalt))
            {
                return BadRequest("Invalid password");
            }

            string token = CreatedToken(userDb);
            //create and set the token
            var refreshToken = RefreshTokenGenerator();
            SetRefreshToken(refreshToken, userDb);
            await _dbContext.SaveChangesAsync();
            return Ok(token);
        }

        [HttpGet]
        [Route("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var refreshToken = Request.Cookies["RefreshToken"];
            var userDb = _dbContext.User.SingleOrDefault(u => u.RefreshToken == refreshToken);

            if (user.RefreshToken.Equals(refreshToken))
            {
                return NotFound("Invalid refresh token");
            }
            else if (user.ExpiresTime > DateTime.Now)
            {
                var RefreshToken = RefreshTokenGenerator();
                SetRefreshToken(RefreshToken, userDb);
                await _dbContext.SaveChangesAsync();
                return BadRequest(RefreshToken);
            }

            string token = CreatedToken(user);
            var newRefreshToken = RefreshTokenGenerator();
            SetRefreshToken(newRefreshToken, userDb);
            await _dbContext.SaveChangesAsync();

            return Ok(newRefreshToken);
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<ActionResult<dynamic>> ResetPassword(string email)
        {
            var isExistUser = _dbContext.User.FirstOrDefault(u => u.Email.Contains(email));
            if (isExistUser == null)
            {
                return NotFound("Not found any user with this email address");
            }

            string newPassword = GenerateRandomString();
            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
            isExistUser.PasswordHash = passwordHash;
            isExistUser.PasswordSalt = passwordSalt;
            await _dbContext.SaveChangesAsync();

            MailerService mailer = new MailerService();
            string recipient = email;
            string subject = "Welcome to our site!";
            string body = $"Your user and reset password: \n Email: {email} \n Password: {newPassword}";
            mailer.SendMail(recipient, subject, body);

            return Ok("Check your email address to get reset password");
        }

        //function
        private string CreatedToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name)
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

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA1(passwordSalt))
            {
                var cmpHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                return cmpHash.SequenceEqual(passwordHash);
            }
        }

        private RefreshToken RefreshTokenGenerator()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Created = DateTime.Now,
                Expires = DateTime.Now.AddMonths(6)
            };

            return refreshToken;
        }

        private void SetRefreshToken(RefreshToken refreshToken, User crrUser)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = false,
                Expires = refreshToken.Expires
            };

            Response.Cookies.Append("RefreshToken", refreshToken.Token, cookieOptions);

            crrUser.RefreshToken = refreshToken.Token;
            crrUser.CreatedTime = refreshToken.Created;
            crrUser.ExpiresTime = refreshToken.Expires;
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
    }
}