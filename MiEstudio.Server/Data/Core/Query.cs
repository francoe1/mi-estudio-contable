using Microsoft.EntityFrameworkCore;
using MiEstudio.Shared.Data.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MiEstudio.Server.Data.Core
{
    public abstract class Query<TModel, TEntity, TResource, Context> where Context : DbContext
    {
        protected QueryContext<Context> _context { get; private set; }
        protected IQueryable<TEntity> _query { get; set; }

        public Type ElementType => _query.ElementType;

        public Expression Expression => _query.Expression;

        public IQueryProvider Provider => _query.Provider;

        public Query(QueryContext<Context> context)
        {
            _context = context;
            _query = BaseQuery(context.Context);
        }

        protected virtual IQueryable<TEntity> ApplyPaginate()
        {
            if (_context.Paginate != null)
            {
                return _query
                    .Skip(_context.Paginate.PerPage * Math.Max(_context.Paginate.Page - 1, 0))
                    .Take(_context.Paginate.PerPage);
            }
            return _query;
        }

        protected abstract IQueryable<TEntity> BaseQuery(Context context);

        protected abstract TResource MapResource(TEntity entity);

        public async Task<List<TResource>> ToList() => (await _query.ToListAsync()).Select(x => MapResource(x)).ToList();

        public async Task<TResource> FirstOrDefault() => (await _query.Take(1).ToListAsync()).Select(x => MapResource(x)).FirstOrDefault();

        public Query<TModel, TEntity, TResource, Context> Where(Expression<Func<TEntity, bool>> predicate)
        {
            _query = _query.Where(predicate);
            return this;
        }

        public async Task<ResourceList<TResource>> ToResourceList()
        {
            int total = await _query.CountAsync();
            List<TResource> resources = (await ApplyPaginate().ToListAsync()).Select(x => MapResource(x)).ToList();

            return new ResourceList<TResource>
            {
                Data = resources,
                Page = _context.Paginate.Page,
                PerPage = _context.Paginate.PerPage,
                Total = total,
                Pages = total / _context.Paginate.PerPage,
            };
        }

        public IEnumerator<TEntity> GetEnumerator() => _query.GetEnumerator();
    }

    public abstract class Query<TModel, TEntity, TResource, Context, TFilter> :
        Query<TModel, TEntity, TResource, Context>
        where TFilter : Paginate
        where Context : DbContext
    {
        protected Query(QueryContext<Context> context) : base(context)
        {
            if (context.Paginate != null)
            {
                if (context.Paginate is TFilter filter) _query = ApplyFilters(_query, filter);
            }
        }

        protected abstract IQueryable<TEntity> ApplyFilters(IQueryable<TEntity> query, TFilter filter);
    }
}