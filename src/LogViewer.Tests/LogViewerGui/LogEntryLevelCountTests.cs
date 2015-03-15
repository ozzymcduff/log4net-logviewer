using IntegrationTests.LogViewerGui;
using Xunit;

namespace IntegrationTests
{
	public class LogEntryLevelCountTests : TestFixtureBase
    {
        [Fact]
        public void Can_equals()
        {
            Assert.Equal(LCount(Kv("ERROR", 1), Kv("WARN", 1)),LCount(Kv("WARN", 1), Kv("ERROR", 1)));
            Assert.Equal(LCount(Kv("WARN", 1), Kv("ERROR", 1)),LCount(Kv("WARN", 1), Kv("ERROR", 1)));
            Assert.Equal(LCount(Kv("WARN", 2), Kv("ERROR", 1)),LCount(Kv("WARN", 2), Kv("ERROR", 1)));
            Assert.NotEqual(LCount(Kv("WARN", 2), Kv("ERROR", 1)),LCount(Kv("ERROR", 1)));
            Assert.NotEqual(LCount(Kv("ERROR", 1)),LCount(Kv("WARN", 2), Kv("ERROR", 1)));
        }

        [Fact]
        public void Can_add()
        {
            var count1 = LCount(Kv("WARN", 1), Kv("ERROR", 2));
            var count2 = LCount(Kv("WARN", 3));
            Assert.Equal(LCount(Kv("WARN", 4), Kv("ERROR", 2)),count1 + count2);
        }

        [Fact]
        public void Can_remove()
        {
            var count1 = LCount(Kv("WARN", 1), Kv("ERROR", 2));
            var count2 = LCount(Kv("ERROR", 1));
            Assert.Equal(LCount(Kv("WARN", 1), Kv("ERROR", 1)),count1 - count2);
        }
    }
}
