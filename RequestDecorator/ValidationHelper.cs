using System;
using System.Collections.Generic;
using System.Text;
using RequestDecorator.Functional;

namespace RequestDecorator
{
    public static class ValidationHelper
    {
        public static MayBe<ValidationMessage<T>> GetValidationMessageFromData<T>(T data, string validationMessage)
        {
            return new MayBe<ValidationMessage<T>>(new ValidationMessage<T>(data, validationMessage));
        }

        public static MayBe<ValidationMessage<T>> GetNoValidationError<T>(T data)
        {
            return MayBeExtension.GetNothingMaybe<ValidationMessage<T>>();
        }
    }
}
