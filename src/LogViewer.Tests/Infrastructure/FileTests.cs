using LogViewer.Infrastructure;
using Xunit;
using TestAttribute = Xunit.FactAttribute;

namespace IntegrationTests
{
	public class FileTests
    {
        [Test]
        public void FileNameMatch() 
        {
            var file = new FileWithPosition(@"C:\Progra~1\someprogram\somelog.txt");
            Assert.True(file.FileNameMatch(@"C:\Program Files\someprogram\somelog.txt"));
        }
        [Test]
        public void FileNameDoesNotMatch()
        {
            var file = new FileWithPosition(@"C:\Progra~1\someprogram\somelog2.txt");
            Assert.True(!file.FileNameMatch(@"C:\Program Files\someprogram\somelog.txt"));
        }

        [Test]
        public void FileInFolder1()
        {
            var file = new FileWithPosition(@"C:\Progra~1\someprogram\somelog.txt");
            Assert.True(file.FileNameInFolder(@"C:\Program Files\someprogram\"));
            Assert.True(file.FileNameInFolder(@"C:\Progra~1\someprogram\"));
        }
        [Test]
        public void FileInFolder2()
        {
            var file2 = new FileWithPosition(@"C:\Program Files\someprogram\somelog.txt");
            Assert.True(file2.FileNameInFolder(@"C:\Program Files\someprogram\"));
            Assert.True(file2.FileNameInFolder(@"C:\Progra~1\someprogram\"));
        }
        [Test]
        public void FileNotInFolder()
        {
            var file = new FileWithPosition(@"C:\Progra~1\someprogram1\somelog.txt");
            Assert.True(!file.FileNameInFolder(@"C:\Program Files\someprogram\"));
            Assert.True(!file.FileNameInFolder(@"C:\Progra~1\someprogram\"));
            var file2 = new FileWithPosition(@"C:\Program Files\someprogram1\somelog.txt");
            Assert.True(!file2.FileNameInFolder(@"C:\Program Files\someprogram\"));
            Assert.True(!file2.FileNameInFolder(@"C:\Progra~1\someprogram\"));
        }
    }
}
