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

        public static async Task<TR> ProcessRequest<TI, TR, TC>(this IRequestWithValidation<TI, TR, TC> request, IAPIContext<TC> apiContext)
        {
            var requestWithContext = new RequestWithValidationContext<TI,TR,TC>(apiContext, request);
            var decoratedFunc = requestWithContext.RequestInfo.ProcessRequestFunc
                .DecorateRequestWithValidation(requestWithContext.RequestInfo.ValidationFunc)
                .DecorateWithExecutionTimeLogger()
                ;
            var res =  await decoratedFunc(requestWithContext);
            var retVal = res.GetValueThrowExceptionIfExceptionPresent();
            return retVal;

            //var validationDecorationFunc = requestWithContext.DecorateWithValidation();
            //var res = await validationDecorationFunc(requestWithContext);

            //decorate ProcessRequestFunc
            //requestWithContext.RequestInfo.ValidationFunc(requestWithContext)
            //requestWithContext.RequestInfo.ProcessRequestFunc.DecorateAPIBehavior()(requestWithContext)
            /*
            var res = await requestWithContext.RequestInfo.ProcessRequestFunc(requestWithContext);
            var retVal = res.GetValueThrowExceptionIfExceptionPresent();
            return retVal;
            */
        }
    }
}
