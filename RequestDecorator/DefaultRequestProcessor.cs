using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RequestDecorator
{
    public static class DefaultRequestProcessor
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

        public static ConcurrentDictionary<Type, object> TypeDecoratorFunctionRegistry =
            new ConcurrentDictionary<Type, object>();

        public static void Register<TI, TR, TC>(Type type,
            Func<Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>>,
                Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>>> func)
        {
            TypeDecoratorFunctionRegistry.TryAdd(type, func);
        }

        public static Func<Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>>, Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>>> GetDecoratorFunc<TI,TR,TC>(Type type)
        {
            if (TypeDecoratorFunctionRegistry.TryGetValue(type, out var func))
            {
                return (Func<Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>>,
                    Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>>>) func;
            }
            else
            {
                return null;
            }
        }

        public static Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>> DecorateFuncBasedOnType<TI, TR, TC>(Type type,
            Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>> funcToBeDecorated)
        {
            var decoratorFunc = GetDecoratorFunc<TI, TR, TC>(type);
            return decoratorFunc != null ? decoratorFunc(funcToBeDecorated) : funcToBeDecorated;
        }


    }
}
