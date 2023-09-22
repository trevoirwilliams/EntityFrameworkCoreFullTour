using Castle.Core.Resource;
using EntityFrameworkCore.Data;
using EntityFrameworkCore.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

// First we need an instance of context
var folder = Environment.SpecialFolder.LocalApplicationData;
var path = Environment.GetFolderPath(folder);
var dbPath = Path.Combine(path, "FootballLeague_EfCore.db");
var optionsBuilder = new DbContextOptionsBuilder<FootballLeagueDbContext>();
optionsBuilder.UseSqlite($"Data Source={dbPath}");
using var context = new FootballLeagueDbContext(optionsBuilder.Options);

using var sqlServerContext = new FootballLeagueSqlServerDbContext();

// Use to automatically apply all outstanding migrations
// Carefully consider before using this approach in production.
//await context.Database.MigrateAsync();

// For SQLite Users to see where the Database file gets created
//Console.WriteLine(context.DbPath);

#region Read Queries
// Select all teams
// await GetAllTeams();
//await GetAllTeamsQuerySyntax();

// Select one team
//await GetOneTeam();

// Select all record that meet a condition
//await GetFilteredTeams();

// Aggregate Methods
//await AggregateMethods();

// Grouping and Aggregating
//GroupByMethod();

// Ordering
//await OrderByMethods();

// Skip and Take - Great for Paging
//await SkipAndTake();

// Select and Projections - more precise queries
//await ProjectionsAndSelect();

// No Tracking - EF Core tracks objects that are returned by queries. This is less useful in
// disconnected applications like APIs and Web apps
//await NoTracking();

//IQueryables vs List Types
//await ListVsQueryable();
#endregion

#region Write Queries

// Use Console.WriteLine(context.ChangeTracker.DebugView.LongView); to see pending changes
// Inserting Data 
/* INSERT INTO Coaches (cols) VALUES (values) */

// Simple Insert
//await InsertOneRecord();
//await InsertOneRecordWithAudit();

// Loop Insert
//await InsertWithLoop();

// Batch Insert
//await InsertRange();

// Update Operations
//await UpdateWithTracking();
//await UpdateNoTracking();

// Delete Operations
//await DeleteRecord();

// Execute Delete
//await ExecuteDelete();

// Execute Update
//await ExecuteUpdate();

#endregion

#region Related Data
// Insert record with FK
//await InsertMatch();

// Insert Parent/Child
//await InsertTeamWithCoach();

// Insert Parent with Children
//await InsertLeagueWithTeams();

// Eager Loading Data
//await EagerLoadingData();

// Explicit Loading Data
//await ExplicitLoadingData();

// Lazy Loading
//await LazyLoadingData();

// Filtering Includes
// Get all teams and only home matches where they have scored
//await FilteringIncludes();

// Projects and Anonymous types
//await AnonymousTypesAndRelatedData();

#endregion

#region Raw SQL

// Querying a Keyless Entity
//await QueryingKeylessEntityOrView();

// Executing Raw SQL Safely
//ExecutingRawSql();


// Mixing with LINQ
//RawSqlWithLinq();

// Executing Stored Procedures
//OtherRawQueries();


#endregion

# region Additional Queries
//TemporalTableQuery();

//TransactionSupport();

// Concurrency Checks
//await ConcurrencyChecks();

// Global Query Filters
//GlobalQueryFilters();

#endregion

void GlobalQueryFilters()
{
    var leagues = context.Leagues.ToList();
    Console.WriteLine("List all leagues");
    foreach (var l in leagues)
    {
        Console.WriteLine(l.Name);
    }
    var league = context.Leagues.Find(1);
    league.IsDeleted = true;
    Console.WriteLine("Soft Delete league with the id 1");
    context.SaveChanges();

    Console.WriteLine("List all leagues - global filter ignores 'deleted' record");
    leagues = context.Leagues.ToList();
    foreach (var l in leagues)
    {
        Console.WriteLine(l.Name);
    }

    Console.WriteLine("List all leagues - global filter is ignored in the query");
    leagues = context.Leagues
        .IgnoreQueryFilters()
        .ToList();
    foreach (var l in leagues)
    {
        Console.WriteLine(l.Name);
    }
}

