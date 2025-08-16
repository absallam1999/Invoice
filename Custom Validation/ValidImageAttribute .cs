﻿using System.ComponentModel.DataAnnotations;

namespace invoice.Custom_Validation
{
    public class ValidImageAttribute: ValidationAttribute
    {
        private readonly string[] _allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        private readonly string[] _allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp" };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file == null)
                return ValidationResult.Success; 

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var contentType = file.ContentType.ToLowerInvariant();

            if (!_allowedExtensions.Contains(extension) || !_allowedContentTypes.Contains(contentType))
            {
                return new ValidationResult("Only valid image files are allowed (.jpg, .png, etc.).");
            }

            return ValidationResult.Success;
        }
    }
}
