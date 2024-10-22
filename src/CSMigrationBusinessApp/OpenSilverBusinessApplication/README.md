## Welcome to OpenSilver Business Application

You are in the Migration focused Business Application. In this application, you will find an ".Web"  
project, which references the latest OpenRiaServices 4.6 packages for max compatibility with existing  
Silverlight or .Net Framework 4.x server applications because of the same API as WCF RIA Services.

The server application uses the latest compatible Entity Framework (6.4.4) version for .Net Framework 4.x,  
and uses, for user management, the ASP.Net Membership system, which was the standard in that time.  
For information about moving from ASP.Net Membership to ASP.Net Identity, please refer to this link:  
[Migrate from ASP.NET Membership authentication to ASP.NET Core 2.0 Identity](https://learn.microsoft.com/en-us/aspnet/core/migration/proper-to-2x/membership-to-core-identity?view=aspnetcore-8.0).

TIP: Use "Project -> Configure Startup Projects..." option in Visual Studio, to change the "Action" for  
both "Multiple startup projects" option for ".Browser" and ".Web" projects from "None" to "Start".  

You can find more information:
- [Creating a Business Application with RIA Services](https://doc.opensilver.net/documentation/general/business-app.html)  
- [OpenSilver Home](https://opensilver.net/)  
- [OpenSilver GitHub Repository](https://github.com/OpenSilver/OpenSilver)  