async Task ConcurrencyChecks()
{
    var team = context.Teams.Find(1);
    team.Name = "New Team With Concurrency Check 1";

    try
    {
        await context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException ex)
    {
        Console.WriteLine(ex.Message);
        //throw;
    }
}
void TransactionSupport()
{
    var transaction = context.Database.BeginTransaction();
    var league = new League
    {
        Name = "Testing Transactions"
    };

    context.Add(league);
    context.SaveChanges();
    transaction.CreateSavepoint("CreatedLeague");

    var coach = new Coach
    {
        Name = "Transaction Coach"
    };

    context.Add(coach);
    context.SaveChanges();

    var teams = new List<Team>
{
    new Team
    {
        Name = "Transaction Team 1",
        LeagueId = league.Id,
        CoachId = coach.Id
    }
};
    context.AddRange(teams);
    context.SaveChanges();

    try
    {
        transaction.Commit();
    }
    catch (Exception)
    {
        // Roll back entire operation
        //transaction.Rollback();

        // Rollback to specific point 
        transaction.RollbackToSavepoint("CreatedLeague");
        throw;
    }
}
void TemporalTableQuery()
{
    var teamHistory = sqlServerContext.Teams
        .TemporalAll()
        .Where(q => q.Id == 1)
        .Select(team => new
        {
            team.Name,
            ValueFrom = EF.Property<DateTime>(team, "PeriodStart"),
            ValueTo = EF.Property<DateTime>(team, "PeriodEnd"),
        })
        .ToList();

    foreach (var record in teamHistory)
    {
        Console.WriteLine($"{record.Name} | From {record.ValueFrom} | To {record.ValueTo}");
    }
}

void OtherRawQueries()
{
    // Executing Stored Procedures
    var leagueId = 1;
    var league = context.Leagues
        .FromSqlInterpolated($"EXEC dbo.StoredProcedureToGetLeagueNameHere {leagueId}");

    // Non-querying statement 
    var someName = "Random Team Name";
    context.Database.ExecuteSqlInterpolated($"UPDATE Teams SET Name = {someName}");

    int matchId = 1;
    context.Database.ExecuteSqlInterpolated($"EXEC dbo.DeleteMatch {matchId}");

    // Query Scalar or Non-Entity Type
    var leagueIds = context.Database.SqlQuery<int>($"SELECT Id FROM Leagues")
        .ToList();

    // Execute User-Defined Query
    var earliestMatch = context.GetEarliestTeamMatch(1);
}

void RawSqlWithLinq()
{
    var teamsList = context.Teams.FromSql($"SELECT * FROM Teams")
    .Where(q => q.Id == 1)
    .OrderBy(q => q.Id)
    .Include("League")
    .ToList();

    foreach (var t in teamsList)
    {
        Console.WriteLine(t);
    }
}

void ExecutingRawSql()
{
    // FromSqlRaw()
    Console.WriteLine("Enter Team Name: ");
    var teamName = Console.ReadLine();
    var teamNameParam = new SqliteParameter("teamName", teamName);
    var teams = context.Teams.FromSqlRaw($"SELECT * FROM Teams WHERE name = @teamName", teamNameParam);
    foreach (var t in teams)
    {
        Console.WriteLine(t);
    }

    // FromSql()
    teams = context.Teams.FromSql($"SELECT * FROM Teams WHERE name = {teamName}");
    foreach (var t in teams)
    {
        Console.WriteLine(t);
    }

    // FromSqlInterpolated
    teams = context.Teams.FromSqlInterpolated($"SELECT * FROM Teams WHERE name = {teamName}");
    foreach (var t in teams)
    {
        Console.WriteLine(t);
    }
}
async Task QueryingKeylessEntityOrView()
{
    var teams = await context.TeamsAndLeaguesView.ToListAsync();
    foreach (var team in teams)
    {
        Console.WriteLine($"{team.Name} - {team.LeagueName}");
    }
}

