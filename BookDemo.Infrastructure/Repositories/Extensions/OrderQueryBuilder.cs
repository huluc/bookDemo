using System.Reflection;
using System.Text;

namespace BookDemo.Infrastructure.Repositories.Extensions
{
    public static class OrderQueryBuilder
    {
        public static String CreateOrderQuery<T>(string orderByQueryString)
        {
            var orderParams = orderByQueryString.Trim().Split(',');

            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var orderQueryBuilder = new StringBuilder();

            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue;

                var propertyFromQueryName = param.Trim().Split(' ')[0];
                var objectProperty = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

                if (objectProperty is null)
                    continue;

                var direction = param.Trim()
                    .EndsWith(" desc", StringComparison.InvariantCultureIgnoreCase)
                    ? "descending"
                    : "ascending";

                orderQueryBuilder.Append($"{objectProperty.Name} {direction}, ");
            }

            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
            return orderQuery;
        }
    }
}
