namespace PIMS.Model.CommandModel
{
    public class ProductCommandModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public List<Guid> CategoryIds { get; set; }
    }
}