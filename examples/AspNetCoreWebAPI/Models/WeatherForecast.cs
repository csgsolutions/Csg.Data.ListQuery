using System;
using Csg.ListQuery;

namespace AspNetCoreWebAPI;

[Sortable]
public class WeatherForecast
{
    [Filterable]
    public DateTime Date { get; set; }

    [Filterable(Description ="The temperature value in silly units")]
    public int TemperatureC { get; set; }

    [Filterable(Description ="The temperature value in 'Murica units.")]
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    [Sortable(false)]
    public string Summary { get; set; }

    public Person Author { get; set; }
}