using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace Heracles.Infrastructure.UnitTests
{

    class DependencyInjectionTests
    {
        [Test]
        public void AddMigrationEndPoint_IsDevelopment_AddsMigrationEndpoint()
        {
            var mockEnv = new Mock<IWebHostEnvironment>();
            mockEnv.Setup(x => x.EnvironmentName).Returns("Development");
            var mockApp = new Mock<IApplicationBuilder>();
            mockApp.Setup(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()));

            DependencyInjection.UseMigrationsEndPoint(mockApp.Object, mockEnv.Object);

            mockApp.Verify(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once);
        }

        [Test]
        public void AddMigrationEndPoint_IsProduction_DoesNotAddMigrationsEndpoint()
        {
            var mockEnv = new Mock<IWebHostEnvironment>();
            mockEnv.Setup(x => x.EnvironmentName).Returns("Production");
            var mockApp = new Mock<IApplicationBuilder>();
            mockApp.Setup(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()));

            DependencyInjection.UseMigrationsEndPoint(mockApp.Object, mockEnv.Object);

            mockApp.Verify(x => x.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Never);
        }
    }
}
