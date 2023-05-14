using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Unilever.v1.Models.DistributorConf
{
    public class Distributor
    {
        [Key]
        public int DistributorCd { get; set; }
        public String? Name { get; set; }
        public String Address { get; set; } = String.Empty;
        public String Email { get; set; } = String.Empty;
        public String Phone { get; set; } = String.Empty;
        public string AreaCd { get; set; } = String.Empty;
        [JsonProperty("user_lst")]
        public string? DistributorUsers { get; set; }


        public Distributor() { }

        public Distributor(string Name, String Address, String Phone, String AreaCd, String Email, String DistributorUsers)
        {
            this.Name = Name;
            this.Address = Address;
            this.Phone = Phone;
            this.AreaCd = AreaCd;
            this.Email = Email;
            this.DistributorUsers = DistributorUsers;
        }
    }
}