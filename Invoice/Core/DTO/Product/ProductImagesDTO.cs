using Microsoft.AspNetCore.Mvc;

namespace invoice.Core.DTO.Product
{
    public class ProductImagesDTO
    {
        [FromForm(Name = "mainImage")]
        public IFormFile? MainImage { get; set; }

        [FromForm(Name = "galleryImages")]
        public List<IFormFile>? GalleryImages { get; set; }
    }
}
