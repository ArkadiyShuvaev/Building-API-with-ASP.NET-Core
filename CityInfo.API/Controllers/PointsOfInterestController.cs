using System;
using System.Collections.Generic;
using System.Linq;
using CityInfo.API.Models;
using CityInfo.API.Services;
using CityInfo.Data.Entities;
using CityInfo.Data.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _localMailService;
	    private readonly ICityInfoRepository _repository;

	    public PointsOfInterestController(ILogger<PointsOfInterestController> logger, 
			IMailService localMailService, ICityInfoRepository repository)
        {
            _logger = logger;
            _localMailService = localMailService;
	        _repository = repository;
        }

        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            if (cityId <= 0) return BadRequest(nameof(cityId));

            if (cityId <= 0)
            {
                return BadRequest(nameof(cityId));
            }

	        IActionResult actionResult;
	        if (!DoesCityExistAndLogMessage(cityId, out actionResult))
	        {
		        return actionResult;
	        }

	        var pointsOfInterestForCity = _repository.GetPointsOfInterestForCity(cityId);
	        var pointsResult = AutoMapper.Mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity);

            return Ok(pointsResult);
        }

	    

	    [HttpGet("{cityId}/pointsofinterest/{interestId}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int interestId)
        {
            if (cityId <= 0) return BadRequest(nameof(cityId));
            if (interestId <= 0) return BadRequest(nameof(interestId));

			IActionResult actionResult;
	        if (!DoesCityExistAndLogMessage(cityId, out actionResult))
	        {
		        return actionResult;
	        }

			var interest = _repository.GetPointOfInterestForCity(cityId, interestId);
	        if (interest == null)
	        {
		        return NotFound(nameof(interestId));
	        }
			
	        var result = AutoMapper.Mapper.Map<PointOfInterestDto>(interest);

            return Ok(result);
        }


        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointsOfInterest(int cityId, 
            [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (pointOfInterest == null)
            {
                return BadRequest(nameof(pointOfInterest));
            }
            if (cityId <= 0) return BadRequest(nameof(cityId));

            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
			
            if (!_repository.DoesCityExist(cityId))
            {
                return NotFound(nameof(cityId));
            }

			//var city = _repository.GetCity(cityId);

            

	        var newEntity = AutoMapper.Mapper.Map<PointOfInterest>(pointOfInterest);

	        if (!_repository.AddPointOfInterestForCity(cityId, newEntity))
	        {
		        return StatusCode(500, "Some issue has been occurred with saving item.");
	        }

	        var createdPointOfInterestToReturn = AutoMapper.Mapper.Map<PointOfInterestDto>(newEntity);
            return CreatedAtRoute("GetPointOfInterest", new
            {
                cityId,
                interestId = createdPointOfInterestToReturn.Id
			}, createdPointOfInterestToReturn);
        }

        [HttpPut("{cityId}/pointsofinterest/{interestId}")]
        public IActionResult UpdatePointOfInterest(int cityId, int interestId, 
            [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            IActionResult actionResult;
            if (!IsValidationSuccessfull(cityId, interestId, pointOfInterest, out actionResult))
            {
                return actionResult;
            }
			
	        var city = _repository.GetCity(cityId, true);
	        if (city == null)
	        {
		        return NotFound(nameof(city));
	        }

	        var entityFromDataBase = city.PointsOfInterest.FirstOrDefault(p => p.Id == interestId);
	        if (entityFromDataBase == null)
	        {
		        return NotFound(nameof(interestId));
	        }

	        AutoMapper.Mapper.Map(pointOfInterest, entityFromDataBase);

	        if (!_repository.Save())
	        {
		        return StatusCode(500, "Some issue has been occurred while updating pointOfInterest.");
	        }
			
			return NoContent();

        }

        
        [HttpPatch("{cityId}/pointsofinterest/{interestId}")]
        public IActionResult UpdatePartialPointOfInterest(int cityId, int interestId,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest(nameof(patchDocument));
            }

            IActionResult actionResult;
            if (!AreCityAndInterestIdParametersValid(cityId, interestId, out actionResult))
            {
                return actionResult;
            }

			var city = _repository.GetCity(cityId, true);
	        if (city == null)
	        {
		        return NotFound(nameof(city));
	        }

	        var entityFromDataBase = city.PointsOfInterest.FirstOrDefault(p => p.Id == interestId);
	        if (entityFromDataBase == null)
	        {
		        return NotFound(nameof(interestId));
	        }

	        var pointOfInterestToPatch = AutoMapper.Mapper.Map<PointOfInterestForUpdateDto>(entityFromDataBase);

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!IsModelValid(pointOfInterestToPatch, out actionResult))
            {
                return actionResult;
            }
            
            var result = TryValidateModel(pointOfInterestToPatch);
            if (!result)
            {
                return BadRequest(pointOfInterestToPatch);
            }

	        AutoMapper.Mapper.Map(pointOfInterestToPatch, entityFromDataBase);

			if (!_repository.Save())
	        {
		        return StatusCode(500, "Some issue has been occurred while patching pointOfInterest.");
	        }

	        return NoContent();
		}

        [HttpDelete("{cityId}/pointsofinterest/{interestId}")]
        public IActionResult DeletePointOfInterest(int cityId, int interestId)
        {
            IActionResult actionResult;
            if (!AreCityAndInterestIdParametersValid(cityId, interestId, out actionResult))
            {
                return actionResult;
            }

            var city = _repository.GetCity(cityId);

            if (city == null)
            {
                return NotFound(nameof(cityId));
            }

            var existingPointEntity = _repository.GetPointOfInterestForCity(cityId, interestId);

            if (existingPointEntity == null)
            {
                return NotFound(nameof(interestId));
            }


            _repository.DeletePointOfInterest(existingPointEntity);
            _localMailService.Send("Point of interest deleted", 
                $"The point of interest '{existingPointEntity.Name}' with '{existingPointEntity.Id}' id was deleted.");

	        if (!_repository.Save())
	        {
		        return StatusCode(500, "Some issue has been occurred while deleting pointOfInterest.");
	        }

			return NoContent();
        }

	    private bool DoesCityExistAndLogMessage(int cityId, out IActionResult actionResult)
	    {
		    actionResult = null;
			if (_repository.DoesCityExist(cityId))
		    {
			    return true;
		    }
		    
			_logger.LogInformation($"City with id {cityId} was not found when accessing points of interest.");
		    
			actionResult = NotFound(nameof(cityId));
			return false;
		}
		private bool DoesPointOfInterestExist(int cityId, int interestId, out PointOfInterestDto existingPoint,
            out IActionResult actionResult)
        {
            actionResult = null;
            existingPoint = null;
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                {
                    actionResult = NotFound(nameof(cityId));
                    return false;
                }
            }

            existingPoint = city.PointsOfInterest.FirstOrDefault(p => p.Id == interestId);

            if (existingPoint == null)
            {
                {
                    actionResult = NotFound(nameof(interestId));
                    return false;
                }
            }
            return true;
        }


        private bool IsValidationSuccessfull(int cityId, int interestId, PointOfInterestForUpdateDto pointOfInterest,
            out IActionResult actionResult)
        {
            if (!AreCityAndInterestIdParametersValid(cityId, interestId, out actionResult))
            {
                return false;
            }

            if (pointOfInterest == null)
            {
                actionResult = BadRequest(nameof(pointOfInterest));
                return false;
            }

            if (!IsModelValid(pointOfInterest, out actionResult))
            {
                return false;
            }

            return true;
        }

        private bool IsModelValid(PointOfInterestForUpdateDto pointOfInterest, out IActionResult actionResult)
        {
            actionResult = null;
            if (pointOfInterest.Name == pointOfInterest.Description)
            {
                ModelState.AddModelError("description", "The provided description should be different from the name.");
            }

            if (!ModelState.IsValid)
            {
                actionResult = BadRequest(ModelState);
                return false;
            }
            return true;
        }

        private bool AreCityAndInterestIdParametersValid(int cityId, int interestId, out IActionResult actionResult)
        {
            actionResult = null;

            if (cityId <= 0)
            {
                actionResult = BadRequest(nameof(cityId));
                return false;
            }
            if (interestId <= 0)
            {
                actionResult = BadRequest(nameof(interestId));
                return false;
            }
            return true;
        }
    }
}