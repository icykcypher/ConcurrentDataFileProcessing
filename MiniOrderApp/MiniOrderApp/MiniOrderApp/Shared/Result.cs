using System.ComponentModel.DataAnnotations;

namespace MiniOrderApp.Shared;

public enum ErrorStatus
{
        None = 0,            
        ValidationError,     
        Conflict,            
        NotFound,            
        Unauthorized,        
        Forbidden,           
        ServerError  
}

public class Result<T> 
{
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string? ErrorMessage { get; }
        public ErrorStatus? Status { get; }

        private Result(T value)
        {
                IsSuccess = true;
                Value = value;
                Status = ErrorStatus.None;
        }

        private Result(string errorMessage, ErrorStatus status)
        {
                IsSuccess = false;
                ErrorMessage = errorMessage;
                Status = status;
        }

        public static Result<T> Success(T value) => new(value);

        public static Result<T> Failure(string error, ErrorStatus status) => new(error, status);

        public static Result<T> ValidationFailure(List<ValidationResult> errors)
        {
                var errorMessage = string.Join("; ", errors.Select(e => e.ErrorMessage));
                
                return new Result<T>(errorMessage, ErrorStatus.ValidationError);
        }
}