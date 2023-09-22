using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCore.Domain;

public class Match : BaseDomainModel
{
    public int HomeTeamScore { get; set; }
    public int AwayTeamScore { get; set; }
    public decimal TicketPrice { get; set; }
    public DateTime Date { get; set; }

    public virtual Team HomeTeam { get; set; }
    public int HomeTeamId { get; set; }

    public virtual Team AwayTeam { get; set; }
    public int AwayTeamId { get; set; }


}