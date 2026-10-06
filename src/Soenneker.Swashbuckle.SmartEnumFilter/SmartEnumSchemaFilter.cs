using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Soenneker.Swashbuckle.SmartEnumFilter;

/// <summary>Maps enum-like model types to their string values in OpenAPI schemas.</summary>
public sealed class SmartEnumSchemaFilter : ISchemaFilter
{
    private readonly Func<Type, IReadOnlyList<string>?> _values;

    /// <summary>Discovers enum fields at runtime. Use explicit value registrations when trimming.</summary>
    [RequiresUnreferencedCode("Runtime schema discovery requires preserved enum fields. Supply an explicit type-to-values map.")]
    public SmartEnumSchemaFilter() => _values = Discover;

    /// <summary>Uses statically registered schema values without reflection.</summary>
    public SmartEnumSchemaFilter(IReadOnlyDictionary<Type, IReadOnlyList<string>> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        var snapshot = values.ToDictionary(pair => pair.Key, pair => (IReadOnlyList<string>)pair.Value.ToArray());
        _values = type => snapshot.TryGetValue(type, out var result) ? result : null;
    }

    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema is not OpenApiSchema mutable || _values(context.Type) is not { } values)
            return;
        mutable.Type = JsonSchemaType.String;
        mutable.Enum = values.Select(value => (JsonNode)JsonValue.Create(value)!).ToList();
        mutable.Properties = null;
    }

    [RequiresUnreferencedCode("Runtime schema discovery requires preserved enum fields.")]
    private static IReadOnlyList<string>? Discover(Type type)
    {
        if (!IsTypeDerivedFromGenericType(type, typeof(Ardalis.SmartEnum.SmartEnum<>)) && !IsTypeDerivedFromGenericType(type, typeof(Ardalis.SmartEnum.SmartEnum<,>)))
            return null;
        return type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(field => field.FieldType == type)
            .Select(field => field.GetValue(null)?.ToString())
            .Where(value => value is not null).Select(value => value!).ToArray();
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
