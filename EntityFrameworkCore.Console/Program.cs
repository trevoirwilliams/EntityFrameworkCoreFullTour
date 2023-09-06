using EntityFrameworkCore.Data;
using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;

// First we need an instance of context
using var context = new FootballLeagueDbContext();

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

