using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webserver.Models.DTOs;
public class ClientDTO {

    //Insert Client specific fields

    //Insert IdentityUser fields

    public ClientDTO(){

    }

    public static explicit operator Client(ClientDTO dto){

    }

    public static explicit operator ClientDTO(Client client){
        
    }    
}