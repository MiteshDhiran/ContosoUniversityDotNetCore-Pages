using System;
using System.Collections.Generic;
using System.Text;

namespace RequestDecorator
{
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
}
