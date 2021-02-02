using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CommandDecoratorExtension;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Requests;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RequestDecorator;
using RequestDecorator.Functional;
using ValidationException = FluentValidation.ValidationException;

namespace ContosoUniversity.Pages.Instructors
{
    public class Details : PageModel
    {
        private readonly IMediator _mediator;
        private readonly APIContext<ContosoContext> _apiContext;

        public Details(IMediator mediator, SchoolContext db, IConfigurationProvider configuration, APIContext<ContosoContext> apiContext)
        {
            _mediator = mediator;
            _apiContext = apiContext;
        }

        public Model Data { get; private set; }
        //new ContosoAPIContext(new ContosoContext(_db, _configuration))
        /*
        public async Task OnGetAsync(Query query) =>
            Data = await Task.FromResult(query.Process(_apiContext)
                .Result.TryGetResult(out var m)
                ? m
                : null);
                */
        //await Task.FromResult((Model)null) ; //await _mediator.Send(query);

        //public async Task OnGetAsync(Query query) => Data = await query.ProcessRequest(_apiContext);
        public async Task OnGetAsync(Query query) => Data = await query.Process(_apiContext);


        public class Query : IRequestWithFluentValidator<int?, Model, ContosoContext> 
        {
            public int? Id { get; set; }
            public int? Data => Id;
            public Func<IRequestContext<int?, Model, ContosoContext>, Task<Result<Model>>> ProcessRequestFunc 
                => GetInstructorByIDRequest.ProcessFunc;
            public Func<IRequestContext<int?, Model, ContosoContext>, MayBe<ValidationException>> ValidationFunc
                => (reqContext) =>
                {
                    QueryValidator validator = new QueryValidator();
                    var validationResult = validator.Validate(this);
                    if (validationResult.IsValid)
                    {
                        return new MayBe<ValidationException>(MayBeDataState.DataNotPresent);
                    }
                    else
                    {
                        var validationException = new ValidationException(validationResult.Errors);
                        return new MayBe<FluentValidation.ValidationException>(validationException);
                    }
                };

            public Task<Model> Process(IAPIContext<ContosoContext> context) => 
                ((IRequestWithFluentValidator<int?, Model, ContosoContext>)this).InterfaceProcess(context);

            internal class QueryValidator : AbstractValidator<Query>
            {
                public QueryValidator()
                {
                    RuleFor(m => m.Id).NotNull();
                }
            }
            
        }

        


        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                //RuleFor(m => m.Id).GreaterThan(100);
                RuleFor(m => m.Id).NotNull();
            }
        }

        public record Model
        {
            public int? Id { get; init; }

            public string LastName { get; init; }
            [Display(Name = "First Name")]
            public string FirstMidName { get; init; }

            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
            public DateTime? HireDate { get; init; }

            [Display(Name = "Location")]
            public string OfficeAssignmentLocation { get; init; }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Instructor, Model>();
        }
        /*
        public class Handler : IRequestHandler<Query, Model>
        {
            private readonly SchoolContext _db;
            private readonly IConfigurationProvider _configuration;

            public Handler(SchoolContext db, IConfigurationProvider configuration)
            {
                _db = db;
                _configuration = configuration;
            }

            public Task<Model> Handle(Query message, CancellationToken token) => _db
                .Instructors
                .Where(i => i.Id == message.Id)
                .ProjectTo<Model>(_configuration)
                .SingleOrDefaultAsync(token);
        }
        */
    }
}