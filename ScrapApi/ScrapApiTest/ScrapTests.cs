using Microsoft.AspNetCore.Mvc;
using ScrapApi.Controllers;
using ScrapApi.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ScrapApiTest
{
    public class ScrapTests
    {
        [Fact]
        public async Task CollectDataTesteSuccess()
        {
            var uriSite = new Uri("https://github.com/pixijs/pixi.js");
            var service = new ScrapService(uriSite);

            var result = await service.CollectData();

            Assert.True(result.Any());
        }


        [Fact]
        public async Task CollectDataBadUrlFail()
        {
            ScrapController scrapController = new ScrapController();

            var result = await scrapController.ScrapRepository("C:\\https://github.com/pixijs/pixi.js");


            //var uriSite = new Uri("C:\\https://github.com/pixijs/pixi.js");
            //var service = new ScrapService(uriSite);

            //var result = await service.CollectData();

            Assert.IsType<BadRequestObjectResult>(result);
            Assert.True(((BadRequestObjectResult)result).StatusCode == 400);
        }
    }
}
