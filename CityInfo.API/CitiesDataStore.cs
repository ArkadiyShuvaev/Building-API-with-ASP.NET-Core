using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Models;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        private static readonly CitiesDataStore _current = new CitiesDataStore();
        public static CitiesDataStore Current => _current;

        public List<CityDto> Cities { get; }
        public CitiesDataStore()
        {
            Cities = new List<CityDto>
            {
                new CityDto {Id = 1, Name = "New York", Description = "NY description",
                    PointsOfInterest = new List<PointOfInterestDto>
                    {
                        new PointOfInterestDto {Id = 2, Name = "Paris", Description = "Paris description"},
                        new PointOfInterestDto {Id = 3, Name = "Moscow", Description = "Moscow description"}
                    }},
                new CityDto {Id = 2, Name = "Paris", Description = "Paris description" ,
                    PointsOfInterest = new List<PointOfInterestDto>
                    {
                        new PointOfInterestDto {Id = 1, Name = "New York", Description = "NY description"},
                        new PointOfInterestDto {Id = 3, Name = "Moscow", Description = "Moscow description"}
                    }},
                new CityDto {Id = 3, Name = "Moscow", Description = "Moscow description",
                    PointsOfInterest = new List<PointOfInterestDto>
                    {
                        new PointOfInterestDto {Id = 2, Name = "Paris", Description = "Paris description"},
                        new PointOfInterestDto {Id = 1, Name = "New York", Description = "NY description"}
                    }}
            };

        }
    }
}
