using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using LTRegistrator.DAL.Contracts;

namespace LTRegistrator.BLL.Services.Services
{
    public abstract class BaseService
    {
        protected IUnitOfWork UnitOfWork { get; set; }
        protected IMapper Mapper { get; set; }

        public BaseService(IUnitOfWork uow, IMapper mapper)
        {
            UnitOfWork = uow ?? throw new ArgumentNullException(nameof(uow));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
    }
}
