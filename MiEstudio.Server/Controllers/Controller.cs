using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiEstudio.Server.Data.Core;
using MiEstudio.Shared.Data.Core;
using MiEstudio.Shared.Data.Resources;
using System.Security.Claims;

namespace MiEstudio.Server.Controllers
{
    public class Controller : ControllerBase
    {
        public Guid UserId
        {
            get
            {
                if (User == null || !User.Identity.IsAuthenticated) return Guid.Empty;

                ClaimsIdentity indentity = User.Identity as ClaimsIdentity;
                string sid = indentity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
                return Guid.Parse(sid);
            }
        }

        public UserType UserType
        {
            get
            {
                if (User == null || !User.Identity.IsAuthenticated) return UserType.None;

                ClaimsIdentity indentity = User.Identity as ClaimsIdentity;
                string type = indentity.Claims.Where(c => c.Type == "Type").Select(c => c.Value).SingleOrDefault();
                if (string.IsNullOrEmpty(type)) return UserType.None;
                return (UserType)int.Parse(type);
            }
        }

        protected QueryContext<TContext> GetQueryContext<TContext>(TContext dbContext) where TContext : DbContext
        {
            QueryContext<TContext> queryContext = new QueryContext<TContext>(dbContext, Paginate.Default, UserId);
            return queryContext;
        }

        protected QueryContext<TContext> GetQueryContext<TContext, TFilter>(TContext dbContext, TFilter filter) where TContext : DbContext where TFilter : Paginate
        {
            QueryContext<TContext> queryContext = new QueryContext<TContext>(dbContext, filter, UserId);
            return queryContext;
        }
    }
}