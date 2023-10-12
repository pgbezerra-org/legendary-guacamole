using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using webserver.Data;
using webserver.Utilities;
using webserver.Models;
using Newtonsoft.Json;

namespace webserver.Controllers;

[Authorize(Roles=Common.BZE_Role+","+Common.Company_Role)]
[ApiController]
[Route("api/v1/realestates")]
public class RealEstatesController : ControllerBase {
    
    private readonly WebserverContext _context;

    public RealEstatesController(WebserverContext context) {
        _context = context;
    }

    [HttpGet("unique/{id}")]
    public async Task<IActionResult> ReadRealEstate(int id){

        var realEstate = await _context.RealEstates.FindAsync(id);
        if(realEstate==null){
            return NotFound();
        }

        return Ok(JsonConvert.SerializeObject(realEstate));
    }

    [HttpGet]
    public async Task<IActionResult> ReadRealEstates(int? minPrice, int? maxPrice, int? offset, int? limit, string sort = "Id") {

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
            default:
                realEstates = realEstates.OrderBy(re => re.Id);
                break;
        }

        if(offset.HasValue){
            realEstates = realEstates.Skip(offset.Value);
        }
        if(limit.HasValue){
            realEstates = realEstates.Take(limit.Value);
        }

        var resultArray = await realEstates.ToArrayAsync();

        if(resultArray==null || resultArray.Length==0){
            return NotFound();
        }

        return Ok(JsonConvert.SerializeObject(resultArray));
    }

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