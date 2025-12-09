using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace invoice.Helpers
{
    public class FormFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.RequestBody == null) return;

            if (!operation.RequestBody.Content.ContainsKey("multipart/form-data"))
                return;

            var schema = operation.RequestBody.Content["multipart/form-data"].Schema;

            foreach (var prop in schema.Properties.ToList())
            {
                if (prop.Value.Type == "object" && prop.Value.Properties?.Any() == true)
                {
                    schema.Properties[prop.Key] = new OpenApiSchema
                    {
                        Type = "string",
                        Format = "binary"
                    };
                }
            }
        }
    }
}
