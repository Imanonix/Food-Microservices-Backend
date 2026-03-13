using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Presentation.Controllers
{
    public class ApiResponse
    {
        public object Data { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
    }

}

