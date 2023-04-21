using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Unilever.v1.Database.config;
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
           CreatePasswordHash(req.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var NewUser = new User(req.Name, req.Email, req.Status, req.Role, req.Reporter, passwordHash, passwordSalt);
            _dbContext.Add(NewUser);
            await _dbContext.SaveChangesAsync();
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
            return Ok(token);
        }




        //function
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
                expires: DateTime.Now.AddDays(23),
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
    }
    
}