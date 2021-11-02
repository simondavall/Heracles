using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Specification.EntityFrameworkCore;
using Heracles.Domain.Interfaces;

namespace Heracles.Infrastructure.Data
{
    public class EfRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T> where T : class, IAggregateRoot
    {
        public EfRepository(GpxDbContext dbContext) : base(dbContext)
        {
        }
    }
}
