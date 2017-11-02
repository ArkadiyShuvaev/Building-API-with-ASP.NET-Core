using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.Data.Entities
{
    public class City
    {
	    [Key] 
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
	    [MaxLength(50)]
		public string Name { get; set; }
	    [MaxLength(250)]
		public string Description { get; set; }
		public ICollection<PointOfInterest> PointsOfInterest { get; set; } =
		    new List<PointOfInterest>();
	}
}
