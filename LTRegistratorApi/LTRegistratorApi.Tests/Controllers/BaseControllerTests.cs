using AutoMapper;
using LTRegistrator.BLL.Services;
using LTRegistratorApi.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;

namespace LTRegistratorApi.Tests.Controllers
{
    public abstract class BaseControllerTests
    {
        protected readonly LTRegistratorDbContext Db;
        protected readonly IMapper Mapper;

        protected BaseControllerTests()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile<DataMappingProfileWeb>();
            });
            Mapper = mappingConfig.CreateMapper();

            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=productsdb;Trusted_Connection=True;";
            var options = new DbContextOptionsBuilder<LTRegistratorDbContext>()
                .UseInMemoryDatabase(connectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                // don't raise the error warning us that the in memory db doesn't support transactions
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            Db = new LTRegistratorDbContext(options);
            Initializer.Initialize(Db);
        }

        protected static int ToHttpStatusCodeResult(ActionResult result)
        {
            if (result is ObjectResult)
                return (int)(result as ObjectResult).StatusCode;
            else if (result is StatusCodeResult)
                return (result as StatusCodeResult).StatusCode;
            else throw new ArgumentException();
        }
    }
}
