using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starter.Tests
{
    public class IntegrationTest1
    {
        [Fact]
        public async Task GetWebResourceRootReturnsOkStatusCode()
        {
            // Arrange
            var builder = await DistributedApplicationTestingBuilder
                .CreateAsync<Projects.Starter_Web>(TestContext.Current.CancellationToken);

            builder.Services.ConfigureHttpClientDefaults(clientBuilder =>
            {
                clientBuilder.AddStandardResilienceHandler();
            });

            // To capture logs from your tests, see the "Capture logs from tests" section
            // in the documentation or refer to LoggingTest.cs for a complete example

            await using var app = await builder.BuildAsync();

            await app.StartAsync();

            // Act
            var httpClient = app.CreateHttpClient("webfrontend");

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            await app.ResourceNotifications.WaitForResourceHealthyAsync(
                "webfrontend",
                cts.Token);

            var response = await httpClient.GetAsync("/");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
