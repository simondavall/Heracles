using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Heracles.Application.Interfaces;
using Heracles.Application.Services.Import;
using Heracles.Application.TrackAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Heracles.Infrastructure.Data
{
    public class TrackRepository : EfRepository<Track, Guid>, ITrackRepository
    {
        private readonly IServiceProvider _services;

        public TrackRepository(GpxDbContext dbContext, IServiceProvider services) : base(dbContext)
        {
            _services = services;
        }

        public async Task SaveImportedFilesAsync(ImportFilesResult importFilesResult, CancellationToken cancellationToken)
        {
            await using (var dbContext = _services.GetRequiredService<GpxDbContext>())
            {
                await using (var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken))
                {
                    try
                    {
                        var bulkConfig = new BulkConfig()
                        {
                            UnderlyingConnection = GetConnection,
                            UnderlyingTransaction = GetTransaction
                        };

                        await dbContext.BulkInsertAsync(importFilesResult.Tracks, bulkConfig, cancellationToken: cancellationToken);
                        await dbContext.BulkInsertAsync(importFilesResult.TrackSegments, bulkConfig, cancellationToken: cancellationToken);
                        await dbContext.BulkInsertAsync(importFilesResult.TrackPoints, bulkConfig, cancellationToken: cancellationToken);
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        throw;
                    }

                    await transaction.CommitAsync(cancellationToken);
                };
            }
        }

        public async Task<IList<string>> GetExistingTracksAsync()
        {
            return await DbContext.Tracks.Select(x => x.Name).ToListAsync();
        }

        private static DbConnection GetConnection(DbConnection connection)
        {
            return connection;
        }
        private static DbTransaction GetTransaction(DbTransaction transaction)
        {
            return transaction;
        }
    }
}
