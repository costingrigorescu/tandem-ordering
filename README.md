Tandem.Model
- base model that contains the Id
- base product model that contains product type (physical/digital)
- order model that contains Id, ClientId and a list of products
- various physical and digital product models (book, newspaper, membership, membership change)

Tandem.Model.UnitTests
- unit tests on the models to ensure that the product types are defined correctly

Tandem.Service
- order processing service library
- implements a chain of responsibility for processing of orders
- order processing service class: OrderService.cs
- order processing chain links classes: OrderHandlers folder
- each link from the chain can be enabled or disabled through configuration (injected in OrderService.cs)

Tandem.Service.UnitTests
- unit tests on the order processing service and order processing links (just on one of them as an example that can be applied across the rest)
 
