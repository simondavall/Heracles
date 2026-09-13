using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Heracles.Application.Interfaces;
using Heracles.Domain;
using Heracles.Domain.Interfaces;

namespace Heracles.Infrastructure.Data
{
    /// <summary>
    /// "There's some repetition here - couldn't we have some the sync methods call the async?"
    /// https://blogs.msdn.microsoft.com/pfxteam/2012/04/13/should-i-expose-synchronous-wrappers-for-asynchronous-methods/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public abstract class EfRepository<T, TId> : IAsyncRepository<T, TId> where T : BaseEntity<TId>, IAggregateRoot
    {
        private readonly IDbContextFactory<GpxDbContext> _contextFactory;
        
        protected EfRepository(IDbContextFactory<GpxDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public virtual async Task<T> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync(cancellationToken);
            
            var keyValues = new object[] { id };
            return await dbContext.Set<T>().FindAsync(keyValues, cancellationToken);
        }

        public async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync(cancellationToken);
            return await dbContext.Set<T>().ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var specificationResult = ApplySpecification(spec,  dbContext);
            return await specificationResult.ToListAsync(cancellationToken);
        }

        public async Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var specificationResult = ApplySpecification(spec, dbContext);
            return await specificationResult.CountAsync(cancellationToken);
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync(cancellationToken);
            await dbContext.Set<T>().AddAsync(entity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync(cancellationToken);
            dbContext.Entry(entity).State = EntityState.Modified;
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync(cancellationToken);
            dbContext.Set<T>().Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<T> FirstAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var specificationResult = ApplySpecification(spec, dbContext);
            return await specificationResult.FirstAsync(cancellationToken);
        }

        public async Task<T> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var specificationResult = ApplySpecification(spec, dbContext);
            return await specificationResult.FirstOrDefaultAsync(cancellationToken);
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec, GpxDbContext dbContext)
        {
            var evaluator = new SpecificationEvaluator();
            return evaluator.GetQuery(dbContext.Set<T>().AsQueryable(), spec);
        }
    }
}