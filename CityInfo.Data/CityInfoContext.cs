using CityInfo.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.Data
{
    public class CityInfoContext : DbContext
    {
	    public CityInfoContext(DbContextOptions options) : base(options)
	    {
		    Database.Migrate();
	    }
		public DbSet<City> Cities { get; set; }
	    public DbSet<PointOfInterest> PointsOfInterest { get; set; }
		
    }
}
