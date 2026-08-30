using Ardalis.SmartEnum;
using Microsoft.OpenApi;
using Soenneker.Tests.HostedUnit;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Threading.Tasks;

namespace Soenneker.Swashbuckle.SmartEnumFilter.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class SmartEnumSchemaFilterTests : HostedUnitTest
{
    public SmartEnumSchemaFilterTests(Host host) : base(host)
    {
    }

    [Test]
    public void Default()
    {

    }

    [Test]
    public void Apply_should_ignore_schema_references()
    {
        var filter = new SmartEnumSchemaFilter();
        var schema = new OpenApiSchemaReference("RequestDataOptions", new OpenApiDocument(), "3.0");
        var context = new SchemaFilterContext(typeof(string), null!, new SchemaRepository(), null, null);

        filter.Apply(schema, context);
    }

    [Test]
    public async Task Apply_should_use_smart_enum_names_instead_of_field_names()
    {
        var filter = new SmartEnumSchemaFilter();
        var schema = new OpenApiSchema();
        var context = new SchemaFilterContext(typeof(TestStatus), null!, new SchemaRepository(), null, null);

        filter.Apply(schema, context);

        await Assert.That(schema.Enum).HasSingleItem();
        await Assert.That(schema.Enum![0]!.GetValue<string>()).IsEqualTo("in-progress");
    }

    private sealed class TestStatus : SmartEnum<TestStatus>
    {
        public static readonly TestStatus InProgress = new("in-progress", 1);

        private TestStatus(string name, int value) : base(name, value)
        {
        }
    }
}
