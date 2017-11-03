using System;
using System.Collections.Generic;
using System.Linq;
using CityInfo.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.Data.Repositories
{
	public class CityInfoRepository : ICityInfoRepository
	{
		private readonly CityInfoContext _context;

		public CityInfoRepository(CityInfoContext context)
		{
			_context = context;
		}
		public IEnumerable<City> GetCities()
		{
			return _context.Cities.OrderBy(c => c.Name).ToList();
		}

		public City GetCity(int cityId, bool includePointsOfinterest = false)
		{
			if (includePointsOfinterest)
			{
				return _context.Cities.Include(c => c.PointsOfInterest).FirstOrDefault(c => c.Id == cityId);
			}

			return _context.Cities.FirstOrDefault(c => c.Id == cityId);
		}

		public IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId)
		{
			return _context.PointsOfInterest.Where(p => p.CityId == cityId).ToList();
		}

		public PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId)
		{
			return _context.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId && p.CityId == cityId);
		}

		public bool DoesCityExist(int cityId)
		{
			return _context.Cities.Any(c => c.Id == cityId);
		}

		public bool AddPointOfInterestForCity(int cityId, PointOfInterest newEntity)
		{
			var city = GetCity(cityId);
			if (city == null)
			{
				throw new ArgumentNullException(nameof(city));
			}
			
			city.PointsOfInterest.Add(newEntity);

			return Save();
		}

		public bool Save()
		{
			return _context.SaveChanges() >= 0;
		}
		
	}
}