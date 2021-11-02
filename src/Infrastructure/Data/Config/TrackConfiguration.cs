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
    public class TrackConfiguration : IEntityTypeConfiguration<GpxTrack>
    {
        public void Configure(EntityTypeBuilder<GpxTrack> builder)
        {
            builder.Property(p => p.Name)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
