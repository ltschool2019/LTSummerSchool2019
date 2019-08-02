using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace LTRegistrator.BLL.Services.Services
{
    public abstract class BaseService
    {
        protected DbContext DbContext { get; set; }
        protected IMapper Mapper { get; set; }
        public BaseService(DbContext db, IMapper mapper)
        {
            DbContext = db ?? throw new ArgumentNullException(nameof(db));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
    }
}
