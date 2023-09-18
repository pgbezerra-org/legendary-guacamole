using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("{id:int}")]
    public IActionResult ReadRealEstate(int id){

        var result = _context.RealEstates.Find(id);

        if(result == null){
            return NotFound();
        }else{
            return Ok(result);
        }
    }

    [HttpGet]
    public IActionResult ReadAllRealEstates(){

        var result = _context.RealEstates.ToList();

        if(result!=null){
            return Ok(result);
        }else{
            return NotFound();
        }
    }

    [HttpGet("{id}")]
    public IActionResult ReadRealEstatesByCompanyId(string id) {

        var result = _context.RealEstates.Where(r => r.CompanyId.Contains(id)).ToList();

        if(result == null || !result.Any()){
            return NotFound();
        }else{
            return Ok(result);
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