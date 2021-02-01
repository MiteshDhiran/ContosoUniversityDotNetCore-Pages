using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RequestDecorator
{
    public interface IRequest<TI, TR, TC>
    {
        TI Data { get; }
        Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>> ProcessRequestFunc { get; }

        //Task<Result<TR>> Process(IAPIContext<TC> context);
    }
}
