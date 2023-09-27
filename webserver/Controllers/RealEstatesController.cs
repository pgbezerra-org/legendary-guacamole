using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webserver.Data;
using webserver.Models;

namespace webserver.Controllers;

[ApiController]
[Route("api/v1/realestates")]
public class RealEstatesController : ControllerBase {

    public class RealEstateSummary {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CompanyId { get; set; } = string.Empty;


        public static explicit operator RealEstate(RealEstateSummary summary) {
            return new RealEstate {
                Id = summary.Id,
                Name = summary.Name,
                Address = summary.Address,
                Price = summary.Price,
                CompanyId = summary.CompanyId
            };
        }
    }
    
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

        var response = new {
            data = new {
                type = "RealEstate",
                attribute = new RealEstateSummary {
                    Id = id,
                    Name = realEstate.Name,
                    Price = realEstate.Price,
                    Address = realEstate.Address,
                    CompanyId = realEstate.CompanyId
                }
            }
        };

        return Ok(response);
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

        var result = await realEstates.ToListAsync();

        if(result==null){
            return NotFound();
        }

        var response = new {
            data = result.Select(re => new {
                
                type = "RealEstate",
                attributes = new RealEstateSummary {
                    Id = re.Id,
                    Name = re.Name,
                    Price = re.Price,
                    Address = re.Address,
                    CompanyId = re.CompanyId,
                }
            })
        };

        return Ok(response);
    }

    [HttpPost]
    public IActionResult CreateRealEstate([FromBody]RealEstate request ) {

        if(_context.RealEstates.Find(request.Id)!=null) {
            return BadRequest("Real Estate Already Exists!");
        }
        
        if (_context.Company.Find(request.CompanyId) == null) {
            return BadRequest("Owner Company does Not Exist!");
        }

        request.OwnerCompany=_context.Company.Find(request.CompanyId);
        
        _context.RealEstates.AddAsync(request);
        _context.SaveChangesAsync();

        Console.WriteLine(_context.RealEstates.Find(request.Id).Name);

        var response = new {
            data = new {
                type = "RealEstate",
                attribute = new RealEstateSummary {
                    Id = request.Id,
                    Name = request.Name,
                    Price = request.Price,
                    Address = request.Address,
                    CompanyId = request.CompanyId
                }
            }
        };

        return Ok(response);
    }

    [HttpPatch("id")]
    public IActionResult UpdateRealEstate(int id, [FromBody]RealEstate upRE) {

        var realEstate = _context.RealEstates.Find(id);
        if(realEstate == null){
            return NotFound();
        }
        
        realEstate.Address = upRE.Address;
        realEstate.Price = upRE.Price;
        realEstate.Name = upRE.Name;

        _context.SaveChanges();
        
        var response = new {
            data = new {
                type = "RealEstate",
                attribute = new RealEstateSummary {
                    Id = realEstate.Id,
                    Name = realEstate.Name,
                    Price = realEstate.Price,
                    Address = realEstate.Address,
                    CompanyId = realEstate.CompanyId
                }
            }
        };

        return Ok(response);    
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