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
    public class ReqByGuidObj
    {
        [FromQuery(Name = "guid")]
        [JsonPropertyName("guid")]
        [Range(1, long.MaxValue, ErrorMessage = "The Field {0} must be greater than {1}")]
        public Guid Guid { get; set; }
    }
}
