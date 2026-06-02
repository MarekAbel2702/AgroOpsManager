using FluentAssertions;

namespace AgroOpsManager.Tests.Unit
{
    public class HealthCheckTests
    {
        [Fact]
        public void FirstTest_ShouldPass()
        {
            var applicationName = "AgroOps Manager";

            applicationName.Should().Be("AgroOps Manager");
        }
    }
}
