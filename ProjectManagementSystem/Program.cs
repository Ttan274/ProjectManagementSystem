using ProjectManagementSystem.Application.Abstractions.SubTaskProducer;
using ProjectManagementSystem.Application.SubTaskProducer;
using ProjectManagementSystem.Persistance;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

builder.Services.AddRequiredServices(builder.Configuration);

builder.Services.AddHttpClient<ISubTaskProducerService, SubTaskProducerService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["OllamaOptions:BaseAddress"]);
});

var app = builder.Build();

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
    pattern: "{controller=User}/{action=Login}/{id?}");

app.Run();
