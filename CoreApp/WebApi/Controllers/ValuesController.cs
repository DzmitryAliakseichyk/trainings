using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models.KeyPhrases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ITextAnalyticsService _analyticsService;

        public ValuesController(IConfiguration configuration, ITextAnalyticsService analyticsService)
        {
            _configuration = configuration;
            _analyticsService = analyticsService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpPost]
        public async Task<KeyPhrasesResponseModel> Get(TextList textList)
        {
            var request = new KeyPhrasesRequestModel
            {
                Documents = textList.Text.Select(x => new KeyPhrasesRequestDocument
                {
                    Text = x
                }).ToList()
            };

            try
            {
                var response = await _analyticsService.GetKeyPhrasesAsync(request);
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public class TextList
        {
            public List<string> Text { get; set; }
        }
    }
}