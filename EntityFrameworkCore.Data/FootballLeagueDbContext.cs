using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace EntityFrameworkCore.Data
{
    public class FootballLeagueDbContext : DbContext
    {
        public FootballLeagueDbContext(DbContextOptions<FootballLeagueDbContext> options) : base(options)
        {

        }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<TeamsAndLeaguesView> TeamsAndLeaguesView { get; set; }
        public string DbPath { get; private set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.Entity<TeamsAndLeaguesView>().HasNoKey().ToView("vw_TeamsAndLeagues");
            modelBuilder.HasDbFunction(typeof(FootballLeagueDbContext).GetMethod(nameof(GetEarliestTeamMatch), new[] { typeof(int) })).HasName("fn_GetEarliestMatch");
        }

        public DateTime GetEarliestTeamMatch(int teamId) => throw new NotImplementedException();
    }

    public class FootballLeagueDbContextFactory : IDesignTimeDbContextFactory<FootballLeagueDbContext>
    {
        public FootballLeagueDbContext CreateDbContext(string[] args)
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var dbPath = Path.Combine(path, configuration.GetConnectionString("SqliteDatabaseConnectionString"));

            var optionsBuilder = new DbContextOptionsBuilder<FootballLeagueDbContext>();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            return new FootballLeagueDbContext(optionsBuilder.Options);
        }
    }
}
