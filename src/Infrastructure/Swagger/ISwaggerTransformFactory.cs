using Microsoft.OpenApi.Models;
using System.Collections.Generic;

namespace Finacle.Infrastructure.Swagger
{
    public interface ISwaggerTransformFactory
    {
        bool Build(OpenApiOperation operation, IReadOnlyDictionary<string, string> transformValues);
    }
}
