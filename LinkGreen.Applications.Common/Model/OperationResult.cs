using System.Runtime.Serialization;

namespace LinkGreen.Applications.Common.Model
{
    [DataContract]
    public class OperationResult<T> : OperationResultBase
    {
        [DataMember]
        public T Result { get; set; }

        public OperationResult() { }

        [DataMember]
        public bool HasNextPage { get; set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult{T}"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public OperationResult(T result)
        {
            Success = true;
            Result = result;
        }
    }

    [DataContract]
    public class OperationResultBase
    {
        [DataMember]
        public string Error { get; set; }

        [DataMember]
        public bool Success { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResultBase" /> class.
        /// </summary>
        public OperationResultBase()
        {
            Success = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResultBase" /> class.
        /// </summary>
        /// <param name="error">The error.</param>
        public OperationResultBase(string error)
        {
            Success = false;
            Error = error;
        }
    }
}