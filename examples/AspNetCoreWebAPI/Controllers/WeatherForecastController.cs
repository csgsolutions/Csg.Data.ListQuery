using System.Threading.Tasks;
using AspNetCoreWebAPI.Internal;
using Csg.ListQuery.AspNetCore.ModelBinding;
using Csg.ListQuery.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Csg.ListQuery.AspNetCore.Mvc;

namespace AspNetCoreWebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly Internal.IRepository _repo = null;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public WeatherForecastController(ILogger<WeatherForecastController> logger)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets a list of weather forecasts matching the given criteria
    /// </summary>
    /// <param name="requestModel"></param>
    /// <returns></returns>
    [HttpGet("filter")]
    [ProducesResponseType(200)]
    public Task<ActionResult<ListResponse<WeatherForecast>>> GetListOfWeather(
        [ModelBinder(typeof(ListRequestQueryStringModelBinder))]
        [SwashbuckleValidationHint(typeof(WeatherForecast), MaxRecursionDepth = 1)]
        ListRequest requestModel
    )
    {
        return this.HandleListRequestAsync<WeatherForecast>(requestModel, _repo.GetWidgetsAsync);
    }
               
}