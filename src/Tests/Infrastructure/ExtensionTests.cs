using Xunit;
using LogViewer.Infrastructure;
using TestAttribute = Xunit.FactAttribute;

namespace IntegrationTests.Infrastructure
{
	public class ExtensionTests
    {
        [Test]
        public void Test_Next() 
        {
            var collection = new int?[] {0,1,2,3,4};
            for (int i = 0; i < collection.Length-1; i++)
            {
                Assert.Equal(i+1, collection.Next(i));
            }
        }
        [Test]
        public void Test_Previous()
        {
            var collection = new int?[] { 0, 1, 2, 3, 4 };
            for (int i = collection.Length-1; i >= 1; i--)
            {
                Assert.Equal(i - 1, collection.Previous(i));
            }
            //Assert.That(collection.Previous(1, c => true), Is.EqualTo(0), "0");
        }

    }
}
