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
        protected readonly DbContext Db;
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

        protected int ToHttpStatusCodeResult(ActionResult result)
        {
            switch (result)
            {
                case ObjectResult objectResult:
                    return objectResult.StatusCode ?? throw new ArgumentException(nameof(objectResult.StatusCode));
                case StatusCodeResult codeResult:
                    return codeResult.StatusCode;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
