using Blazor.Ninja.Common.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.Ninja.KickStart.Common
{
    public class FileUpload : IdDataObject
    {
        [Required(ErrorMessage = "Please Enter Title")]
        public string Title { get; set; }
        public string ContentType { get; set; }

        [Required(ErrorMessage = "Please Select File")]
        public byte[] file {get; set;}


    }
}
