using Authentication.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authentication.Infrastructure.Configuration
{
    public class SchedulerUserConfiguration : IEntityTypeConfiguration<FitCoreUser>
    {
        public void Configure(EntityTypeBuilder<FitCoreUser> builder)
        {
            builder.Property(a => a.RefreshToken)
                .HasMaxLength(3000);
        }
    }
}