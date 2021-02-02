using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RequestDecorator.Functional;

namespace RequestDecorator
{
    /*
    public static class QueryRequestExtension
    {
        public static QueryRequest<TI, TR, TC> GetQueryRequest<TI, TR, TC>(IRequestWithoutContextType<TI, TR> requestWithoutContextType
            , Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>> processFunc
            , Func<IRequestContext<TI, TR, TC>, MayBe<ValidationMessage<TI>>> validateFunc)
        {
            return new QueryRequest<TI, TR, TC>(requestWithoutContextType, validateFunc,processFunc );
        }

        public static QueryRequest<TI, TR, TC> GetQueryRequest<TI, TR, TC>(TI data
            , Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>> processFunc
            , Func<IRequestContext<TI, TR, TC>, MayBe<ValidationMessage<TI>>> validateFunc)
        {
            return new QueryRequest<TI, TR, TC>(new RequestWithoutContextType<TI, TR>(data), validateFunc, processFunc);
        }
    }
    */
}
