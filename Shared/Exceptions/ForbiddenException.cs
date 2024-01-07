using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WingetNexus.Shared.Exceptions
{
    /// <summary>
    /// This is for exceptions that occur when validating client certificates.
    /// </summary>
    public class ForbiddenException : Exception
    {
        /// <summary>
        /// Gets or sets internal error.
        /// </summary>
        public InternalRestError InternalRestError { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
        /// </summary>
        public ForbiddenException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public ForbiddenException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
        /// </summary>
        /// <param name="internalRestError">Internal Rest Error.</param>
        public ForbiddenException(InternalRestError internalRestError)
            : base(internalRestError.ErrorMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="internalRestError">Internal Rest Error.</param>
        public ForbiddenException(string message, InternalRestError internalRestError)
            : base(message)
        {
            this.InternalRestError = internalRestError;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="inner">Inner exception.</param>
        public ForbiddenException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="inner">Inner exception.</param>
        /// <param name="internalRestError">Internal Rest Error.</param>
        public ForbiddenException(string message, Exception inner, InternalRestError internalRestError)
            : base(message, inner)
        {
            this.InternalRestError = internalRestError;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
        /// </summary>
        /// <param name="exception">Exception.</param>
        public ForbiddenException(Exception exception)
            : base(exception.Message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
        /// </summary>
        /// <param name="exception">Exception.</param>
        /// <param name="internalRestError">Internal Rest Error.</param>
        public ForbiddenException(Exception exception, InternalRestError internalRestError)
            : base(exception.Message)
        {
            this.InternalRestError = internalRestError;
        }
    }
}
