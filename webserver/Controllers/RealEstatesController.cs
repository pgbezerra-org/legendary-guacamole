using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webserver.Data;
using webserver.Models;
using webserver.Models.DTOs;
using Newtonsoft.Json;

namespace webserver.Controllers;

[ApiController]
[Route("api/v1/realestates")]
public class RealEstatesController : ControllerBase {
    
    private readonly WebserverContext _context;

    public RealEstatesController(WebserverContext context) {
        _context = context;
    }

    [HttpGet("unique/{id}")]
    public IActionResult ReadRealEstate(int id){

        var realEstate = _context.RealEstates.Find(id);
        if(realEstate==null){
            return NotFound();
        }

        var realEstateDto = JsonConvert.SerializeObject( new RealEstateDTO (id, realEstate.Name, realEstate.Address, realEstate.Price, realEstate.CompanyId));

        return Ok(realEstateDto);
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
                realEstates = realEstates.OrderBy(re => re.Price);
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
            realEstates=realEstates.Skip(offset.Value);
        }
        if(limit.HasValue){
            realEstates=realEstates.Take(limit.Value);
        }

        var resultArray = await realEstates.ToArrayAsync();
        var resultDtoArray = realEstates.Select(r=>(RealEstateDTO)r).ToArray();

        if(resultDtoArray==null){
            return NotFound();
        }

        var resultSerial = JsonConvert.SerializeObject(resultDtoArray);

        return Ok(resultSerial);
    }

    [HttpPost]
    public IActionResult CreateRealEstate([FromBody]RealEstateDTO request ) {

        if(_context.RealEstates.Find(request.Id) != null) {
            return BadRequest("Real Estate Already Exists!");
        }
        
        if (_context.Company.Find(request.CompanyId) == null) {
            return BadRequest("Owner Company does Not Exist!");
        }
        
        _context.RealEstates.AddAsync((RealEstate)request);
        _context.SaveChangesAsync();

        var realEstateDto = JsonConvert.SerializeObject( new RealEstateDTO (request.Id, request.Name, request.Address, request.Price, request.CompanyId));

        return Ok(realEstateDto);
    }

    [HttpPatch]
    public IActionResult UpdateRealEstate([FromBody]RealEstateDTO upRE) {

        var realEstate = _context.RealEstates.Find(upRE.Id);
        if(realEstate == null){
            return NotFound();
        }
        
        realEstate.Address = upRE.Address;
        realEstate.Price = upRE.Price;
        realEstate.Name = upRE.Name;

        _context.SaveChanges();
        
        var realEstateDto = JsonConvert.SerializeObject( new RealEstateDTO (upRE.Id, upRE.Name, upRE.Address, upRE.Price, upRE.CompanyId));

        return Ok(realEstateDto);    
    }

    [HttpDelete("id")]
    public IActionResult DeleteRealEstate(int id) {

        var RE = _context.RealEstates.Find(id);
        if(RE==null) {
            return NotFound();
        }

        _context.RealEstates.Remove(RE);
        _context.SaveChanges();
        
        return NoContent();
    }
}