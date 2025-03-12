using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace framework.DTO.BaseDTO.GenericRequest
{
    public class ReqByCodeObj
    {
        [FromQuery(Name = "code")]
        [JsonPropertyName("code")] 
        [Required(AllowEmptyStrings = false, ErrorMessage = "Code cannot be null")]
        [StringLength(50, ErrorMessage = "Code cannot be more than {1}")]
        public required string Code { get; set; }
    }
}
