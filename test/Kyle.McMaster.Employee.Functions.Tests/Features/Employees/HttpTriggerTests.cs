using FluentAssertions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using System.Net;
using Xunit;

namespace Kyle.McMaster.Employee.Functions.Tests;
public class HttpTriggerTests
{
    private readonly IHost host;

    public HttpTriggerTests()
    {
        host = Host.CreateDefaultBuilder()
            .UseEnvironment("Development")
            .ConfigureHostConfiguration(c =>
            {
                var config = new Dictionary<string, string>
                {
                    { "AzureWebJobsStorage", "UseDevelopmentStorage=true" }
                };
                c.AddInMemoryCollection(config);
            })
            .ConfigureWebJobs(webJobs =>
            {
                webJobs.AddFiles();
                webJobs.AddAzureStorageCoreServices();
                webJobs.AddHttp();
            })
            .ConfigureServices(services => 
            {
                services.AddHttpClient();
            })
            .Build();
    }

    [Fact]
    public async void Test1()
    {
        using (host)
        {
            await host.StartAsync();

            var factory = host.Services.GetRequiredService<IHttpClientFactory>();
            var client = factory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:7071/api/HttpTrigger");
            var response = await client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}