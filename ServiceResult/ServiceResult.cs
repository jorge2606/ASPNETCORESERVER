using System;
using System.Collections.Generic;
namespace server.ServiceResult
{
    public class ServiceResult : ServiceResultBase
    {

    }

    public class ServiceResult<T> : ServiceResultBase
    {
        public ServiceResult()
        {

        }

        public ServiceResult(T response) : base()
        {
            Response = response;
        }

        public T Response { get; set; }
    }
}
