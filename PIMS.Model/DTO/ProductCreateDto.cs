namespace PIMS.Model.DTO
{
    public class ProductCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; }
        public List<Guid> CategoryIds { get; set; } // List of Category IDs
    }
}