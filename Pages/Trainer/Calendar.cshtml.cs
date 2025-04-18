using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using LabTrainer.Data;
using LabTrainer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Markdig;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace LabTrainer.Pages.Trainer
{
     // [AllowAnonymous]
    public class CalendarModel : PageModel
    {
       
        public void OnGet()
        {
        }
    }
}
