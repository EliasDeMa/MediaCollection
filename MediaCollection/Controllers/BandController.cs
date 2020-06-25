using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaCollection.Data;
using Microsoft.AspNetCore.Mvc;

namespace MediaCollection.Controllers
{
    public class BandController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public BandController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
