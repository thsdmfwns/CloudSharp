using CloudSharp.Data.EntityFramework.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//DB
builder.Services.AddDbContext<DatabaseContext>(
    options =>
    {
        options.UseMySQL("server=localhost;port=11001;database=library;user=root;password=q1w2e3r4",
            b => b.MigrationsAssembly("CloudSharp.Migration"));
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.Run();