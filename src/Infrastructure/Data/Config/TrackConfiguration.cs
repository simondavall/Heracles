using Heracles.Application.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Heracles.Infrastructure.Data.Config
{
    /// <summary>
    /// These configurations are picked up from an assembly search
    /// and applied to the HeraclesDbContext
    /// </summary>
    public class TrackConfiguration : IEntityTypeConfiguration<Track>
    {
        public void Configure(EntityTypeBuilder<Track> builder)
        {
            builder.Property(p => p.Name)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}