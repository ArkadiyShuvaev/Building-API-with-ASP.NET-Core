using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.Data.Entities
{
	public class PointOfInterest
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		[ForeignKey(nameof(CityId))]
		public City City { get; set; }

		public int CityId { get; set; }
		[MaxLength(50)]
		public string Name { get; set; }
		[MaxLength(250)]
		public string Description { get; set; }
	}
}