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
        public string? AreaCd { get; set; }
        public int? DistributorCd { get; set; }
        public String Email { get; set; } = String.Empty;
        public String Name { get; set; } = String.Empty;
        public string Status { get; set; } = "Active";
        [JsonProperty("role_list")]
        public string Role { get; set; } = String.Empty;
        public string Title { get; set; } = string.Empty;
        public string? UserImage { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Reporter { get; set; } = String.Empty;
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string? Address { get; set; }
        public DateTime PasswordLifeTime { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public DateTime ExpiresTime { get; set; }
        public DateTime? LastLogin { get; set; }

        public User() { }
        public User(string Name, string Email, string Title, string AreaCd, string Status, string Role, string Reporter, byte[] PasswordHash, byte[] PasswordSalt, DateTime PasswordLifeTime, DateTime LastLogin)
        {
            this.Name = Name;
            this.Email = Email;
            this.Title = Title;
            this.Status = Status;
            this.Role = Role;
            this.AreaCd = AreaCd;
            this.Reporter = Reporter;
            this.PasswordHash = PasswordHash;
            this.PasswordSalt = PasswordSalt;
            this.PasswordLifeTime = PasswordLifeTime;
            this.LastLogin = LastLogin;
        }

    }
}