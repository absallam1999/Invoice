using invoice.DTO.Product;

public class CategoryDetailsDTO
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<ProductDetailsDTO> Products { get; set; }
}
