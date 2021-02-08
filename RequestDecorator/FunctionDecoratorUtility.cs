using System;
using System.Diagnostics;
using System.Threading.Tasks;
using RequestDecorator.Functional;
using static RequestDecorator.Functional.MayBeExtension;

namespace RequestDecorator
{
    public static class FunctionDecoratorUtility
    {
        /*
        public static Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>> DecorateAPIBehavior<TI, TR,TC>(
            this Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>> apiFuncToBeDecorated)
        {
            return apiFuncToBeDecorated;
                //.DecorateQueryWithValidation()
                //.DecorateWithExecutionTimeLogger((input) => input.Context.FrameworkContext.APITrackingInfo.IsAPITrackingPerformanceEnabled)
                //.DecorateWithAPIInputOutputLogger((input) => input.Context.FrameworkContext.APITrackingInfo.IsAPITrackingInputEnabled || input.Context.FrameworkContext.APITrackingInfo.IsAPITrackingOutputEnabled || input.Context.FrameworkContext.APITrackingInfo.IsTrackingExceptionEnabled);
        }
        */

        /*
        public static Func<IRequestWithValidationContext<TI, TR, TC>, Task<Result<TR>>> DecorateAPIBehavior<TI, TR, TC>(
            this Func<IRequestWithValidationContext<TI, TR, TC>, Task<Result<TR>>> apiFuncToBeDecorated)
        {
            return apiFuncToBeDecorated;
            //   .DecorateQueryWithValidation();
            //.DecorateWithExecutionTimeLogger((input) => input.Context.FrameworkContext.APITrackingInfo.IsAPITrackingPerformanceEnabled)
            //.DecorateWithAPIInputOutputLogger((input) => input.Context.FrameworkContext.APITrackingInfo.IsAPITrackingInputEnabled || input.Context.FrameworkContext.APITrackingInfo.IsAPITrackingOutputEnabled || input.Context.FrameworkContext.APITrackingInfo.IsTrackingExceptionEnabled);
        }

        public static Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>> DecorateWithValidation<TI, TR, TC>(this IRequestWithValidationContext<TI, TR, TC> requestWithValidationContext)
        {
            return (IRequestContext<TI, TR, TC> inputWithContext) =>
            {
                var mayBeValidationMessage = requestWithValidationContext.RequestInfo.ValidationFunc(inputWithContext);
                if (mayBeValidationMessage.TryGetValue(out var validationMessage))
                {
                    return Task.FromResult(new Result<TR>(ValidationException.GetValidationExceptionFromData<TI>(validationMessage)));
                }
                else
                {
                    return requestWithValidationContext.RequestInfo.ProcessRequestFunc(inputWithContext);
                }
            };
        }
        */

        public static Func<IRequestContext<TI, TR,TC>, Task<Result<TR>>> DecorateWithExecutionTimeLogger<TI, TR,TC>(this Func<IRequestContext<TI, TR,TC>, Task<Result<TR>>> funcToBeDecorated) =>
            funcToBeDecorated.PipeLineDecorateFunc<Stopwatch, IRequestContext<TI, TR,TC>, Task<Result<TR>>>(
                (input) =>
                {
                    var sw = new Stopwatch();
                    return sw;
                }
                , (sw, input) =>
                {
                    sw.Start();
                    return new MayBe<Task<Result<TR>>>(MayBeDataState.DataNotPresent);
                }
                , (sw, input, previousResultValue) =>
                {
                    var elapsedMillisecond = sw.ElapsedMilliseconds;
                    sw.Stop();
                    var logData = new LogDataInfoWithInputOutputDataAndTiming(input.Context,input.RequestInfo, elapsedMillisecond);
                    input.Context.LogRequestProcessingTime(logData);
                    return previousResultValue.GetValueThrowExceptionIfExceptionPresent();
                });


        /*
        public static Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>> DecorateRequestWithValidation<TI, TR, TC>(
            this Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>> funcToBeDecorated,
            Func<IRequestContext<TI, TR, TC>, MayBe<ValidationMessage<TI>>> validationFunc)
            =>
                funcToBeDecorated.PipeLineDecorateFunc<int, IRequestContext<TI, TR, TC>, Task<Result<TR>>>(
                    (input) => 0
                    , (sw, input) =>
                    {
                        var mayBeValidationMessage = validationFunc(input);
                        if (mayBeValidationMessage.TryGetValue(out var validationMessage))
                        {
                            return
                                new MayBe<Task<Result<TR>>>(Task.FromResult(new Result<TR>(ValidationException.GetValidationExceptionFromData<TI>(validationMessage))));
                        }
                        else
                        {
                            return GetNothingMaybe<Task<Result<TR>>>();
                        }
                    }
                    , (sw, input, previousResultValue) => previousResultValue.GetValueThrowExceptionIfExceptionPresent()
                );

                */

        public static Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>> DecorateRequestWithInputOutputLogging<TI, TR, TC>(
            this Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>> funcToBeDecorated,
            Func<object,string> serializeFunc)
            =>
                funcToBeDecorated.PipeLineDecorateFunc<int, IRequestContext<TI, TR, TC>, Task<Result<TR>>>(
                    (input) => 0
                    , (sw, input) => GetNothingMaybe<Task<Result<TR>>>()
                    , (sw, input, previousResultValue) =>
                    {
                        var serializedInputData = serializeFunc(input.RequestInfo);
                        if (previousResultValue.TryGetResult(out var previousResultTask))
                        {
                            var taskResult = previousResultTask.Result;
                            if (taskResult.TryGetResult(out var finalResultValue))
                            {
                                var logData = new LogDataInfoWithInputOutputData(input.Context, input.RequestInfo, finalResultValue,null);
                                input.Context.LogRequestInputOutput(logData);
                            }
                            else
                            {
                                var logData = new LogDataInfoWithInputOutputData(input.Context, input.RequestInfo, finalResultValue, null);
                                input.Context.LogRequestInputOutput(logData);
                            }
                        }
                        return previousResultValue.GetValueThrowExceptionIfExceptionPresent();
                    }
                );

    }
}