async Task AnonymousTypesAndRelatedData()
{
    var teams = await context.Teams
    .Select(q => new TeamDetails
    {
        TeamId = q.Id,
        TeamName = q.Name,
        CoachName = q.Coach.Name,
        TotalHomeGoals = q.HomeMatches.Sum(x => x.HomeTeamScore),
        TotalAwayGoals = q.AwayMatches.Sum(x => x.AwayTeamScore),
    })
    .ToListAsync();

    foreach (var team in teams)
    {
        Console.WriteLine($"{team.TeamName} - {team.CoachName} | Home Goals: {team.TotalHomeGoals} | Away Goals: {team.TotalAwayGoals}");
    }

}
async Task FilteringIncludes()
{
    //await InsertMoreMatches();
    var teams = await context.Teams
        .Include("Coach")
        .Include(q => q.HomeMatches.Where(q => q.HomeTeamScore > 0))
        .ToListAsync();

    foreach (var team in teams)
    {
        Console.WriteLine($"{team.Name} - {team.Coach.Name}");
        foreach (var match in team.HomeMatches)
        {
            Console.WriteLine($"Score - {match.HomeTeamScore}");
        }
    }
}

async Task ExplicitLoadingData()
{
    var league = await context.FindAsync<League>(1);
    if (!league.Teams.Any())
    {
        Console.WriteLine("Teams have not been loaded");
    }

    await context.Entry(league)
        .Collection(q => q.Teams)
        .LoadAsync();

    if (league.Teams.Any())
    {
        foreach (var team in league.Teams)
        {
            Console.WriteLine($"{team.Name}");
        }
    }
}

async Task LazyLoadingData()
{
    var league = await context.FindAsync<League>(1);
    foreach (var team in league.Teams)
    {
        Console.WriteLine($"{team.Name}");
    }

    // Example of N+1 Problem
    //foreach (var league in context.Leagues)
    //{
    //    foreach (var team in league.Teams)
    //    {
    //        Console.WriteLine($"{team.Name} - {team.Coach.Name}");
    //    }
    //}

}
async Task EagerLoadingData()
{
    var leagues = await context.Leagues
        //.Include("Teams") // You can also use the name of the property
        .Include(q => q.Teams)
            .ThenInclude(q => q.Coach) // Use for tables realted to the related table
        .ToListAsync();

    foreach (var league in leagues)
    {
        Console.WriteLine($"League - {league.Name}");
        foreach (var team in league.Teams)
        {
            Console.WriteLine($"{team.Name} - {team.Coach.Name}");
        }
    }
}
async Task InsertMatch()
{
    var match = new Match
    {
        AwayTeamId = 1,
        HomeTeamId = 2,
        HomeTeamScore = 0,
        AwayTeamScore = 0,
        Date = new DateTime(2023, 10, 1),
        TicketPrice = 20,
    };

    await context.AddAsync(match);
    await context.SaveChangesAsync();

    /* Incorrect reference data  - Will give error*/
    var match1 = new Match
    {
        AwayTeamId = 0,
        HomeTeamId = 0,
        HomeTeamScore = 0,
        AwayTeamScore = 0,
        Date = new DateTime(2023, 10, 1),
        TicketPrice = 20,
    };

    await context.AddAsync(match1);
    await context.SaveChangesAsync();
}
async Task InsertMoreMatches()
{
    var match1 = new Match
    {
        AwayTeamId = 2,
        HomeTeamId = 3,
        HomeTeamScore = 1,
        AwayTeamScore = 0,
        Date = new DateTime(2023, 01, 1),
        TicketPrice = 20,
    };
    var match2 = new Match
    {
        AwayTeamId = 2,
        HomeTeamId = 1,
        HomeTeamScore = 1,
        AwayTeamScore = 0,
        Date = new DateTime(2023, 01, 1),
        TicketPrice = 20,
    };
    var match3 = new Match
    {
        AwayTeamId = 1,
        HomeTeamId = 3,
        HomeTeamScore = 1,
        AwayTeamScore = 0,
        Date = new DateTime(2023, 01, 1),
        TicketPrice = 20,
    };
    var match4 = new Match
    {
        AwayTeamId = 4,
        HomeTeamId = 3,
        HomeTeamScore = 0,
        AwayTeamScore = 1,
        Date = new DateTime(2023, 01, 1),
        TicketPrice = 20,
    };
    await context.AddRangeAsync(match1, match2, match3, match4);
    await context.SaveChangesAsync();
}

