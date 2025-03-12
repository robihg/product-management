using ProductManagement.DataAccess.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace framework.DTO.BaseDTO.GenericResponse
{
    public class ResGeneric<T>
    {
        public required HeaderObj HeaderObj { get; set; }
        public T Data { get; set; }
    }
}
