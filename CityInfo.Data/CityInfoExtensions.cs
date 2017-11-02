using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CityInfo.Data.Entities;

namespace CityInfo.Data
{
    public static class CityInfoExtensions
    {
	    public static void EnsureSeedDataForContext(this CityInfoContext context)
	    {
		    if (context.Cities.Any())
		    {
			    return;
		    }

		    var cities = new List<City>
		    {
			    new City {Name = "New York", Description = "NY description",
				    PointsOfInterest = new List<PointOfInterest>
				    {
					    new PointOfInterest {Name = "Paris", Description = "Paris description"},
					    new PointOfInterest {Name = "Moscow", Description = "Moscow description"}
				    }},
			    new City {Name = "Paris", Description = "Paris description" ,
				    PointsOfInterest = new List<PointOfInterest>
				    {
					    new PointOfInterest {Name = "New York", Description = "NY description"},
					    new PointOfInterest {Name = "Moscow", Description = "Moscow description"}
				    }},
			    new City {Name = "Moscow", Description = "Moscow description",
				    PointsOfInterest = new List<PointOfInterest>
				    {
					    new PointOfInterest {Name = "Paris", Description = "Paris description"},
					    new PointOfInterest {Name = "New York", Description = "NY description"}
				    }}
		    };

		    context.Cities.AddRange(cities);

			context.SaveChanges();
	    }
    }
}