async Task InsertTeamWithCoach()
{
    var team = new Team
    {
        Name = "New Team",
        Coach = new Coach
        {
            Name = "Johnson"
        },
    };
    await context.AddAsync(team);
    await context.SaveChangesAsync();
}

async Task InsertLeagueWithTeams()
{
    var league = new League
    {
        Name = "Serie A",
        Teams = new List<Team>
                {
                    new Team
                    {
                        Name = "Juventus",
                        Coach = new Coach
                        {
                            Name = "Juve Coach"
                        },
                    },
                    new Team
                    {
                        Name = "AC Milan",
                        Coach = new Coach
                        {
                            Name = "Milan Coach"
                        },
                    },
                    new Team
                    {
                        Name = "AS Roma",
                        Coach = new Coach
                        {
                            Name = "Roma Coach"
                        },
                    }
                }
    };
    await context.AddAsync(league);
    await context.SaveChangesAsync();
}
async Task ExecuteDelete()
{
    await context.Coaches.Where(q => q.Name == "Theodore Whitmore").ExecuteDeleteAsync();
}
async Task ExecuteUpdate()
{
    await context.Coaches.Where(q => q.Name == "Jose Mourinho").ExecuteUpdateAsync(set => set
    .SetProperty(prop => prop.Name, "Pep Guardiola"));
}
async Task DeleteRecord()
{
    /* DELETE FROM Coaches WHERE Id = 1 */
    var coach = await context.Coaches.FindAsync(10);
    // context.Remove(coach);
    context.Entry(coach).State = EntityState.Deleted;
    await context.SaveChangesAsync();
}

async Task UpdateWithTracking()
{
    // Find uses tracking
    var coach = await context.Coaches.FindAsync(9);
    coach.Name = "Trevoir Williams";
    await context.SaveChangesAsync();
}

async Task UpdateNoTracking()
{
    // We cannot use find with no tracking enabled, so we look for the FirstOrDefault()
    var coach1 = await context.Coaches
        .AsNoTracking()
        .FirstOrDefaultAsync(q => q.Id == 10);
    coach1.Name = "Testing No Tracking Behavior - Entity State Modified";

    // We can either call the Update() method or change the Entity State manually
    //context.Update(coach1);
    context.Entry(coach1).State = EntityState.Modified;
    await context.SaveChangesAsync();
}
async Task InsertOneRecord()
{
    var newCoach = new Coach
    {
        Name = "Jose Mourinho",
        CreatedDate = DateTime.Now,
    };
    await context.Coaches.AddAsync(newCoach);
    await context.SaveChangesAsync();
}
async Task InsertOneRecordWithAudit()
{
    var newLeague = new League
    {
        Name = "New League With Audit"
    };
    await context.AddAsync(newLeague);
    await context.SaveChangesAsync();
}
async Task InsertWithLoop()
{
    var newCoach = new Coach
    {
        Name = "Jose Mourinho",
        CreatedDate = DateTime.Now,
    };
    var newCoach1 = new Coach
    {
        Name = "Theodore Whitmore",
        CreatedDate = DateTime.Now,
    };
    List<Coach> coaches = new List<Coach>
    {
        newCoach1,
        newCoach
    };
    foreach (var coach in coaches)
    {
        await context.Coaches.AddAsync(coach);
    }
    await context.SaveChangesAsync();
}

async Task InsertRange()
{
    var newCoach = new Coach
    {
        Name = "Jose Mourinho",
        CreatedDate = DateTime.Now,
    };
    var newCoach1 = new Coach
    {
        Name = "Theodore Whitmore",
        CreatedDate = DateTime.Now,
    };
    List<Coach> coaches = new List<Coach>
    {
        newCoach1,
        newCoach
    };
    await context.Coaches.AddRangeAsync(coaches);
    await context.SaveChangesAsync();
}


