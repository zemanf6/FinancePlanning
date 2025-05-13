using FinancePlanning.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancePlanning.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<SavedSimpleInterest> SavedSimpleInterests { get; set; }
        public DbSet<SavedCompoundInterest> SavedCompoundInterests { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<SavedSimpleInterest>(entity =>
            {
                entity.Property(e => e.Principal).HasPrecision(18, 3);
                entity.Property(e => e.InterestRate).HasPrecision(18, 3);
                entity.Property(e => e.CalculatedInterest).HasPrecision(18, 3);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 3);
            });

            builder.Entity<SavedCompoundInterest>(entity =>
            {
                entity.Property(e => e.Principal).HasPrecision(18, 3);
                entity.Property(e => e.InterestRate).HasPrecision(18, 3);
                entity.Property(e => e.CalculatedInterest).HasPrecision(18, 3);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 3);
            });
        }
    }
}
