# ProductAPIManagementWebAPI
CRUD operation for Products using .Net 8.0 Web API 

This Web API project is presented with 7 endpoints namely below:
- GET /api/products - Get all products
- GET /api/products/{id} - Get product by ID
- POST /api/products - Create new product
- PUT /api/products/{id} - Update product
- DELETE /api/products/{id} - Delete product 
- PUT /api/products/decrement-stock/{id}/{quantity} - Decrement stock
- PUT /api/products/add-to-stock/{id}/{quantity} - Add to stock

# Instructions to run this project locally
1. Clone this repo locally and get this project.
2. Have a local SQL server running in your system.
3. In appsettings.json, please specify your server name
4. Being its Code First approach using Entity framework, so we have entity class "Product.cs" that will create a corresponding table "Products" in the DB with name "ProductManagementDB" in your local SQL server on running below commands via Package Manager Console:
   add-migration "new migration"
   update-database
5. Now this Web API project is ready to run, launch it and get the 7 end points in Swagger UI.