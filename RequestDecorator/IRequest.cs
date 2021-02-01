using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RequestDecorator.Functional;

namespace RequestDecorator
{
    public interface IRequest<TI, TR, TC>
    {
        TI Data { get; }
        Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>> ProcessRequestFunc { get; }
    }

    public interface  IRequestWithValidation<TI, TR, TC> : IRequest<TI, TR, TC>
    {
        Func<IRequestContext<TI, TR, TC>, MayBe<ValidationMessage<TI>>> ValidationFunc { get; }
    }
}
