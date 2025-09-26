using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class GovernanceDbContext(DbContextOptions<GovernanceDbContext> options) : DbContext(options)
    {
        public DbSet<ApprovalJob> ApprovalJobs => Set<ApprovalJob>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<ApprovalJob>().ToTable("ApprovalJobs");

            mb.Entity<ApprovalJob>().Ignore(j => j.RequestedByUser);
            mb.Entity<ApprovalJob>().Ignore(j => j.DecidedByUser);
        }
    }
}
