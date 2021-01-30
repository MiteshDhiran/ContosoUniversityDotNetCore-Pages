/*
using System;
using System.Threading.Tasks;

namespace ContosoUniversity.Infrastructure
{
    public class Result<T>
    {
        private T Data { get; set; }

        private Exception Exception { get; set; }

        public bool TryGetResult(out T res)
        {
            if (this.Exception == null)
            {
                res = Data;
                return true;
            }
            else
            {
                res = default(T);
                return false;
            }
        }

        public Result(T data)
        {
            Data = data;
            Exception = null;
        }

        public static Result<T> GetResultFromData(T data)
        {
            return new Result<T>(data);
        }

        public Result(Exception ex)
        {
            this.Exception = ex ?? throw new ArgumentNullException(nameof(ex));
            Data = default(T);
        }

        public void Match(Action<T> onSuccessAction, Action<Exception> onExceptionAction)
        {
            if (Exception == null)
            {
                onSuccessAction(Data);
            }
            else
            {
                onExceptionAction(this.Exception);
            }
        }


        public TO Select<TO>(Func<T, TO> onSuccessFunc, Func<Exception, TO> onExceptionFunc)
        {
            return Exception == null ? onSuccessFunc(Data) : onExceptionFunc(this.Exception);
        }
    }

    public interface IRequest<TI, TR, TC>
    {
        TI Data { get; }
        Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>> ProcessRequestFunc { get; }
    }

    public interface IRequestContext<TI, TR, TC>
    {
        IAPIContext<TC> Context { get; }
        IRequest<TI, TR, TC> RequestInfo { get; }
    }

    public interface IAPIContext<TC>
    {
        TC ContextInfo { get; }
    }

    public interface IRequestContext<out TI, TC>
    {
        IAPIContext<TC> Context { get; }

        TI Data { get; }
    }

    public enum RequestType
    {
        Query,
        Mutation,
        Subscription
    }

    public enum MayBeDataState
    {
        DataPresent,
        DataNotPresent
    }

    public class MayBe<T>
    {
        private T Data { get; }

        private MayBeDataState DataState { get; }


        public MayBe(T data)
        {
            Data = data;
            DataState = MayBeDataState.DataPresent;
        }


        public MayBe(MayBeDataState dataAbsent)
        {
            if (dataAbsent != MayBeDataState.DataNotPresent)
                throw new InvalidOperationException($"Parameter should always be MayBeDataState.DataNotPresent");
            DataState = MayBeDataState.DataNotPresent;
        }


        public bool TryGetValue(out T result)
        {
            if (DataState == MayBeDataState.DataPresent)
            {
                result = Data;
                return true;
            }
            else
            {
                result = default(T);
                return false;
            }
        }
    }

    public static class MaybeExtension
    {
        public static MayBe<T> GetNothingMaybe<T>() => new MayBe<T>(MayBeDataState.DataNotPresent);
    }

    public class ValidationMessage<T>
    {
        /// <summary>
        /// Data on which validation was performed
        /// </summary>
        public T Data { get; }

        /// <summary>
        /// Error Message
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="errorMessage"></param>
        public ValidationMessage(T data, string errorMessage)
        {
            Data = data;
            ErrorMessage = errorMessage;
        }
    }

    public static class ValidationHelper
    {
        public static MayBe<ValidationMessage<T>> GetValidationMessageFromData<T>(T data, string validationMessage)
        {
            return new MayBe<ValidationMessage<T>>(new ValidationMessage<T>(data, validationMessage));
        }

        public static MayBe<ValidationMessage<T>> GetNoValidationError<T>(T data)
        {
            return MaybeExtension.GetNothingMaybe<ValidationMessage<T>>();
        }
    }

    public class RequestContext<TI, TR, TC> : IRequestContext<TI, TR, TC>
    {
        public IAPIContext<TC> Context { get; }
        public IRequest<TI, TR, TC> RequestInfo { get; }

        public RequestContext(IAPIContext<TC> context, IRequest<TI, TR, TC> requestInfo)
        {
            Context = context;
            RequestInfo = requestInfo;
        }

        public Task<Result<TR>> ProcessRequest()
        {
            //cached decoration if static this.RequestInfo.ProcessRequestFunc.Method.IsStatic ?
            //return this.RequestInfo.ProcessRequestFunc.DecorateAPIBehavior()(this);
            return RequestProcessor.ProcessRequest(this);
        }
    }

    public class QueryRequest<TI, TR, TC> : IRequest<TI, TR, TC>
    {
        public TI Data { get; }
        public RequestType RequestType => RequestType.Query;
        public Func<IRequestContext<TI, TR, TC>, MayBe<ValidationMessage<TI>>> ValidateFunc { get; }

        public Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>> ProcessRequestFunc { get; }

        public Task<Result<TR>> Process(IAPIContext<TC> context)
        {
            var requestContext = new RequestContext<TI, TR, TC>(context, this);
            return RequestProcessor.ProcessRequest(requestContext);
        }

        public QueryRequest(TI data, Func<IRequestContext<TI, TR, TC>, MayBe<ValidationMessage<TI>>> validateFunc,
            Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>> generateResponseFunc)
        {
            Data = data;
            ValidateFunc = validateFunc ?? throw new ArgumentNullException();
            ProcessRequestFunc = generateResponseFunc ?? throw new ArgumentNullException();
        }
    }

    public static class QueryRequestExtension
    {
        public static QueryRequest<TI, TR, TC> GetQueryRequest<TI, TR, TC>(TI data
            , Func<IRequestContext<TI, TR, TC>, Task<Result<TR>>> processFunc
            , Func<IRequestContext<TI, TR, TC>, MayBe<ValidationMessage<TI>>> validateFunc)
        {
            return new QueryRequest<TI, TR, TC>(data, validateFunc, processFunc);
        }
    }

    public static class RequestProcessor
    {
        public static Task<Result<TR>> ProcessRequest<TI, TR, TC>(IRequestContext<TI, TR, TC> inputRequestWithContext)
        {
            return inputRequestWithContext.RequestInfo.ProcessRequestFunc(inputRequestWithContext);
        }
    }
}
*/