using MiEstudio.Server.Data.Contexts;
using MiEstudio.Server.Data.Core;
using MiEstudio.Server.Data.Models;
using MiEstudio.Shared.Data.Filters;
using MiEstudio.Shared.Data.Resources;
using Newtonsoft.Json;
using System.Linq;

namespace MiEstudio.Server.Data.Queries
{
    public class ClientQuery : Query<ClientModel, ClientEntity, ClientResource, DataContext, ClientFilter>
    {
        public ClientQuery(QueryContext<DataContext> context) : base(context)
        {
        }

        protected override IQueryable<ClientEntity> ApplyFilters(IQueryable<ClientEntity> query, ClientFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Search)) query = query.Where(x => x.ClientModel.Name.ToLower().Contains(filter.Search.ToLower()));

            switch (filter.Sort)
            {
                case ClientSort.Name:
                    query = query.OrderBy(x => x.ClientModel.Name);
                    break;

                case ClientSort.City:
                    query = query.OrderBy(x => x.ClientModel.City);
                    break;

                case ClientSort.Type:
                    query = query.OrderBy(x => x.ClientModel.Type);
                    break;

                case ClientSort.Balance:
                    query = query.OrderByDescending(x => (int)x.LastMovementModel.Balance);
                    break;

                case ClientSort.Expense:
                    query = query.OrderByDescending(x => (int)x.ClientExpenseModel.Value);
                    break;
            }

            return query;
        }

        protected override IQueryable<ClientEntity> BaseQuery(DataContext context)
        {
            return from client in context.Clients
                   select new ClientEntity
                   {
                       ClientModel = client,
                       ClientExpenseModel = (from expense in context.ClientsExpense
                                             where expense.ClientId == client.Id
                                             orderby expense.Date descending
                                             select expense).FirstOrDefault(),
                       LastMovementModel = (from movement in context.Movements
                                            where movement.ClientId == client.Id
                                            orderby movement.Date descending
                                            select movement).FirstOrDefault()
                   };
        }

        protected override ClientResource MapResource(ClientEntity entity)
        {
            ClientResource resource = JsonConvert.DeserializeObject<ClientResource>(JsonConvert.SerializeObject(entity.ClientModel));
            if (entity.ClientExpenseModel != null) resource.Expense = entity.ClientExpenseModel.Value;
            if (entity.LastMovementModel != null) resource.Balance = entity.LastMovementModel.Balance;
            return resource;
        }
    }

    public class ClientEntity
    {
        public ClientModel ClientModel { get; set; }
        public ClientExpenseModel ClientExpenseModel { get; set; }
        public MovementModel LastMovementModel { get; set; }
    }
}