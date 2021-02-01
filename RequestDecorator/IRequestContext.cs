using System;
using System.Collections.Generic;
using System.Text;

namespace RequestDecorator
{
    public interface IRequestContext<TI, TR, TC>
    {
        IAPIContext<TC> Context { get; }
        IRequest<TI, TR, TC> RequestInfo { get; }
    }

    public interface IRequestWithValidationContext<TI, TR, TC>
    {
        IAPIContext<TC> Context { get; }
        IRequestWithValidation<TI, TR, TC> RequestInfo { get; }
    }
}
