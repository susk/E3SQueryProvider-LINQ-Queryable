using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Expressions.Task3.E3SQueryProvider.Helpers;
using Expressions.Task3.E3SQueryProvider.Models.Entities;
using Expressions.Task3.E3SQueryProvider.QueryProvider;
using Expressions.Task3.E3SQueryProvider.Services;
using Microsoft.Extensions.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace Expressions.Task3.E3SQueryProvider.Test.Ignored
{
    /// <summary>
    /// Please ignore this integration test set, because the E3S emulator is not currently available.
    /// </summary>
    public class E3SProviderTests
    {
        #region private

        private static IConfigurationRoot Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        private static readonly Lazy<E3SSearchService> SearchService = new Lazy<E3SSearchService>(() =>
        {
            HttpClient httpClient = HttpClientHelper.CreateClient(user, password);
            return new E3SSearchService(httpClient, baseUrl);
        });

        private static string user = Config["api:user"];
        private static string password = Config["api:password"];
        private static string baseUrl = Config["api:apiBaseUrl"];

        private readonly ITestOutputHelper testOutputHelper;

        #endregion

        public E3SProviderTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        #region public tests

        [Fact(Skip = "This test is provided to show the general idea of usage.")]
        public void WithoutProvider()
        {
            IEnumerable<EmployeeEntity> res = SearchService.Value.SearchFts<EmployeeEntity>("workstation:(EPRUIZHW0249)", 0, 1);

            foreach (var emp in res)
            {
                testOutputHelper.WriteLine("{0} {1}", emp.NativeName, emp.StartWorkDate);
            }
        }

        [Fact(Skip = "This test is provided to show the general idea of usage.")]
        public void WithoutProviderNonGeneric()
        {
            var res = SearchService.Value.SearchFts(typeof(EmployeeEntity), "workstation:(EPRUIZHW0249)", 0, 10);

            foreach (var emp in res.OfType<EmployeeEntity>())
            {
                testOutputHelper.WriteLine("{0} {1}", emp.NativeName, emp.StartWorkDate);
            }
        }

        [Fact(Skip = "This test is provided to show the general idea of usage.")]
        public void WithProvider()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(SearchService.Value);

            foreach (var emp in employees.Where(e => e.Workstation == "EPRUIZHW0249"))
            {
                testOutputHelper.WriteLine("{0} {1}", emp.NativeName, emp.StartWorkDate);
            }
        }

        #endregion
    }
}
