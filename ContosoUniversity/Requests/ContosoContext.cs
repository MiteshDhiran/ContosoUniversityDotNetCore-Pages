using System;
using System.Threading.Tasks;
using AutoMapper;
using ContosoUniversity.Data;
using ContosoUniversity.Infrastructure;
using RequestDecorator;

namespace ContosoUniversity.Requests
{
    public class ContosoContext
    {
        public SchoolContext DbContext { get; }
        public IConfigurationProvider Configuration { get; }
        public ContosoContext(SchoolContext dbContext, IConfigurationProvider configuration)
        {
            DbContext = dbContext;
            Configuration = configuration;
        }
    }

    public class ContosoAPIContext : IAPIContext<ContosoContext>
    {
        public ContosoContext ContextInfo { get; }
        public Guid TraceID { get; }
        public LogType EnvironmentLogLevel { get; }
        public Func<object, string> SerializerFunc { get; }
        public Action<LogDataInfoWithInputOutputData> LogRequestInputOutput { get; }
        public Action<LogDataInfoWithInputOutputDataAndTiming> LogRequestProcessingTime { get; }
        public Func<object, Result<string>> TrySerializerFunc => MethodExtension.TryExecuteFunc(SerializerFunc);
        public ContosoAPIContext(ContosoContext contextInfo, Action<LogDataInfoWithInputOutputData> log, Action<LogDataInfoWithInputOutputData> logRequestInputOutput, Action<LogDataInfoWithInputOutputDataAndTiming> logRequestProcessingTime, Func<object, string> serializerFunc)
        {
            ContextInfo = contextInfo;
            LogRequestInputOutput = logRequestInputOutput;
            LogRequestProcessingTime = logRequestProcessingTime;
            SerializerFunc = serializerFunc;
            TraceID = Guid.NewGuid();
            EnvironmentLogLevel = LogType.Info;
        }
        public ContosoAPIContext(ContosoContext contextInfo, LogType logType, Action<LogDataInfoWithInputOutputData> logRequestInputOutput, Action<LogDataInfoWithInputOutputDataAndTiming> logRequestProcessingTime, Func<object, string> serializerFunc)
        {
            ContextInfo = contextInfo;
            TraceID = Guid.NewGuid();
            EnvironmentLogLevel = logType;
            LogRequestInputOutput = logRequestInputOutput;
            LogRequestProcessingTime = logRequestProcessingTime;
            SerializerFunc = serializerFunc;
        }
        
    }
}