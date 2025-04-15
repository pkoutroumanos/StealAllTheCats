using Microsoft.AspNetCore.Mvc;
using StealAllTheCats.Dtos;
using StealAllTheCats.Services;

namespace StealAllTheCats.Controllers
{
    /// <summary>
    /// Controller responsible for handling cat-related API requests.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CatsController : ControllerBase
    {
        private readonly ICatApiService _catApiService;
        private readonly ICatQueryService _catQueryService;
        private readonly ILogger<CatsController> _logger;

        public CatsController(ICatApiService catApiService, ICatQueryService catQueryService, ILogger<CatsController> logger)
        {
            _catApiService = catApiService;
            _catQueryService = catQueryService;
            _logger = logger;
        }

        /// <summary>
        /// Fetches 25 cat images from the external API and saves them to the database.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating success with a message, or an error status code if an exception occurs.
        /// </returns>
        [HttpPost("fetch")]
        public async Task<IActionResult> FetchCats()
        {
            try
            {
                _logger.LogInformation("Initiating FetchCats endpoint.");
                await _catApiService.FetchCatsAsync(25);
                return Ok(new { message = "25 cats fetched and saved!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in FetchCats endpoint.");
                return StatusCode(500, "An error occurred while fetching cats.");
            }
        }

        /// <summary>
        /// Retrieves a paged list of cats, optionally filtered by a tag.
        /// </summary>
        /// <param name="page">The page number (must be a positive integer).</param>
        /// <param name="pageSize">The number of items per page (must be a positive integer).</param>
        /// <param name="tag">An optional tag to filter cats.</param>
        /// <returns>
        /// An <see cref="ActionResult{PagedResult{CatDto}}"/> containing the paged list of cats, 
        /// or a bad request status if the input parameters are invalid.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<PagedResult<CatDto>>> GetCats([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? tag = null)
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    _logger.LogWarning("Invalid paging parameters: page={Page}, pageSize={PageSize}", page, pageSize);
                    return BadRequest("Page and pageSize must be positive integers.");
                }
                var result = await _catQueryService.GetCatsAsync(page, pageSize, tag);
                _logger.LogInformation("Returning paged result: Page {Page}, PageSize {PageSize}, Tag {Tag}.", page, pageSize, tag ?? "all");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving cats.");
                return StatusCode(500, "An error occurred while retrieving cats.");
            }
        }

        /// <summary>
        /// Retrieves the details of a cat specified by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the cat.</param>
        /// <returns>
        /// An <see cref="ActionResult{CatDto}"/> containing the cat details if found; otherwise, a NotFound result.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CatDto>> GetCatById(int id)
        {
            try
            {
                var cat = await _catQueryService.GetCatByIdAsync(id);
                if (cat == null)
                {
                    _logger.LogWarning("Cat with id {Id} not found.", id);
                    return NotFound();
                }
                _logger.LogInformation("Returning cat details for id {Id}.", id);
                return Ok(cat);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving cat with Id {Id}.", id);
                return StatusCode(500, "An error occurred while retrieving the specific cat.");
            }
        }
    }
}
