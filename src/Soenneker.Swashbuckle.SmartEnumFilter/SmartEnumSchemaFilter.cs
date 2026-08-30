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
    /// <summary>
    /// Replaces a SmartEnum object schema with its declared string values.
    /// </summary>
    /// <param name="schema">Schema to read or generate.</param>
    /// <param name="context">Context for the schema being generated.</param>
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema is not OpenApiSchema mutator)
            return;

        Type? type = context.Type;

        if (!IsTypeDerivedFromGenericType(type, typeof(SmartEnum<>)) && !IsTypeDerivedFromGenericType(type, typeof(SmartEnum<,>)))
        {
            return;
        }

        IEnumerable<string> enumValues = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                                             .Where(field => field.FieldType == type)
                                             .Select(field => field.GetValue(null)?.ToString())
                                             .Where(value => value is not null)
                                             .Select(value => value!);

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
