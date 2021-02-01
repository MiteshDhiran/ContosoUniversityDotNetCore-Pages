using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RequestDecorator
{
    public static class RequestProcessor
    {
        public static async Task<TR> ProcessRequest<TI, TR, TC>(this IRequest<TI, TR, TC> request, IAPIContext<TC> apiContext)
        {
            var requestWithContext = new RequestWithContext<TI, TR, TC>(apiContext, request);
            var res = await requestWithContext.RequestInfo.ProcessRequestFunc(requestWithContext);
            var retVal = res.GetValueThrowExceptionIfExceptionPresent();
            return retVal;
        }
    }
}
