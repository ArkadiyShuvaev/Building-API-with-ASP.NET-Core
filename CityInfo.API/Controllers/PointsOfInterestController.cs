using System;
using System.Linq;
using CityInfo.API.Models;
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
        
        public PointsOfInterestController(ILogger<PointsOfInterestController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            if (cityId <= 0) return BadRequest(nameof(cityId));

            if (cityId <= 0)
            {
                return BadRequest(nameof(cityId));
            }
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                _logger.LogInformation($"The '{cityId}' city cannot be found.");
                return NotFound();
            }

            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{cityId}/pointsofinterest/{interestId}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int interestId)
        {
            if (cityId <= 0) return BadRequest(nameof(cityId));
            if (interestId <= 0) return BadRequest(nameof(interestId));

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound(nameof(cityId));
            }

            var interest = city.PointsOfInterest.FirstOrDefault(i => i.Id == interestId);

            if (interest == null)
            {
                return NotFound(nameof(interestId));
            }

            return Ok(interest);
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

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound(nameof(cityId));
            }

            var points = city.PointsOfInterest;
            var newInterestId = points.Max(c => c.Id) + 1;

            var newPointOfInterestDto = new PointOfInterestDto
            {
                Description = pointOfInterest.Description,
                Name = pointOfInterest.Name,
                Id = newInterestId
            };
            city.PointsOfInterest.Add(newPointOfInterestDto);


            return CreatedAtRoute("GetPointOfInterest", new
            {
                cityId,
                interestId = newInterestId
            }, newPointOfInterestDto);
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

            PointOfInterestDto existingPoint;
            if (!DoesPointOfInterestExist(cityId, interestId, out existingPoint, out actionResult))
            {
                return actionResult;
            }

            existingPoint.Name = pointOfInterest.Name;
            existingPoint.Description = pointOfInterest.Description;

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
            if (!IsValidationCityAndInterestIdSuccessfull(cityId, interestId, out actionResult))
            {
                return actionResult;
            }

            PointOfInterestDto existingPoint;
            if (!DoesPointOfInterestExist(cityId, interestId, out existingPoint, out actionResult))
            {
                return actionResult;
            }

            var pointOfInterestForUpdateDto = new PointOfInterestForUpdateDto
            {
                Name = existingPoint.Name,
                Description = existingPoint.Description
            };

            patchDocument.ApplyTo(pointOfInterestForUpdateDto, ModelState);

            if (!IsModelValid(pointOfInterestForUpdateDto, out actionResult))
            {
                return actionResult;
            }
            
            var result = TryValidateModel(pointOfInterestForUpdateDto);
            if (!result)
            {
                return BadRequest(pointOfInterestForUpdateDto);
            }
            
            existingPoint.Name = pointOfInterestForUpdateDto.Name;
            existingPoint.Description = pointOfInterestForUpdateDto.Description;

            return NoContent();
        }

        [HttpDelete("{cityId}/pointsofinterest/{interestId}")]
        public IActionResult DeletePointOfInterest(int cityId, int interestId)
        {
            IActionResult actionResult;
            if (!IsValidationCityAndInterestIdSuccessfull(cityId, interestId, out actionResult))
            {
                return actionResult;
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound(nameof(cityId));
            }

            var existingPoint = city.PointsOfInterest.FirstOrDefault(p => p.Id == interestId);

            if (existingPoint == null)
            {
                return NotFound(nameof(interestId));
            }


            city.PointsOfInterest.Remove(existingPoint);

            return NoContent();
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
            if (!IsValidationCityAndInterestIdSuccessfull(cityId, interestId, out actionResult))
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

        private bool IsValidationCityAndInterestIdSuccessfull(int cityId, int interestId, out IActionResult actionResult)
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