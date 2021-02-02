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
                    var validator = new QueryValidator();
                    var validationResult = validator.Validate(this);
                    return validationResult.IsValid
                    ? new MayBe<ValidationException>(MayBeDataState.DataNotPresent)
                    : new MayBe<FluentValidation.ValidationException>(new ValidationException(validationResult.Errors));
                    
                };

            public Task<Model> Process(IAPIContext<ContosoContext> context) => this.ProcessRequest(context);

            class QueryValidator : AbstractValidator<Query>
            {
                public QueryValidator()
                {
                    RuleFor(m => m.Data).NotNull();
                }
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
        
    }
}