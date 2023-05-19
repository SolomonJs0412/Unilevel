using System.Runtime.CompilerServices;
using System.Reflection.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Unilever.v1.Models.AreaConf;
using Unilever.v1.Models.RoleConf;
using Unilever.v1.Models.UserConf;
using Unilever.v1.Models.DistributorConf;
using Unilever.v1.Models.Title;
using Unilever.v1.Models.SaleSUP;
using Unilever.v1.Models.Plan;
using Unilever.v1.Models.Task;
using Unilever.v1.Models.Comment;
using Unilever.v1.Models.Notification;
using Unilever.v1.Models.Surveys;
using Unilever.v1.Models.Question;
using Unilever.v1.Models.CMS;

namespace Unilever.v1.Database.config
{
    public class UnileverDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {        //base.OnModelCreating(modelBuilder);
        }

        public UnileverDbContext(DbContextOptions<UnileverDbContext> options) : base(options) { }
        public DbSet<Area> Area { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Title> Title { get; set; }
        public DbSet<Distributor> Distributor { get; set; }
        public DbSet<SaleSUP> SaleSUP { get; set; }
        public DbSet<DistributorPlan> DistributorPlan { get; set; }
        public DbSet<Plan> Plan { get; set; }
        public DbSet<PlanDetail> PlanDetail { get; set; }
        public DbSet<TaskModel> Task { get; set; }
        public DbSet<TaskDetail> TaskDetail { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<NotificationModel> Notification { get; set; }
        public DbSet<Survey> Survey { get; set; }
        public DbSet<QuestionModel> Question { get; set; }
        public DbSet<CMSModel> CMS { get; set; }
    }
}