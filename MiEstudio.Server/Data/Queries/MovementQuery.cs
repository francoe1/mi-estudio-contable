using MiEstudio.Server.Data.Contexts;
using MiEstudio.Server.Data.Core;
using MiEstudio.Server.Data.Models;
using MiEstudio.Server.Data.Resources;
using MiEstudio.Shared.Data.Filters;
using MiEstudio.Shared.Data.Resources;
using System.Linq;

namespace MiEstudio.Server.Data.Queries
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