using System;
using System.Linq;
using System.Linq.Expressions;

namespace SimpleCmsWebApi.Helpers
{
    public static class SortingHelper
    {
        private const string descEnding = " desc";

        public static IQueryable<T> Sort<T>(this IQueryable<T> query, string sortExpression)
        {
            var sortProperties = sortExpression.Split(',');

            for (var i = 0; i < sortProperties.Length; i++)
            {
                bool ascending = true;
                string propertyName = sortProperties[i].Trim();
                if (propertyName.EndsWith(descEnding))
                {
                    ascending = false;
                    propertyName = propertyName.Substring(0, propertyName.Length - descEnding.Length);
                }

                if (i == 0)
                {
                    query = query.OrderByPropertyName(propertyName, ascending);
                }
                else
                {
                    query = query.ThenByPropertyName(propertyName, ascending);
                }
            }

            return query;
        }

        public static IQueryable<T> OrderByPropertyName<T>(this IQueryable<T> query, string sortBy, bool ascending)
        {
            string method = ascending ? "OrderBy" : "OrderByDescending";
            return query.MethodOnPropertyName(method, sortBy);
        }

        public static IQueryable<T> ThenByPropertyName<T>(this IQueryable<T> query, string sortBy, bool ascending)
        {
            string method = ascending ? "ThenBy" : "ThenByDescending";
            return query.MethodOnPropertyName(method, sortBy);
        }

        private static IQueryable<T> MethodOnPropertyName<T>(this IQueryable<T> query, string methodName, string propertyName)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var prop = Expression.Property(param, propertyName);
            var exp = Expression.Lambda(prop, param);

            Type[] types = new Type[] { query.ElementType, exp.Body.Type };
            var methodCall = Expression.Call(typeof(Queryable), methodName, types, query.Expression, exp);

            return query.Provider.CreateQuery<T>(methodCall);
        }
    }
}
