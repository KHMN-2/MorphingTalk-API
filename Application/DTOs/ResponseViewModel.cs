using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ResponseViewModel<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public ResponseViewModel(T data, string message, bool success, int statusCode)
        {
            Data = data;
            Message = message;
            Success = success;
            StatusCode = statusCode;
        }
    }
}
