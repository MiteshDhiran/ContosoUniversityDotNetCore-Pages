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
            var decoratedFunc = DecoratedFunc(requestWithContext);
            var res =  await decoratedFunc(requestWithContext);
            var retVal = res.GetValueThrowExceptionIfExceptionPresent();
            return retVal;
        }

        private static Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>> DecoratedFunc<TI, TR, TC>(IRequestContext<TI, TR, TC> requestWithContext)
        {
            if (requestWithContext is IRequestWithValidationContext<TI, TR, TC> requestWithValidationContext)
            {
                var decoratedFunc = requestWithContext.RequestInfo.ProcessRequestFunc
                        .DecorateRequestWithValidation(requestWithValidationContext.RequestInfo.ValidationFunc)
                        .DecorateWithExecutionTimeLogger()
                    ;
                return decoratedFunc;
            }
            else
            {
                var decoratedFunc = requestWithContext.RequestInfo.ProcessRequestFunc
                        .DecorateWithExecutionTimeLogger()
                    ;
                return decoratedFunc;
            }
            
        }

        
    }
}
