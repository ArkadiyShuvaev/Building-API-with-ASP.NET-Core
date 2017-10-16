using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
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
                cityId = cityId,
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
            existingPoint.Name = pointOfInterest.Name;
            existingPoint.Description = pointOfInterest.Description;

            return NoContent();

        }
        
        private bool IsValidationSuccessfull(int cityId, int interestId, PointOfInterestForUpdateDto pointOfInterest,
            out IActionResult actionResult)
        {
            actionResult = null;

            if (pointOfInterest == null)
            {
                actionResult = BadRequest(nameof(pointOfInterest));
                return false;
            }
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
    }
}