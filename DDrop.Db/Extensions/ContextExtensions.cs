using System.Data.Entity;
using System.Linq;

namespace DDrop.Db.Extensions
{
    public static class ContextExtensions
    {
        public static bool Exists<TContext, TEntity>(this TContext context, TEntity entity) where TContext : DbContext where TEntity : class
        {
            return context.Set<TEntity>().Local.Any(e => e == entity);
        }
    }
}
