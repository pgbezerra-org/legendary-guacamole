using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using webserver.Data;
using webserver.Models;
using Newtonsoft.Json;

namespace webserver.Controllers;

/// <summary>
/// Controller class for Real Estate CRUD requests via the HTTP API
/// Responses are sent only in JSON
/// </summary>
[Authorize(Roles=Common.BZE_Role+","+Common.Company_Role)]
[ApiController]
[Route("api/v1/realestates")]
[Produces("application/json")]
public class RealEstatesController : ControllerBase {
    
    private readonly WebserverContext _context;

    /// <summary>
    /// Real Estates API Controller Constructor. Only the IdentityDbContext is needed for it
    /// </summary>
    public RealEstatesController(WebserverContext context) {
        _context = context;
    }

    /// <summary>
    /// Get the Real Estate with the given Id
    /// </summary>
    /// <returns>Real Estate with the given Id</returns>
    /// <response code="200">Real Estate object</response>
    /// <response code="404">If there is no Real Estate with the given Id</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> ReadRealEstate(int id){

        var realEstate = await _context.RealEstates.FindAsync(id);
        if(realEstate==null){
            return NotFound();
        }

        return Ok(JsonConvert.SerializeObject(realEstate));
    }

    /// <summary>
    /// Get an array of Real Estates, with optional filters
    /// </summary>
    /// <returns>Real Estates Array</returns>
    /// <param name="minPrice">MinimumPrice filter</param>
    /// <param name="maxPrice">Maximum Price filter</param>
    /// <param name="offset">Offsets the result by a given amount</param>
    /// <param name="limit">Limits the number of results</param>
    /// <param name="sort">Orders the result by a given field. Does not order if the field does not exist</param>
    /// <response code="200">Returns an array of Real Estates</response>
    /// <response code="404">No Real Estates matching the given filters were found</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Company>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [HttpGet]
    public async Task<IActionResult> ReadRealEstates(int? minPrice, int? maxPrice, int? offset, int limit, string sort = "Id") {

        if(limit<1){
            return BadRequest("Limit parameter must be a natural number greater than 0");
        }

        var realEstates = _context.RealEstates.AsQueryable();

        if (minPrice.HasValue) {
            realEstates = realEstates.Where(re => re.Price >= minPrice);
        }
        if (maxPrice.HasValue) {
            realEstates = realEstates.Where(re => re.Price <= maxPrice);
        }

        sort = sort.ToLower();
        switch (sort) {
            case "name":
                realEstates = realEstates.OrderBy(re => re.Name);
                break;
            case "price":
                realEstates = realEstates.OrderBy(re => (double) re.Price);
                break;
            case "companyid":
                realEstates = realEstates.OrderBy(re => re.CompanyId);
                break;
            case "address":
                realEstates = realEstates.OrderBy(re => re.Address);
                break;
        }

        if(offset.HasValue){
            realEstates = realEstates.Skip(offset.Value);
        }
        realEstates = realEstates.Take(limit);

        var resultArray = await realEstates.ToArrayAsync();

        if(resultArray.Length==0){
            return NotFound();
        }

        return Ok(JsonConvert.SerializeObject(resultArray));
    }

    /// <summary>
    /// Creates a Real Estate
    /// </summary>
    /// <returns>The created Real Estate</returns>
    /// <response code="200">Created Real Estate</response>
    /// <response code="400">Bad Request, OwnerCompany does not exist</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Company))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadRequestObjectResult))]
    [HttpPost]
    public async Task<IActionResult> CreateRealEstate([FromBody]RealEstate request) {
        
        if (_context.Company.Find(request.CompanyId) == null) {
            return BadRequest("Owner Company does Not Exist!");
        }else{
            request.OwnerCompany = _context.Company.Find(request.CompanyId);
        }
        
        var newState = _context.RealEstates.Add(request).Entity;
        await _context.SaveChangesAsync();

        return Ok(JsonConvert.SerializeObject(newState));
    }

    /// <summary>
    /// Updates the RealEstate with the given Id and data.
    /// Does not change the Id and OwnerCompany, they are not meant to.
    /// </summary>
    /// <returns>RealEstate with the updated Data</returns>
    /// <response code="200">RealEstate with the updated data</response>
    /// <response code="404">NotFound result if the Id was not found</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Company))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundObjectResult))]
    [HttpPatch]
    public async Task<IActionResult> UpdateRealEstate([FromBody]RealEstate upRE) {

        var realEstate = _context.RealEstates.Find(upRE.Id);
        if(realEstate == null){
            return NotFound();
        }

        upRE.OwnerCompany = realEstate.OwnerCompany;
        
        realEstate = upRE;

        await _context.SaveChangesAsync();

        return Ok(JsonConvert.SerializeObject(realEstate));    
    }

    /// <summary>
    /// Deletes the RealEstate with the given Id
    /// </summary>
    /// <returns>NoContent if successfull</returns>
    /// <response code="200">RealEstate was found, and thus deleted</response>
    /// <response code="400">RealEstate not found</response>
    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(Company))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadRequestObjectResult))]
    [HttpDelete("id")]
    public async Task<IActionResult> DeleteRealEstate(int id) {

        var RE = _context.RealEstates.Find(id);
        if(RE==null) {
            return NotFound();
        }

        _context.RealEstates.Remove(RE);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}