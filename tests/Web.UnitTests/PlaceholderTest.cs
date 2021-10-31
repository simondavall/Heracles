using FluentAssertions;
using NUnit.Framework;

namespace Heracles.Web.UnitTests
{
    class PlaceholderTest
    {
        [Test]
        public void Place_Holder_Test()
        {
            true.Should().BeTrue("Placeholder Test");
        }
    }
}
