using Microsoft.AspNetCore.Identity;
using rehome.Data;
using rehome.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigurationManager configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();


builder.Services.AddAuthentication("rehomeAuthenticationScheme")
    .AddCookie("rehomeAuthenticationScheme", options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/login/Forbidden/";
        options.LoginPath = "/Login/Login";
    });

builder.Services.AddTransient<IQuoteService, QuoteService>();
builder.Services.AddTransient<IClientService, ClientService>();
builder.Services.AddTransient<IDropDownListService, DropDownListService>();
builder.Services.AddTransient<ITantouService, TantouService>();
builder.Services.AddTransient<IChumonService, ChumonService>();
builder.Services.AddTransient<ISiireService, SiireService>();
builder.Services.AddTransient<IHouzinService, HouzinService>();
builder.Services.AddTransient<IBunruiService, BunruiService>();
builder.Services.AddTransient<IOfficeService, OfficeService>();
builder.Services.AddTransient<INissiService, NissiService>();
builder.Services.AddTransient<ICalendarService, CalendarService>();
builder.Services.AddTransient<ISyouhinService, SyouhinService>();
builder.Services.AddTransient<INyukinService, NyukinService>();
builder.Services.AddTransient<IImportService, ImportService>();
builder.Services.AddTransient<IPayService, PayService>();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "rehome";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();

    //これで本番環境でもエラー内容が表示される
    //app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
//name: "default",
//pattern: "{controller=Samples}/{action=Index}");

name: "default",
//pattern: "{controller=Client}/{action=Index}");
pattern: "{controller=Nyukin}/{action=Index}");
//name: "default",
//pattern: "{controller=Quote}/{action=Clear}");
app.MapRazorPages();
app.MapDefaultControllerRoute();

app.Run();
