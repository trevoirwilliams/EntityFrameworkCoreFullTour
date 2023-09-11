using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkCore.Data.Configurations
{
    internal class CoachConfiguration : IEntityTypeConfiguration<Coach>
    {
        public void Configure(EntityTypeBuilder<Coach> builder)
        {
            builder.HasData(
                    new Coach
                    {
                        Id = 1,
                        Name = "Jose Mourinho",
                    },
                    new Coach
                    {
                        Id = 2,
                        Name = "Pep Guardiola",
                    },
                    new Coach
                    {
                        Id = 3,
                        Name = "Trevoir Williams",
                    }
                );
        }
    }
}
