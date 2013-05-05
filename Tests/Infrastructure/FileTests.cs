using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using LogViewer;
using LogViewer.Infrastructure;
namespace IntegrationTests
{
    [TestFixture]
    public class FileTests
    {
        [Test]
        public void FileNameMatch() 
        {
            var file = new FileWithPosition(@"C:\Progra~1\someprogram\somelog.txt");
            Assert.That(file.FileNameMatch(@"C:\Program Files\someprogram\somelog.txt"));
        }
        [Test]
        public void FileNameDoesNotMatch()
        {
            var file = new FileWithPosition(@"C:\Progra~1\someprogram\somelog2.txt");
            Assert.That(!file.FileNameMatch(@"C:\Program Files\someprogram\somelog.txt"));
        }

        [Test]
        public void FileInFolder1()
        {
            var file = new FileWithPosition(@"C:\Progra~1\someprogram\somelog.txt");
            Assert.That(file.FileNameInFolder(@"C:\Program Files\someprogram\"));
            Assert.That(file.FileNameInFolder(@"C:\Progra~1\someprogram\"));
        }
        [Test]
        public void FileInFolder2()
        {
            var file2 = new FileWithPosition(@"C:\Program Files\someprogram\somelog.txt");
            Assert.That(file2.FileNameInFolder(@"C:\Program Files\someprogram\"));
            Assert.That(file2.FileNameInFolder(@"C:\Progra~1\someprogram\"));
        }
        [Test]
        public void FileNotInFolder()
        {
            var file = new FileWithPosition(@"C:\Progra~1\someprogram1\somelog.txt");
            Assert.That(!file.FileNameInFolder(@"C:\Program Files\someprogram\"));
            Assert.That(!file.FileNameInFolder(@"C:\Progra~1\someprogram\"));
            var file2 = new FileWithPosition(@"C:\Program Files\someprogram1\somelog.txt");
            Assert.That(!file2.FileNameInFolder(@"C:\Program Files\someprogram\"));
            Assert.That(!file2.FileNameInFolder(@"C:\Progra~1\someprogram\"));
        }
    }
}
