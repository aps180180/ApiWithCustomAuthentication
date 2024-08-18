using ApiWithAccessToken.Data;
using ApiWithAccessToken.Scheme;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region Database Services
builder.Services.AddDbContext<AuthDbContext>(opt => opt.UseSqlite("Data Source = App.db"));
#endregion

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Autenticação 
builder.Services.AddAuthentication("Custom-Scheme")
    .AddScheme<AuthenticationSchemeOptions,CustomAuthenticationHandler>("Custom-Scheme",null);
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
