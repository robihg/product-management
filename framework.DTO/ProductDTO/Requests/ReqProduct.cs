

using System.ComponentModel.DataAnnotations;

namespace framework.DTO.ProductDTO.Requests
{
    public class ReqProduct
    {
        public Guid? Guid { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}
