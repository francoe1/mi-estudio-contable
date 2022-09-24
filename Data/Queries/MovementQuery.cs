using MiEstudio.Data.Contexts;
using MiEstudio.Data.Core;
using MiEstudio.Data.Filters;
using MiEstudio.Data.Models;
using MiEstudio.Data.Resources;

namespace MiEstudio.Data.Queries
{
    public class MovementQuery : Query<MovementModel, MovementEntity, MovementResource, DataContext, MovementFilter>
    {
        public MovementQuery(QueryContext<DataContext> context) : base(context)
        {
        }

        protected override IQueryable<MovementEntity> ApplyFilters(IQueryable<MovementEntity> query, MovementFilter filter)
        {
            return query;
        }

        protected override IQueryable<MovementEntity> BaseQuery(DataContext context)
        {
            return from movement in context.Movements
                   orderby movement.Date descending
                   select new MovementEntity
                   {
                       MovementModel = movement
                   };
        }

        protected override MovementResource MapResource(MovementEntity entity)
        {
            return entity.MovementModel.ToResource();
        }
    }

    public class MovementEntity
    {
        public MovementModel MovementModel { get; set; }
    }
}