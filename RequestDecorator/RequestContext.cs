using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RequestDecorator
{
    public class RequestContext<TI, TR, TC> : IRequestContext<TI, TR, TC>
    {
        public IAPIContext<TC> Context { get; }
        public IRequest<TI, TR, TC> RequestInfo { get; }

        public RequestContext(IAPIContext<TC> context, IRequest<TI, TR, TC> requestInfo)
        {
            Context = context;
            RequestInfo = requestInfo;
        }

        public Task<Result<TR>> ProcessRequest()
        {
            return RequestInfo.ProcessRequestFunc(this);
        }
    }
}
