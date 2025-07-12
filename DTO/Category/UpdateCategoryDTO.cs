using System.ComponentModel.DataAnnotations;
public class UpdateCategoryDTO
{
    [Required]
    public string Id { get; set; }

    [Required]
    public string Name { get; set; }
}
