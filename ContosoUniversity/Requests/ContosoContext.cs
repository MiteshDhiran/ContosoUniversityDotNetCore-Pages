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

        public ContosoAPIContext(ContosoContext contextInfo)
        {
            ContextInfo = contextInfo;
        }
    }
}