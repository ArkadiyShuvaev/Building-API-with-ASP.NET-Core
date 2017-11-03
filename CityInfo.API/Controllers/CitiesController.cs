using System;
using System.Collections.Generic;
using System.Linq;
using CityInfo.API.Models;
using CityInfo.Data.Entities;
using CityInfo.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class CitiesController : Controller
    {
	    private readonly ICityInfoRepository _repository;
	    private readonly ILogger<CitiesController> _logger;

	    public CitiesController(ICityInfoRepository repository, ILogger<CitiesController> logger)
	    {
		    _repository = repository;
		    _logger = logger;
	    }
		[HttpGet]
        public IActionResult GetCities()
		{
			var cityEntities = _repository.GetCities();
			var cities = AutoMapper.Mapper.Map<IEnumerable<CityWithoutPOintsOfInterestDto>>(cityEntities);

			return Ok(cities);
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool includePointsOfinterest = false)
        {
	        var cityEntity = _repository.GetCity(id, includePointsOfinterest);

			if (!includePointsOfinterest)
			{
				if (cityEntity == null)
				{
					return NotFound();
				}

				var city = AutoMapper.Mapper.Map<CityWithoutPOintsOfInterestDto>(cityEntity);

				return Ok(city); 
			}
			
			return Ok(AutoMapper.Mapper.Map<CityDto>(cityEntity));
        }
    }
}
