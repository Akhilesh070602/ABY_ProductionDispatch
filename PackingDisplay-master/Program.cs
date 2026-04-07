//using PackingDisplay.Services;
//using PackingDisplay.SAP;
//using SAP.Middleware.Connector;

//namespace PackingDisplay
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            builder.Services.AddControllersWithViews();
//            builder.Services.AddScoped<DispatchService>();

//            // ✅ VERY IMPORTANT
//            RfcDestinationManager.RegisterDestinationConfiguration(new HanaConfig());

//            var app = builder.Build();

//            app.UseHttpsRedirection();
//            app.UseStaticFiles();

//            app.UseRouting();
//            app.UseAuthorization();

//            app.MapControllers();

//            app.MapControllerRoute(
//                name: "default",
//                pattern: "{controller=Dispatch}/{action=Index}/{id?}");

//            app.Run();
//        }
//    }
//}

using PackingDisplay.SAP;
using PackingDisplay.Services;
using SAP.Middleware.Connector;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();


builder.Services.AddSingleton<SapConnectionService>();
builder.Services.AddSingleton<HanaConfig>();


builder.Services.AddScoped<DispatchService>();
builder.Services.AddScoped<ScannedNoService>();
builder.Services.AddSingleton<LogService>();
builder.Services.AddTransient<WeighmentService>(); // 
//builder.Services.AddScoped<DashboardService>();

var app = builder.Build();



var hanaConfig = app.Services.GetRequiredService<HanaConfig>();
RfcDestinationManager.RegisterDestinationConfiguration(hanaConfig);



if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dispatch}/{action=Index}/{id?}");

app.Run();