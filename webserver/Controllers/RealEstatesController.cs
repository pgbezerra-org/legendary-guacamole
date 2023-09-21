using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webserver.Data;
using webserver.Models;

namespace webserver.Controllers;

[Route("api/v1/realestates")]
[ApiController]
public class RealEstatesController : ControllerBase {
    
    private readonly WebserverContext _context;

    public RealEstatesController(WebserverContext context) {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> ReadRealEstate(int? minPrice, int? maxPrice, int? offset, int? limit, string sort = "Id") {

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

        var result = await realEstates.ToListAsync();

        if (result == null) {
            return NotFound();
        }else {

            var response = new {
                data = result.Select(re => new {
                    
                    type = "realestates",
                    id = re.Id,
                    attributes = new RealEstate
                    {
                        Name = re.Name,
                        Price = re.Price,
                        Address = re.Address,
                        CompanyId = re.CompanyId,
                    }
                })
            };

            return Ok(response);
        }
    }

    [HttpPost]
    public IActionResult CreateRealEstate(RealEstate request ) {

        if(_context.RealEstates.Find(request.Id)!=null){
            return BadRequest("Real Estate Already Exists!");
        }
        
        if (_context.Company.Find(request.CompanyId) == null) {
            return BadRequest("Owner Company does Not Exist!");
        }
        
        _context.RealEstates.Add(request);
        _context.SaveChanges();

        var response = new {
            data = new {
                type = "realestates",
                id = request.Id,
                attributes = new {
                    name = request.Name,
                    price = request.Price,
                    address = request.Address,
                    companyId = request.CompanyId
                }
            }
        };

        return CreatedAtAction(nameof(CreateRealEstate), new { id = request.Id }, response);
    }

    [HttpPatch("{id}")]
    public IActionResult UpdateRealEstate(int id, RealEstate upRE) {

        var realEstate=_context.RealEstates.Find(id);
        if(realEstate==null){
            return NotFound();
        }
        
        realEstate.Address=upRE.Address;
        realEstate.Price=upRE.Price;
        realEstate.Name=upRE.Name;

        _context.SaveChanges();
        
        var response = new {
            data = new {
                type = "realestates",
                id = realEstate.Id,
                attributes = new {
                    name = realEstate.Name,
                    price = realEstate.Price,
                    companyId = realEstate.CompanyId,
                    // Add other attributes as needed
                }
            }
        };

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteRealEstate(int id){

        var RE=_context.RealEstates.Find(id);
        if(RE==null){
            return NotFound();
        }

        _context.RealEstates.Remove(RE);
        _context.SaveChanges();
        
        return NoContent();
    }
}