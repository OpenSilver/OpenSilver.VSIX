## Welcome to OpenSilver Business Application

You are in the Modern Business Application. In this application, you will find a ".Server" project,  
which references the latest OpenRiaServices 5.x packages. There is an intermediate project ".Client",  
that will contain the generated client code for the OpenRia Services, and the main OpenSilver application  
references this ".Client" project.

The server application uses the latest .Net "Core" technologies, including Entity Framework Core and uses,  
for user management, the ASP.Net Identity system, differently of the legacy ASP.Net Membership system most  
used in Silverlight systems. For more information regarding ASP.Net Membership to ASP.Net Identity, please  
refer to this link: [Migrate from ASP.NET Membership authentication to ASP.NET Core 2.0 Identity](https://learn.microsoft.com/en-us/aspnet/core/migration/proper-to-2x/membership-to-core-identity?view=aspnetcore-8.0).  

TIP: Use "Project -> Configure Startup Projects..." option in Visual Studio, to change the "Action" for  
both "Multiple startup projects" options for ".Browser" and ".Server" projects from "None" to "Start".

You can find more information:
- [Creating a Business Application with RIA Services](https://doc.opensilver.net/documentation/general/business-app.html)  
- [OpenSilver Home](https://opensilver.net/)  
- [OpenSilver GitHub Repository](https://github.com/OpenSilver/OpenSilver)  