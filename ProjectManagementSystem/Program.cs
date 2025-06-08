using ProjectManagementSystem.Application.Abstractions.SubTaskProducer;
using ProjectManagementSystem.Application.SubTaskProducer;
using ProjectManagementSystem.Common.ServiceResponse;
using ProjectManagementSystem.Models;
using ProjectManagementSystem.Persistance;
using ProjectManagementSystem.Services.Mail;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
                .AddViewOptions(options =>
                {
                    options.HtmlHelperOptions.ClientValidationEnabled = true;
                })
                .AddRazorRuntimeCompilation();

builder.Services.AddRequiredServices(builder.Configuration);

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/User/Login";
    options.AccessDeniedPath = "/Home/AccessDenied";
});

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IServiceResponseHelper, ServiceResponseHelper>();

builder.Services.Configure<List<NavBarItem>>(builder.Configuration.GetSection("NavBarOptions"));

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dept}/{action=Index}/{id?}");

app.Run();
