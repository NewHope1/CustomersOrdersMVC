This is a simple typical customers orders .Net (9.0) MVC application. In order to build and run the app, you need to install the following NuGets 
(since I'm running .Net 9.0, I installed the 9.x.xx of these packages, not the 10.x.xx):

1. <b>Microsoft.EntityFrameworkCore</b> (which will automatically install other Microsoft EF packages).
2. Since the application is using a local mdf SQL Server file, package <b>Microsoft.EntityFrameworkCore.SqlServer</b> needs to be installed.

For the Kendo Grid component in the solution here, it is part of a complete suite of the Telerik product <b>"Telerik UI for Asp.Net Core"</b> which is a suite of components (the solution here only uses the Kendo Grid component).
   Telerik offers a trial version of the product here: https://www.telerik.com/aspnet-core-ui
   (Although the trial period expires in 30 days, the product remains functional only it will continue to show a prompt to purchase it.)

<b>IMPORTANT:</b> The Kendo Grid will not show any data if it is not in PascalCase (e.g. OrderId, ShipCity...etc.). .Net converts the JSON from the API to camelCase (orderId, shipCity...et.). Thus you have to set the .Net to NOT convert JSON to camelCase or the grid won't show any data.

   Telerik tech support offers technical support for their trial products (while in trial). Their support is awesome in my experience.
