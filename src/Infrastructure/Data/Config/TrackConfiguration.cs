using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heracles.Application.GpxTrackAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Heracles.Infrastructure.Data.Config
{
    public class TrackConfiguration : IEntityTypeConfiguration<TrackAggregate>
    {
        public void Configure(EntityTypeBuilder<TrackAggregate> builder)
        {
            builder.Property(p => p.Name)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
