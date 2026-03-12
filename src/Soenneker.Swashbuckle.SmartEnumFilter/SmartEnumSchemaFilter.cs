using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Nodes;
using Ardalis.SmartEnum;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Soenneker.Swashbuckle.SmartEnumFilter;

/// <summary>
/// A Swashbuckle Schema filter for SmartEnum
/// </summary>
public sealed class SmartEnumSchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        var mutator = (OpenApiSchema)schema;

        Type? type = context.Type;

        if (!IsTypeDerivedFromGenericType(type, typeof(SmartEnum<>)) && !IsTypeDerivedFromGenericType(type, typeof(SmartEnum<,>)))
        {
            return;
        }

        IEnumerable<string>? enumValues = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                                              .Select(d => d.Name);

        var openApiValues = new List<JsonNode>();
        openApiValues.AddRange(enumValues.Select(d => JsonValue.Create(d)));

        // See https://swagger.io/docs/specification/data-models/enums/
        mutator.Type = JsonSchemaType.String;
        mutator.Enum = openApiValues;
        mutator.Properties = null;
    }

    private static bool IsTypeDerivedFromGenericType(Type? typeToCheck, Type genericType)
    {
        while (true)
        {
            if (typeToCheck == typeof(object))
            {
                return false;
            }

            if (typeToCheck == null)
            {
                return false;
            }

            if (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            typeToCheck = typeToCheck.BaseType;
        }
    }
}