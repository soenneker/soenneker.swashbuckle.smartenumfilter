using Soenneker.Tests.HostedUnit;

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
}
