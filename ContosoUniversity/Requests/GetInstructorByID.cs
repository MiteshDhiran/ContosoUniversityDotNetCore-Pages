using System;
using System.Linq;
using System.Threading.Tasks;
using ContosoUniversity.Infrastructure;
using ContosoUniversity.Pages.Instructors;
using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;
using RequestDecorator;
using RequestDecorator.Functional;

namespace ContosoUniversity.Requests
{
    public static class GetInstructorByIDRequest 
    {
        public static readonly Func<IRequestContext<int?, Details.Model, ContosoContext>, MayBe<RequestDecorator.ValidationMessage<int?>>>
            InstructorValidationFunc = ((req) => MayBeExtension.GetNothingMaybe<RequestDecorator.ValidationMessage<int?>>());

        public static readonly Func<IRequestContext<int?, Details.Model, ContosoContext>, Task<Result<Details.Model>>>
            ProcessFunc =
                (req) =>
                {
                    var dbContext = req.Context.ContextInfo.DbContext;
                    var instructor =
                        dbContext.Instructors.FirstOrDefault(i => i.Id == req.RequestInfo.Data);
                    if (instructor != null)
                    {
                        var model = new Details.Model()
                        {
                            Id = instructor.Id,
                            HireDate = instructor.HireDate,
                            LastName = instructor.LastName,
                            FirstMidName = instructor.FirstMidName,
                            OfficeAssignmentLocation = instructor.OfficeAssignment?.Location
                        };
                        return Task.FromResult<Result<Details.Model>>(new Result<Details.Model>(model));
                    }
                    else
                    {
                        return Task.FromResult<Result<Details.Model>>(new Result<Details.Model>(new ApplicationException("Instructor does not exists")));
                    }
                };
        
        
    }

    
}