async Task ListVsQueryable()
{
    Console.WriteLine("Enter '1' for Team with Id 1 or '2' for teams that contain 'F.C.'");
    var option = Convert.ToInt32(Console.ReadLine());
    List<Team> teamsAsList = new List<Team>();

    // After executing to ToListAsync, the records are loaded into memory. Any operation is then done in memory
    teamsAsList = await context.Teams.ToListAsync();
    if (option == 1)
    {
        teamsAsList = teamsAsList.Where(q => q.Id == 1).ToList();
    }
    else if (option == 2)
    {
        teamsAsList = teamsAsList.Where(q => q.Name.Contains("F.C.")).ToList();
    }

    foreach (var t in teamsAsList)
    {
        Console.WriteLine(t.Name);
    }

    // Records stay as IQueryable until the ToListAsync is executed, then the final query is performed. 
    var teamsAsQueryable = context.Teams.AsQueryable();
    if (option == 1)
    {
        teamsAsQueryable = teamsAsQueryable.Where(team => team.Id == 1);
    }

    if (option == 2)
    {
        teamsAsQueryable = teamsAsQueryable.Where(team => team.Name.Contains("F.C."));
    }

    // Actual Query execution
    teamsAsList = await teamsAsQueryable.ToListAsync();
    foreach (var t in teamsAsList)
    {
        Console.WriteLine(t.Name);
    }

}


async Task NoTracking()
{
    var teams = await context.Teams
        .AsNoTracking()
        .ToListAsync();

    foreach (var t in teams)
    {
        Console.WriteLine(t.Name);
    }
}

async Task ProjectionsAndSelect()
{
    var teams = await context.Teams
        .Select(q => new TeamInfo { Name = q.Name, Id = q.Id })
        .ToListAsync();

    foreach (var team in teams)
    {
        Console.WriteLine($"{team.Id} - {team.Name}");
    }
}

async Task SkipAndTake()
{
    var recordCount = 3;
    var page = 0;
    var next = true;
    while (next)
    {
        var teams = await context.Teams.Skip(page * recordCount).Take(recordCount).ToListAsync();
        foreach (var team in teams)
        {
            Console.WriteLine(team.Name);
        }
        Console.WriteLine("Enter 'true' for the next set of records, 'false' to exit");
        next = Convert.ToBoolean(Console.ReadLine());

        if (!next) break;
        page += 1;
    }
}

void GroupByMethod()
{
    var groupedTeams = context.Teams
    //.Where(q => q.Name == '') // Translates to a WHERE clause
    .GroupBy(q => q.CreatedDate.Date);
    //.Where(q => q.Name == '')// Translates to a HAVING clause;
    //.ToList(); // Use the executing method to load the results into memory before processing

    // EF Core can iterate through records on demand. Here, there is no executing method, but EF Core is bringing back records per iteration. 
    // This is convenient, but dangerous when you have several operations to complete per iteration. 
    // It is generally better to execute with ToList() and then operate on whatever is returned to memory. 
    foreach (var group in groupedTeams)
    {
        Console.WriteLine(group.Key);
        Console.WriteLine(group.Sum(q => q.Id));

        foreach (var team in group)
        {
            Console.WriteLine(team.Name);
        }
    }
}

async Task OrderByMethods()
{
    var orderedTeams = await context.Teams
    .OrderBy(q => q.Name)
    .ToListAsync();

    foreach (var item in orderedTeams)
    {
        Console.WriteLine(item.Name);
    }

    var descOrderedTeams = await context.Teams
        .OrderByDescending(q => q.Name)
        .ToListAsync();

    foreach (var item in descOrderedTeams)
    {
        Console.WriteLine(item.Name);
    }

    // Getting the record with a maximum value
    var maxByDescendingOrder = await context.Teams
        .OrderByDescending(q => q.Id)
        .FirstOrDefaultAsync();

    var maxBy = context.Teams.MaxBy(q => q.Id);

    // Getting the record with a minimum value
    var minByDescendingOrder = await context.Teams
        .OrderBy(q => q.Id)
        .FirstOrDefaultAsync();

    var minBy = context.Teams.MinBy(q => q.Id);

}

