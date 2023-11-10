using CorreoFei.Services.Email;
using CorreoFei.Services.ErrorLog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Mis servicios

//soporte para guardar errorres
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IErrorLog, ErrorLog>();
//Soporte para enviar correo
builder.Services.AddScoped<IEmail, Email>();

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
