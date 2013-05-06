using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using LogViewer.Infrastructure;
namespace IntegrationTests.Infrastructure
{
    [TestFixture]
    public class ExtensionTests
    {
        [Test]
        public void Test_Next() 
        {
            var collection = new int?[] {0,1,2,3,4};
            for (int i = 0; i < collection.Length-1; i++)
            {
                Assert.That(collection.Next(i, c => true), Is.EqualTo(i+1), i.ToString());
            }
        }
        [Test]
        public void Test_Previous()
        {
            var collection = new int?[] { 0, 1, 2, 3, 4 };
            for (int i = collection.Length-1; i >= 1; i--)
            {
                Assert.That(collection.Previous(i, c => true), Is.EqualTo(i - 1), i.ToString());
            }
            //Assert.That(collection.Previous(1, c => true), Is.EqualTo(0), "0");
        }

    }
}
