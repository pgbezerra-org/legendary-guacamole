namespace webserver.Models.DTOs;

//DTOs are only being used for Http Requests. So i elected to keep the minimum to distinguish them apart
//Http Responses are still the full RealEstate property, minus the 'OwnerCompany' class, since we have it's Id.

public class RealEstateDTO {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public string CompanyId { get; set; } = string.Empty;

    public RealEstateDTO(int id, string name, string address, decimal price, string companyId){

        Id = id;
        Name = name;
        Address = address;
        Price = price;
        CompanyId = companyId;
    }

    public static explicit operator RealEstate(RealEstateDTO dto) {
        return new RealEstate {
            Id = dto.Id,
            Name = dto.Name,
            Address = dto.Address,
            Price = dto.Price,
            CompanyId = dto.CompanyId
        };
    }

    public static explicit operator RealEstateDTO(RealEstate re){
        return new RealEstateDTO (re.Id, re.Name, re.Address, re.Price, re.CompanyId);
    }
}