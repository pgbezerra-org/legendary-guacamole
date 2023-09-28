# legendary-guacamole

This is a multi-tenant royal state service, with a random name for now.

To run this application on your machine, you need .NET 7.0, and a MySQL Database at port 3306

In Linux, you can setup the database with Docker and this command:
```shell
sudo docker run -d -p 3306:3306 --name mysql-container -e MYSQL_ROOT_PASSWORD={password} -e MYSQL_DATABASE=Guacamole mysql
```
From the root folder, run:

```shell
dotnet restore
dotnet watch run --project .\webserver\webserver.csproj
```
This command builds and runs the project, as well as hot-reloads.

The project will load with the Url **localhost:5067**. You can use Swagger in Development mode with **/swagger******

# API

## Companies



## Real Estates

RealEstatesDTO Request Body example:
```shell
{
  "id": 0,
  "name": "string",
  "address": "string",
  "price": 0,
  "companyId": "string"
}
```
It is used for the POST and Patch methods, however they don't require Id and CompanyId respectively


*  **GET /api/v1/unique/{id}**
    
{id} (integer): Real Estate unique identifier

Possible Responses:
200 OK; (If R.E with given Id is found)
404 Not Found; (If no R.E is found)


*  **GET /api/v1/ReadRealEstates**

    -  minPrice (integer, optional): Minimum price filter for Real Estates

    -  maxPrice (integer, optional): Maximum price filter for Real Estates

    -  offset (integer, optional): Offset for paginating results

    -  limit (integer, optional): Maximum number of results to return per page

    -  sort (string, optional, default: "Id"): Sort order for the results. Options: name, price, companyid, address

Retrieves a list of Real Estates fitting optional parameters. Paginable and Sortable

Possible Responses:
200 OK; (If at least ONE R.E fits the search)
404 Not Found; (If no R.E fits the search)

Possible Responses: 200 OK, 404 Not Found


*  **POST /api/v1/CreateRealEstate**

Possible Responses:
200 OK; (If R.E is created)
404 Not Found; (If OwnerCompanyId is not present in the database)

Possible Responses: 201 Created, 400 Bad Request


* **PATCH /api/v1/realestates/id**

{id} (integer): Real Estate unique identifier

Updates the Name, Address and Price of the Real Estate with the given Id. Returns 400 if no R.E has such Id

Possible Responses:
200 OK; (If R.E with given Id is found)
404 Not Found; (If no R.E is found)


* **DELETE /api/v1/realestates/id**

{id} (integer): Real Estate unique identifier

Possible Responses:
204 No Content; (If R.E with given Id is found)
404 Not Found; (If no R.E is found)
