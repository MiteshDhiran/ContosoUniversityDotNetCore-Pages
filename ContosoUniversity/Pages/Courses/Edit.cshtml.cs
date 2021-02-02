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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RequestDecorator;
using RequestDecorator.Functional;
using ValidationException = FluentValidation.ValidationException;

namespace ContosoUniversity.Pages.Courses
{
    public class Edit : PageModel
    {
        private readonly APIContext<ContosoContext> _apiContext;

        [BindProperty]
        public Command Data { get; set; }

        public Edit(APIContext<ContosoContext> apiContext) => _apiContext = apiContext;

        //public async Task OnGetAsync(Query query) => Data = await query.Process(_apiContext);
        public async Task OnGetAsync(Query query)
        {
            var queryRequest = new QueryRequest(query);
            Data = await queryRequest.Process(_apiContext);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var commandRequest = new CommandRequest(Data);
            await commandRequest.Process(_apiContext);
            return this.RedirectToPageJson(nameof(Index));
        }

        public record Query 
        {
            public int? Id { get; init; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(m => m.Id).NotNull();
            }
        }

        public class QueryRequest : IRequestWithFluentValidator<Query, Command, ContosoContext>
        {
            public QueryRequest(Query data)
            {
                Data = data;
            }

            public System.Func<IRequestContext<Query, Command, ContosoContext>, MayBe<FluentValidation.ValidationException>> ValidationFunc 
                => (reqContext) =>
                {
                    var validator = new QueryValidator();
                    var validationResult = validator.Validate(reqContext.RequestInfo.Data);
                    return validationResult.IsValid
                        ? new MayBe<FluentValidation.ValidationException>(MayBeDataState.DataNotPresent)
                        : new MayBe<FluentValidation.ValidationException>(new FluentValidation.ValidationException(validationResult.Errors));

                };
            public Query Data { get; }

            public System.Func<IRequestContext<Query, Command, ContosoContext>, Task<Result<Command>>> ProcessRequestFunc
                => (req) =>
                {
                    var r =  req.Context.ContextInfo.DbContext.Courses
                        .Where(c => c.Id == req.RequestInfo.Data.Id)
                        .ProjectTo<Command>(req.Context.ContextInfo.Configuration)
                        .SingleOrDefault();
                    return Task.FromResult(new Result<Command>(r)) ;
                };

            public Task<Command> Process(IAPIContext<ContosoContext> context) => this.ProcessRequest(context);
        }
        

        public record Command 
        {
            [Display(Name = "Number")]
            public int Id { get; init; }
            public string Title { get; init; }
            public int? Credits { get; init; }
            public Department Department { get; init; }
            public Command Data => this;
        }

        public class CommandRequest : IRequestWithFluentValidator<Command, Unit, ContosoContext>
        {
            public Command Data { get; }

            public Func<IRequestContext<Command, Unit, ContosoContext>, Task<Result<Unit>>> ProcessRequestFunc => req =>
            {
                return Task.Run(() =>
                {
                    var request = req.RequestInfo.Data;
                    var course = req.Context.ContextInfo.DbContext.Courses.FindAsync(request.Id).Result;
                    course.Title = request.Title;
                    course.Department = request.Department;
                    course.Credits = request.Credits!.Value;
                    return new Result<Unit>(new Unit());
                });
            };
            public Task<Unit> Process(IAPIContext<ContosoContext> context) => this.ProcessRequest(context);

            public Func<IRequestContext<Command, Unit, ContosoContext>, MayBe<ValidationException>> ValidationFunc
                => (reqContext) =>
                {
                    var validator = new CommandValidator();
                    var validationResult = validator.Validate(Data);
                    return validationResult.IsValid
                        ? new MayBe<FluentValidation.ValidationException>(MayBeDataState.DataNotPresent)
                        : new MayBe<FluentValidation.ValidationException>(new FluentValidation.ValidationException(validationResult.Errors));
                };

            public CommandRequest(Command data)
            {
                Data = data;
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile() => CreateMap<Course, Command>();
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(m => m.Title).NotNull().Length(3, 50);
                RuleFor(m => m.Credits).NotNull().InclusiveBetween(0, 5);
            }
        }

        
    }
}