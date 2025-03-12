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
        [Required(ErrorMessage = "Guid is required.")]
        public Guid Guid { get; set; }
    }
}
