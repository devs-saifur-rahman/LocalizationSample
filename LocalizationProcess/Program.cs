using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddLocalization();

CultureInfo[] supportedCulures = new CultureInfo[]
{
  new CultureInfo("en-US"),
  new CultureInfo("es-ES"),
};


RequestLocalizationOptions reqLocalizatonOptions = new RequestLocalizationOptions();


reqLocalizatonOptions.SupportedCultures = supportedCulures;
reqLocalizatonOptions.SupportedUICultures = supportedCulures;
reqLocalizatonOptions.SetDefaultCulture("en-US");
reqLocalizatonOptions.ApplyCurrentCultureToResponseHeaders = true;



var app = builder.Build();
app.UseRequestLocalization(reqLocalizatonOptions);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
