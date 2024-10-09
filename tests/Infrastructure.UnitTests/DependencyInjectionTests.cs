using System;
using FluentAssertions;
using Heracles.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace Heracles.Infrastructure.UnitTests
{

    class DependencyInjectionTests
    {
        [Test]
        public void AddDatabaseContexts_UseInMemory_AddsCorrectContexts()
        {
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("UseInMemoryDatabase").Value).Returns("true");
            var services = new ServiceCollection();

            DependencyInjection.AddDatabaseContexts(services, mockConfig.Object);

            services.Should().Contain(x => x.ServiceType.Name == nameof(AppIdentityDbContext));
            services.Should().Contain(x => x.ServiceType.Name == nameof(IDeveloperPageExceptionFilter));
        }

        [Test]
        public void AddDatabaseContexts_UseSQLServer_AddsCorrectContexts()
        {
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("UseInMemoryDatabase").Value).Returns("false");
            var services = new ServiceCollection();

            DependencyInjection.AddDatabaseContexts(services, mockConfig.Object);

            services.Should().Contain(x => x.ServiceType.Name == nameof(AppIdentityDbContext));
            services.Should().Contain(x => x.ServiceType.Name == nameof(IDeveloperPageExceptionFilter));
        }

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
