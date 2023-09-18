using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webserver.Data;
using webserver.Models;

namespace webserver.Controllers;

[Route("realestates")]
[ApiController]
public class RealEstatesController : ControllerBase {
    
    private readonly WebserverContext _context;

    public RealEstatesController(WebserverContext context) {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> ReadRealEstate(int? minPrice, int? maxPrice, int? offset, int? limit, string sort = "Id") {

        var realEstates = _context.RealEstates.AsQueryable();

        // Filter by price
        if (minPrice.HasValue) {
            realEstates = realEstates.Where(re => re.Price >= minPrice);
        }
        if (maxPrice.HasValue) {
            realEstates = realEstates.Where(re => re.Price <= maxPrice);
        }

        sort = sort.ToLower();

        // Sort
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
            default:
                realEstates = realEstates.OrderBy(re => re.Id);
                break;
        }

        // Pagination
        if(offset.HasValue){
            realEstates=realEstates.Skip(offset.Value);
        }
        if(limit.HasValue){
            realEstates=realEstates.Take(limit.Value);
        }

        var result = await realEstates.ToListAsync();

        if (result == null) {
            return NotFound();
        }else {
            return Ok(realEstates);
        }
    }

    [HttpPost]
    public IActionResult CreateRealEstate(RealEstate request ) {

        if(_context.RealEstates.Find(request.Id)!=null){
            return BadRequest("Real Estate Already Exists!");
        }

        _context.RealEstates.Add(request);
        _context.SaveChanges();

        return Ok(request);
    }

    [HttpPatch("{id}")]
    public IActionResult UpdateClient(int id, RealEstate upRE){
        var realEstate=_context.RealEstates.Find(id);
        if(realEstate==null){
            return NotFound();
        }
        
        realEstate.Address=upRE.Name;
        realEstate.Price=upRE.Price;
        realEstate.Name=upRE.Name;

        _context.SaveChanges();
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteRealEstate(int id){
        var RE=_context.RealEstates.Find(id);
        if(RE==null){
            return NotFound();
        }
        var result=_context.RealEstates.Remove(RE);
        if(result == null){
            return NotFound();
        }
        _context.SaveChanges();
        return Ok();
    }
}