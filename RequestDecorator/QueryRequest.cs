using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RequestDecorator.Functional;

namespace RequestDecorator
{

    public interface IRequestWithoutContextType<TI, TR>
    {
        TI Data { get; }
        
    }

    
    public class RequestWithoutContextType<TI, TR> : IRequestWithoutContextType<TI, TR>
    {
        public RequestWithoutContextType(TI data)
        {
            Data = data;
        }
        public TI Data { get; }
    }
    

    public class QueryRequest<TI, TR, TC> : IRequest<TI, TR, TC>
    {
        public IRequestWithoutContextType<TI, TR> QueryRequestWithoutContextType { get; }
        public TI Data => QueryRequestWithoutContextType.Data;
        public RequestType RequestType => RequestType.Query;
        public Func<IRequestContext<TI, TR, TC>, MayBe<ValidationMessage<TI>>> ValidateFunc { get; }

        public Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>> ProcessRequestFunc { get; }

        public Task<Result<TR>> Process(IAPIContext<TC> context)
        {
            var decoratedFunc = GenericQueryRequestDecorator<TI, TR, TC>.GenericDecorator(this.ProcessRequestFunc);
            return decoratedFunc(new RequestContext<TI, TR, TC>(context, this));
        }
        public QueryRequest(IRequestWithoutContextType<TI, TR> queryRequestWithoutContextType, Func<IRequestContext<TI, TR, TC>, MayBe<ValidationMessage<TI>>> validateFunc,
            Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>> generateResponseFunc)
        {
            QueryRequestWithoutContextType = queryRequestWithoutContextType;
            ValidateFunc = validateFunc ?? throw new ArgumentNullException();
            ProcessRequestFunc = generateResponseFunc ?? throw new ArgumentNullException();
        }

        
    }

    


    public static class GenericQueryRequestDecorator<TI, TR, TC>
    {
        public static Func<Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>>,
            Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>>> GenericDecorator = func => func;
    }
}
