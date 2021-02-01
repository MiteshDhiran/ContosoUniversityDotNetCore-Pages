using System;
using System.Collections.Generic;
using System.Text;
using RequestDecorator.Functional;

namespace RequestDecorator
{
    public static class MethodExtension
    {
        /// <summary>
        /// Enables decoration of function with before and after behavior
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <typeparam name="TI"></typeparam>
        /// <typeparam name="TO"></typeparam>
        /// <param name="funcToBeDecorated"></param>
        /// <param name="funcToInitializeState"></param>
        /// <param name="funcToBeExecutedBefore"></param>
        /// <param name="funcToBeExecutedAfter"></param>
        /// <returns></returns>
        public static Func<TI, TO> PipeLineDecorateFunc<TS, TI, TO>(this Func<TI, TO> funcToBeDecorated
            , Func<TI, TS> funcToInitializeState
            , Func<TS, TI, MayBe<TO>> funcToBeExecutedBefore
            , Func<TS, TI, Result<TO>, TO> funcToBeExecutedAfter)
        {
            return (i) =>
            {
                var state = funcToInitializeState(i);
                try
                {
                    var maybeResult = funcToBeExecutedBefore != null ? funcToBeExecutedBefore(state, i) : new MayBe<TO>(MayBeDataState.DataNotPresent);
                    var retVal = maybeResult.TryGetValue(out var res1) ? res1 : funcToBeDecorated(i);
                    var res = new Result<TO>(retVal);
                    funcToBeExecutedAfter(state, i, res);
                    return retVal;
                }
                catch (Exception ex)
                {
                    return funcToBeExecutedAfter(state, i, new Result<TO>(ex));
                    throw;
                }
            };
        }
    }
}
