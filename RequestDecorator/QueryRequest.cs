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

    public static class GenericQueryRequestDecorator<TI, TR, TC>
    {
        public static Func<Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>>,
            Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>>> GenericDecorator = func => func;
    }
}
