using Microsoft.AspNetCore.Builder;
using Polaris.WMS;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("Polaris.WMS.Web.csproj"); 
await builder.RunAbpModuleAsync<WMSWebTestModule>(applicationName: "Polaris.WMS.Web");

public partial class Program
{
}

