using LogViewer.Infrastructure;
using Xunit;

namespace IntegrationTests
{
	public class FileTests
    {
        [Fact]
        public void FileInFolder1()
        {
            var file = new FileWithPosition(@"C:\Progra~1\someprogram\somelog.txt");
            Assert.True(file.FileNameInFolder(@"C:\Program Files\someprogram\"),"1");
            Assert.True(file.FileNameInFolder(@"C:\Progra~1\someprogram\"),"2");
        }
        [Fact]
        public void FileInFolder2()
        {
            var file2 = new FileWithPosition(@"C:\Program Files\someprogram\somelog.txt");
            Assert.True(file2.FileNameInFolder(@"C:\Program Files\someprogram\"),"1");
            Assert.True(file2.FileNameInFolder(@"C:\Progra~1\someprogram\"),"2");
        }
        [Fact]
        public void FileNotInFolder()
        {
            var file = new FileWithPosition(@"C:\Progra~1\someprogram1\somelog.txt");
            Assert.True(!file.FileNameInFolder(@"C:\Program Files\someprogram\"),"1");
            Assert.True(!file.FileNameInFolder(@"C:\Progra~1\someprogram\"),"2");
            var file2 = new FileWithPosition(@"C:\Program Files\someprogram1\somelog.txt");
            Assert.True(!file2.FileNameInFolder(@"C:\Program Files\someprogram\"));
            Assert.True(!file2.FileNameInFolder(@"C:\Progra~1\someprogram\"));
        }
    }
}
