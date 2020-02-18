using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ScrapApi.Models;
using ScrapApi.Services;
using ScrapApi.Utils;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScrapApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScrapController
    {
        /// <summary>
        /// Gets the data from a github repository.
        /// </summary>
        /// <param name="repositorySiteUrl">Github website url.</param>
        /// <returns>The detais of the repository content.</returns>
        [HttpGet]
        [Authorize]
        [Route("ScrapRepository")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IList<RepositoryDetailsModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<IActionResult> ScrapRepository(string repositorySiteUrl)
        {
            try
            {
                Uri repositoryUri;

                if (Uri.TryCreate(repositorySiteUrl, UriKind.Absolute, out repositoryUri))
                {
                    var scrapService = new ScrapService(repositoryUri);

                    var result = await scrapService.CollectData();

                    return new OkObjectResult(result);
                }
                else
                {
                    throw new ScrapException("MSG_0001");
                }
            }
            catch (ScrapException ex)
            {
                return new NotFoundObjectResult(JsonConvert.SerializeObject(new ExceptionModel(ex)));
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return new BadRequestObjectResult("Internal Server Error.");
            }
        }
    }
}
