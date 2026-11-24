using FluentValidation;
using Microsoft.Extensions.Hosting;
using RapidApi.Web.Services.Abstract;
using RapidApi.Web.Services.Concrete;
using RapidApi.Web.Validators;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddValidatorsFromAssemblyContaining<BookingSearchViewModelValidator>();
builder.Services.AddHttpClient<ILocationService, LocationService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);

    client.DefaultRequestHeaders.Add(
        "x-rapidapi-key",
        builder.Configuration["RapidApi:ApiKey"]);
});
builder.Services.AddHttpClient<IBookingService, BookingService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);

    var apiKey = builder.Configuration["RapidApi:ApiKey"];

    client.DefaultRequestHeaders.Add("x-rapidapi-key", apiKey);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
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
