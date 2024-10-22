using System.Threading.Tasks;
using Csg.ListQuery;

namespace AspNetCoreWebAPI.Internal;

public interface IRepository
{
    Task<ListQueryResult<WeatherForecast>> GetWidgetsAsync(ListQueryDefinition queryDef);
}