using Microsoft.EntityFrameworkCore;
using TicTacToe.Data;
using TicTacToe.DataSet;
using TicTacToe.Signal;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<TictactoeContext>(options=>options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TICTACTOE;Integrated Security = true"));
builder.Services.AddScoped<IGetData, GetDataController>();
builder.Services.AddSignalR();
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

app.MapHub<TicTacToeHub>("/TicTacToeHub");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
