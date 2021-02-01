using System;
using System.Collections.Generic;
using System.Text;

namespace RequestDecorator
{
    public static class ValidationException
    {
        public static ValidationException<T> GetValidationExceptionFromData<T>(ValidationMessage<T> data)
        {
            return new ValidationException<T>(data);
        }
    }

    public interface IValidationException
    {
        string GetValidationErrorMessage();
    }

    public class ValidationException<T> : ApplicationException, IValidationException
    {
        public ValidationMessage<T> ValidationMessage { get; }

        public ValidationException(ValidationMessage<T> validationMessage)
        {
            ValidationMessage = validationMessage ?? throw new ArgumentNullException(nameof(validationMessage));
        }

        public string GetValidationErrorMessage() => ValidationMessage.ErrorMessage;

    }
}
