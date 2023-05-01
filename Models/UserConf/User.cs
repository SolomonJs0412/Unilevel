using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Unilever.v1.Models.UserConf
{
    public class User
    {
        [Key]
        public int UserCd { get; set; }
        public string AreaCdFK { get; set; }
        public String Email { get; set; } = String.Empty;
        public String Name { get; set; } = String.Empty;
        public string Status { get; set; } = "Active";
        [JsonProperty("role_list")]
        public string? Role { get; set; }
        public string? UserImage { get; set; }
        public string? Reporter { get; set; } = String.Empty;
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public DateTime ExpiresTime { get; set; }
        public string? VerificationToken { get; set; }
        public DateTime? VerifyAt { get; set; }
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTokenExpired { get; set; }

        public User() { }
        public User(string Name, string Email, string AreaCdFK, string Status, string Role, string Reporter, byte[] PasswordHash, byte[] PasswordSalt)
        {
            this.Name = Name;
            this.Email = Email;
            this.Status = Status;
            this.Role = Role;
            this.AreaCdFK = AreaCdFK;
            this.Reporter = Reporter;
            this.PasswordHash = PasswordHash;
            this.PasswordSalt = PasswordSalt;
        }

    }
}