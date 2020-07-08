using Chambers.API;
using Chambers.API.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Application.FunctionlTests
{
    public class FileUploadControllerTest
    {
        [Fact]
        public async Task Will_upload_a_valid_pdf()
        {
            using (var host = await GenericCreateAndStartHost_GetTestServer())
            {
                var filePath = GetFilePath("sample_150kB.pdf");

                using var multipartFormDataContent = new MultipartFormDataContent();
                using var fs = File.OpenRead(filePath);
                using var streamContent = new StreamContent(fs);
                using var fileContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync());
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");

                // "file" parameter name should be the same as the server side input parameter name
                multipartFormDataContent.Add(fileContent, "file", Path.GetFileName(filePath));
                var response = await host.GetTestServer().CreateClient()
                    .PostAsync("/api/v1/document/upload", multipartFormDataContent);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);

                var absoluteUri = response.Headers?.Location?.AbsoluteUri;
                Assert.True(!string.IsNullOrWhiteSpace(absoluteUri));

                //todo: teardown
            }
        }

        [Fact]
        public async Task Wont_upload_an_invalid_extension_file()
        {
            using (var host = await GenericCreateAndStartHost_GetTestServer())
            {
                var filePath = GetFilePath("invalid.txt");

                using var multipartFormDataContent = new MultipartFormDataContent();
                using var fs = File.OpenRead(filePath);
                using var streamContent = new StreamContent(fs);
                using var fileContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync());
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");

                multipartFormDataContent.Add(fileContent, "file", Path.GetFileName(filePath));
                var response = await host.GetTestServer().CreateClient()
                    .PostAsync("/api/v1/document/upload", multipartFormDataContent);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

                //todo: teardown
            }
        }

        [Fact]
        public async Task Wont_upload_an_invalid_size_file()
        {
            using (var host = await GenericCreateAndStartHost_GetTestServer())
            {
                var filePath = GetFilePath("sample_6MB.pdf");

                using var multipartFormDataContent = new MultipartFormDataContent();
                using var fs = File.OpenRead(filePath);
                using var streamContent = new StreamContent(fs);
                using var fileContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync());
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");

                multipartFormDataContent.Add(fileContent, "file", Path.GetFileName(filePath));
                var response = await host.GetTestServer().CreateClient()
                    .PostAsync("/api/v1/document/upload", multipartFormDataContent);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact]
        public async Task Will_get_list_of_all_blobs()
        {
            using (var host = await GenericCreateAndStartHost_GetTestServer())
            {
                var response = await host.GetTestServer().CreateClient()
                    .GetAsync("/api/v1/document/items");

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var responseBody = await response.Content.ReadAsStringAsync();
                var documents = JsonConvert.DeserializeObject<List<Document>>(responseBody);

                Assert.True(documents.Count > 0);
            }
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }

        private static Serilog.ILogger CreateSerilogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Seq(configuration["Serilog:SeqServerUrl"])
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        private async Task<IHost> GenericCreateAndStartHost_GetTestServer()
        {
            var configuration = GetConfiguration();

            Serilog.Log.Logger = CreateSerilogger(configuration);

            var host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .UseStartup<Startup>()
                        .UseSerilog();
                })
                .ConfigureAppConfiguration(cd =>
                {
                    cd.AddJsonFile("appsettings.json", false);
                    cd.AddEnvironmentVariables();
                })
                .StartAsync();

            return host;
        }

        private string GetFilePath(string fileNameWithExtension)
        {
            var directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var projectPath = directoryPath.Replace(directoryPath.Substring(directoryPath.IndexOf("bin\\")), "");
            return Path.Combine(projectPath, "TestData", fileNameWithExtension);
        }

    }
}
