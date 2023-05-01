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


        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<User>> Register(UserDto req)
        {
            var AreaCd = req.AreaCdFK;
            var isExist = _dbContext.Area.SingleOrDefault(a => a.AreaCd == AreaCd);
            if (isExist == null)
            {
                return BadRequest("Not avalable Area");
            }

            //add new account's email to Area which have this account
            var users = ConvertJsonToStringList(isExist.Users);
            users.Add(req.Email);
            isExist.Users = ConvertStringToJson(users);

            string password = GenerateRandomString();
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            //create a new account
            var NewUser = new User(req.Name, req.Email, req.AreaCdFK, req.Status, req.Role, req.Reporter, passwordHash, passwordSalt);
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