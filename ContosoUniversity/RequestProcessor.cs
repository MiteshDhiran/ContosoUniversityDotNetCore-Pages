using CommandDecoratorExtension;
using RequestDecorator;
using System;
using System.Threading.Tasks;

namespace ContosoUniversity
{
    public static class RequestProcessor
    {
        public static async Task<TR> ProcessRequest<TI, TR, TC>(this IRequestWithFluentValidator<TI,TR,TC> request,IAPIContext<TC> apiContext)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (apiContext == null) throw new ArgumentNullException(nameof(apiContext));

            var decoratedFunc = request.ProcessRequestFunc.DecorateRequestWithFluentValidation(request.ValidationFunc)
                .DecorateWithExecutionTimeLogger();
            var res = await decoratedFunc(new RequestWithContext<TI,TR,TC>(apiContext,request)).ConfigureAwait(false);
            var retVal = res.GetValueThrowExceptionIfExceptionPresent();
            return retVal;
        }
    }
}
