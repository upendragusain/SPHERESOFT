using Chambers.API.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.IO;
using Xunit;

namespace Chambers.UnitTests
{
    public class FormFileExtensionsTest
    {

        private static Mock<IFormFile> GetMockFile(string fileName, string content)
        {
            var fileMock = new Mock<IFormFile>();

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            return fileMock;
        }

        [Fact]
        public void File_name_is_invalid()
        {
            //Arrange
            var fileMock = GetMockFile("invalid.txt", "invalid file with txt extension");

            //Act
            var result = fileMock.Object.ValidateFileNameAndExtension();

            Assert.True(result == false);
        }

        [Fact]
        public void File_name_is_valid()
        {
            //Arrange
            var fileMock = GetMockFile("valid.pdf", "valid file with pdf extension");

            //Act
            var result = fileMock.Object.ValidateFileNameAndExtension();

            Assert.True(result == true);
        }


    }
}
