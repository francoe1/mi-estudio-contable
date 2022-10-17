using MiEstudio.Shared.Data.Core;
using System;

namespace MiEstudio.Server.Data.Core
{
    public class QueryContext<T>
    {
        public T Context { get; }
        public Paginate Paginate { get; }
        public string BaseUrl { get; }
        public Guid UserId { get; }

        public QueryContext(T context)
        {
            Context = context;
            Paginate = Paginate.Default;
        }

        public QueryContext(T context, Paginate paginate) : this(context)
        {
            Paginate = paginate;
        }

        public QueryContext(T context, Paginate paginate, Guid userId) : this(context, paginate)
        {
            UserId = userId;
        }

        public QueryContext(T context, Paginate paginate, Guid userId, string baseUrl) : this(context, paginate, userId)
        {
            BaseUrl = baseUrl;
        }

        public string AbsoluteUrl(string path)
        {
            return BaseUrl.TrimEnd('/') + "/" + path.TrimStart('/');
        }
    }
}