[![](https://img.shields.io/nuget/v/Soenneker.Swashbuckle.SmartEnumFilter.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Swashbuckle.SmartEnumFilter/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.swashbuckle.smartenumfilter/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.swashbuckle.smartenumfilter/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Swashbuckle.SmartEnumFilter.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Swashbuckle.SmartEnumFilter/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.swashbuckle.smartenumfilter/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.swashbuckle.smartenumfilter/actions/workflows/codeql.yml)

# Soenneker.Swashbuckle.SmartEnumFilter

Generates string-enum OpenAPI schemas for Ardalis SmartEnum types.

## Installation

```bash
dotnet add package Soenneker.Swashbuckle.SmartEnumFilter
```

## Registration

```csharp
using Soenneker.Swashbuckle.SmartEnumFilter;

builder.Services.AddSwaggerGen(options =>
{
    options.SchemaFilter<SmartEnumSchemaFilter>();
});
```

## Example

```csharp
using Ardalis.SmartEnum;

public sealed class OrderStatus : SmartEnum<OrderStatus>
{
    public static readonly OrderStatus InProgress = new("in-progress", 1);
    public static readonly OrderStatus Complete = new("complete", 2);

    private OrderStatus(string name, int value) : base(name, value)
    {
    }
}
```

The generated schema uses the SmartEnum names, not the C# field identifiers or integer values:

```yaml
type: string
enum:
  - in-progress
  - complete
```

Only public static fields whose type exactly matches the SmartEnum type are included. The filter replaces the object-shaped schema properties but does not configure runtime serialization. Ensure the application's SmartEnum JSON converter emits the same names described by the OpenAPI document.
