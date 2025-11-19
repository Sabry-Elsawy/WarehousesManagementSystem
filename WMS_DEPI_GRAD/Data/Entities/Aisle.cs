using NuGet.DependencyResolver;

namespace WMS_DEPI_GRAD.Data.Entities;

public class Aisle
{
    public int Id { get; set; }
    public string Name { get; set; }


    public int ZoneId { get; set; }
    public Zone Zone { get; set; }

    public ICollection<Rack> Racks { get; set; }
}