async Task AggregateMethods()
{
    // Count
    var numberOfTeams = await context.Teams.CountAsync();
    Console.WriteLine($"Number of Teams: {numberOfTeams}");

    var numberOfTeamsWithCondition = await context.Teams.CountAsync(q => q.Id == 1);
    Console.WriteLine($"Number of Teams with condition above: {numberOfTeamsWithCondition}");

    // Max
    var maxTeams = await context.Teams.MaxAsync(q => q.Id);
    // Min
    var minTeams = await context.Teams.MinAsync(q => q.Id);
    // Average
    var avgTeams = await context.Teams.AverageAsync(q => q.Id);
    // Sum
    var sumTeams = await context.Teams.SumAsync(q => q.Id);


}

async Task GetFilteredTeams()
{
    Console.WriteLine("Enter Search Term");
    var searchTerm = Console.ReadLine();

    var teamsFiltered = await context.Teams.Where(q => q.Name == searchTerm)
        .ToListAsync();

    foreach (var item in teamsFiltered)
    {
        Console.WriteLine(item.Name);
    }

    //var partialMatches = await context.Teams.Where(q => q.Name.Contains(searchTerm)).ToListAsync();
    // SELECT * FROM Teams WHERE Name LIKE '%F.C.%'

    var partialMatches = await context.Teams.Where(q => EF.Functions.Like(q.Name, $"%{searchTerm}%"))
        .ToListAsync();
    foreach (var item in partialMatches)
    {
        Console.WriteLine(item.Name);
    }
}

async Task GetAllTeams()
{
    // SELECT * FROM Teams
    var teams = await context.Teams.ToListAsync();

    foreach (var t in teams)
    {
        Console.WriteLine(t.Name);
    }
}

async Task GetOneTeam()
{
    //Selecting a single record -First one in the list
    var teamFirst = await context.Coaches.FirstAsync();
    if (teamFirst != null)
    {
        Console.WriteLine(teamFirst.Name);
    }
    var teamFirstOrDefault = await context.Coaches.FirstOrDefaultAsync();
    if (teamFirstOrDefault != null)
    {
        Console.WriteLine(teamFirstOrDefault.Name);
    }

    //Selecting a single record -First one in the list that meets a condition
    var teamFirstWithCondition = await context.Teams.FirstAsync(team => team.Id == 1);
    if (teamFirstWithCondition != null)
    {
        Console.WriteLine(teamFirstWithCondition.Name);
    }
    var teamFirstOrDefaultWithCondition = await context.Teams.FirstOrDefaultAsync(team => team.Id == 1);
    if (teamFirstOrDefaultWithCondition != null)
    {
        Console.WriteLine(teamFirstOrDefaultWithCondition.Name);
    }

    //Selecting a single record -Only one record should be returned, or an exception will be thrown
    var teamSingle = await context.Teams.SingleAsync();
    if (teamSingle != null)
    {
        Console.WriteLine(teamSingle.Name);
    }
    var teamSingleWithCondition = await context.Teams.SingleAsync(team => team.Id == 2);
    if (teamSingleWithCondition != null)
    {
        Console.WriteLine(teamSingleWithCondition.Name);
    }
    var SingleOrDefault = await context.Teams.SingleOrDefaultAsync(team => team.Id == 2);
    if (SingleOrDefault != null)
    {
        Console.WriteLine(SingleOrDefault.Name);
    }

    //Selecting based on Primary Key Id value
    var teamBasedOnId = await context.Teams.FindAsync(3);
    if (teamBasedOnId != null)
    {
        Console.WriteLine(teamBasedOnId.Name);
    }
}

async Task GetAllTeamsQuerySyntax()
{
    Console.WriteLine("Enter Search Term");
    var searchTerm = Console.ReadLine();

    var teams = await (from team in context.Teams
                       where EF.Functions.Like(team.Name, $"%{searchTerm}%")
                       select team)
                    .ToListAsync();
    foreach (var t in teams)
    {
        Console.WriteLine(t.Name);
    }
}

class TeamInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
}

class TeamDetails
{
    public int TeamId { get; set; }
    public string TeamName { get; set; }
    public string CoachName { get; set; }

    public int TotalHomeGoals { get; set; }
    public int TotalAwayGoals { get; set; }
}