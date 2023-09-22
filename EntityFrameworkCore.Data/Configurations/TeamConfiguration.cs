using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Data.Configurations
{
    internal class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.HasIndex(q => q.Name)
                .IsUnique();

            // Composite Key Configuration
            //builder.HasIndex(q => new { q.CoachId, q.LeagueId }).IsUnique();

            // For SQL Server Only
            //builder.Property(q => q.Version)
            //    .IsRowVersion();

            builder.Property(q => q.Version)
                .IsConcurrencyToken();

            builder.Property(q => q.Name).HasMaxLength(100).IsRequired();


            builder.ToTable("Teams", b => b.IsTemporal());

            builder.HasMany(m => m.HomeMatches)
                .WithOne(q => q.HomeTeam)
                .HasForeignKey(q => q.HomeTeamId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(m => m.AwayMatches)
                .WithOne(q => q.AwayTeam)
                .HasForeignKey(q => q.AwayTeamId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                    new Team
                    {
                        Id = 1,
                        Name = "Tivoli Gardens F.C.",
                        CreatedDate = new DateTime(2023, 09, 01),
                        LeagueId = 1,
                        CoachId = 1
                    },
                    new Team
                    {
                        Id = 2,
                        Name = "Waterhouse F.C.",
                        CreatedDate = new DateTime(2023,09,01),
                        LeagueId = 1,
                        CoachId = 2
                    },
                    new Team
                    {
                        Id = 3,
                        Name = "Humble Lions F.C.",
                        CreatedDate = new DateTime(2023, 09, 01),
                        LeagueId = 1,
                        CoachId = 3
                    }
                );
        }
    }
}
