namespace webserver.Models.DTOs;

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

    public static explicit operator RealEstate(RealEstateDTO DTO) {
        return new RealEstate {
            Id = DTO.Id,
            Name = DTO.Name,
            Address = DTO.Address,
            Price = DTO.Price,
            CompanyId = DTO.CompanyId
        };
    }

    public static explicit operator RealEstateDTO(RealEstate RE){
        return new RealEstateDTO (RE.Id, RE.Name, RE.Address, RE.Price, RE.CompanyId);
    }
}