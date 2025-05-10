using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty ;
        [Range(0.01, double.MaxValue, ErrorMessage ="price must be greater than 0")]
        public decimal Price { get; set; }
        [Required]
        public string PictureUrl { get; set; }= string.Empty ;
        [Required]
        public string Type { get; set; } = string.Empty;
        [Required]
        public string Brand { get; set; } = string.Empty;
        [Range (1, int.MaxValue, ErrorMessage = "Quantity In Stock must be at least 1")]
        public int QuantityInStock { get; set; }

        public CreateProductDto()
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine(QuantityInStock);
        }
    }
}
