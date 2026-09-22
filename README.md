This is a simple typical customers orders .Net (9.0) MVC application. In order to build and run the app, you need to install the following NuGets 
(since I'm running .Net 9.0, I installed the 9.x.xx of these packages, not the 10.x.xx):

1. <b>Microsoft.EntityFrameworkCore</b> (which will automatically install other Microsoft EF packages).
2. Since the application is using a local mdf SQL Server file, package <b>Microsoft.EntityFrameworkCore.SqlServer</b> needs to be installed.

For the Kendo Grid component in this solution, it is part of a complete suite of components in the Telerik product <b>"Telerik UI for Asp.Net Core"</b>, the solution only uses the Kendo Grid component.
   Telerik offers a trial version of the product here: https://www.telerik.com/aspnet-core-ui
   (Although the trial period expires in 30 days, the product remains functional, only it will continue to show a prompt to purchase it). Go ahead and download and install the product per its documentation on Telerik's site.
   After you've installed the product, you have to do the following two code changes for the product to work:
   1. Add the Kendo wrapper library and helper tags to the _ViewImports.cshtml in your project:
      <p><b>@using Kendo.Mvc.UI<br>
      @addTagHelper *, Kendo.Mvc</b></p>

   3. Add the Kendo to the services container in program.cs:
      <p><b>builder.Services.AddKendo();</b></p>

<b>IMPORTANT:</b> The Kendo Grid will not show any data if it is not in PascalName case (e.g. OrderId, ShipCity...etc.). .Net converts the JSON from the API to camelName (orderId, shipCity...et.). You have to set the .Net to NOT convert JSON to camelName or the grid won't show any data. This setting is made in the program.cs:

// Configure .Net to not convert JSON to camelName case becasue Kendo Grid requires PascalName case
<b>builder.Services.AddControllers().AddJsonOptions(options =>
{<br>
options.JsonSerializerOptions.PropertyNamingPolicy = null;<br>
});
</b>

   Telerik tech support offers technical support for their trial products (while in trial). Their support is awesome in my experience.
