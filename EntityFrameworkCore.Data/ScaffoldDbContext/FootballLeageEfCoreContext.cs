using System;
using System.Collections.Generic;
using EntityFrameworkCore.Data.ScaffoldModels;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Data.ScaffoldDbContext;

public partial class FootballLeageEfCoreContext : DbContext
{
    public FootballLeageEfCoreContext()
    {
    }

    public FootballLeageEfCoreContext(DbContextOptions<FootballLeageEfCoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Audit> Audits { get; set; }

    public virtual DbSet<Coach> Coaches { get; set; }

    public virtual DbSet<League> Leagues { get; set; }

    public virtual DbSet<Match> Matches { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<TeamsCoachesLeague> TeamsCoachesLeagues { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FootballLeage_efCore; Encrypt=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Coach>(entity =>
        {
            entity.HasIndex(e => new { e.Name, e.TeamId }, "IX_Coaches_Name_TeamId")
                .IsUnique()
                .HasFilter("([Name] IS NOT NULL AND [TeamId] IS NOT NULL)");

            entity.HasIndex(e => e.TeamId, "IX_Coaches_TeamId")
                .IsUnique()
                .HasFilter("([TeamId] IS NOT NULL)");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

            entity.HasOne(d => d.Team).WithOne(p => p.Coach).HasForeignKey<Coach>(d => d.TeamId);
        });

        modelBuilder.Entity<League>(entity =>
        {
            entity.HasIndex(e => e.Name, "IX_Leagues_Name");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasIndex(e => e.AwayTeamId, "IX_Matches_AwayTeamId");

            entity.HasIndex(e => e.HomeTeamId, "IX_Matches_HomeTeamId");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");
            entity.Property(e => e.TicketPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.AwayTeam).WithMany(p => p.MatchAwayTeams)
                .HasForeignKey(d => d.AwayTeamId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.HomeTeam).WithMany(p => p.MatchHomeTeams)
                .HasForeignKey(d => d.HomeTeamId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("TeamsHistory", "dbo");
                        ttb
                            .HasPeriodStart("PeriodStart")
                            .HasColumnName("PeriodStart");
                        ttb
                            .HasPeriodEnd("PeriodEnd")
                            .HasColumnName("PeriodEnd");
                    }));

            entity.HasIndex(e => e.LeagueId, "IX_Teams_LeagueId");

            entity.HasIndex(e => e.Name, "IX_Teams_Name")
                .IsUnique()
                .HasFilter("([Name] IS NOT NULL)");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

            entity.HasOne(d => d.League).WithMany(p => p.Teams)
                .HasForeignKey(d => d.LeagueId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TeamsCoachesLeague>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("TeamsCoachesLeagues");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
