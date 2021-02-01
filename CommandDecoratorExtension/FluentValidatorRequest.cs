using System;
using System.Diagnostics;
using System.Threading.Tasks;
using RequestDecorator;
using RequestDecorator.Functional;

namespace CommandDecoratorExtension
{
    public interface IRequestWithFluentValidator<TI, TR, TC> : IRequest<TI, TR, TC>
    {
        Func<IRequestContext<TI, TR, TC>, MayBe<FluentValidation.ValidationException>> ValidationFunc { get; }
    }

    public static class FluentValidationExtension
    {
        public static Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>>
            DecorateRequestWithFluentValidation<TI, TR, TC>(
                this Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>> funcToBeDecorated,
                Func<IRequestContext<TI, TR, TC>, MayBe<FluentValidation.ValidationException>> validationFunc)
            =>
                funcToBeDecorated.PipeLineDecorateFunc<int, IRequestContext<TI, TR, TC>, Task<Result<TR>>>(
                    (input) => 0
                    , (sw, input) =>
                    {
                        var mayBeValidationException = validationFunc(input);
                        return mayBeValidationException.TryGetValue(out var validationException) 
                            ? new MayBe<Task<Result<TR>>>(Task.FromResult(new Result<TR>(validationException))) 
                            : MayBeExtension.GetNothingMaybe<Task<Result<TR>>>();
                    }
                    ,(sw, input, previousResultValue) => previousResultValue.GetValueThrowExceptionIfExceptionPresent());
    }
}
