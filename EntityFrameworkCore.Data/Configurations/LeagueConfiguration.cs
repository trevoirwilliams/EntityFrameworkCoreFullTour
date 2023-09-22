using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkCore.Data.Configurations
{
    internal class LeagueConfiguration : IEntityTypeConfiguration<League>
    {
        public void Configure(EntityTypeBuilder<League> builder)
        {
            builder.HasQueryFilter(x => x.IsDeleted == false);

            builder.HasData(
                    new League
                    {
                        Id = 1,
                        Name = "Jamaica Premiere League",
                    },
                    new League
                    {
                        Id = 2,
                        Name = "English Premiere League",
                    },
                    new League
                    {
                        Id = 3,
                        Name = "La Liga",
                    }
                );
        }
    }
}
