using EntityFrameworkCore.Domain;

namespace EntityFrameworkCore.Api.Models
{
    public class TeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CoachName { get; set; }
    }

    public class TeamDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CoachDto Coach { get; set; }
        public LeagueDto League { get; set; }
    }
}
