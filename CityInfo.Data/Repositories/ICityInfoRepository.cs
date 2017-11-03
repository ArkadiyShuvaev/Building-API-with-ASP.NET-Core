using System.Collections.Generic;
using CityInfo.Data.Entities;

namespace CityInfo.Data.Repositories
{
	public interface ICityInfoRepository
	{
		IEnumerable<City> GetCities();
		City GetCity(int cityId, bool includePointsOfinterest = false);
		IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId);
		PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId);
		bool DoesCityExist(int cityId);
		bool AddPointOfInterestForCity(int cityId, PointOfInterest newEntity);
		
		bool Save();
		bool DeletePointOfInterest(PointOfInterest existingPoint);
	}
}