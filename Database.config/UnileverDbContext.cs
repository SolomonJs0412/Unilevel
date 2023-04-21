using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Unilever.v1.Models.AreaConf;
using Unilever.v1.Models.RoleConf;
using Unilever.v1.Models.UserConf;

namespace Unilever.v1.Database.config
{
    public class UnileverDbContext : DbContext
    {
        public UnileverDbContext(DbContextOptions<UnileverDbContext> options) : base(options) { }
        public DbSet<Area> Area { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
    }
}