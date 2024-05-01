using CloudSharp.Api.Service;
using CloudSharp.Data.EntityFramework;
using CloudSharp.Data.Repository;
using CloudSharp.Data.Store;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//DB
builder.Services.AddDbContext<DatabaseContext>(
    options =>
    {
        options.UseMySQL("server=localhost;port=11001;database=cloud_sharp;user=root;password=q1w2e3r4",
            b => b.MigrationsAssembly("CloudSharp.Migration"));
    }
);

//repository
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IShareRepository, ShareRepository>();

//store
builder.Services.AddSingleton<IFileStore, FileStore>();
builder.Services.AddSingleton<ITicketStore, HashMapTicketStore>(); //HashMap

//service
builder.Services.AddScoped<IFileStreamService, FileStreamService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IMemberFileService, MemberFileService>();
builder.Services.AddScoped<IMemberTicketService, MemberTicketService>();
builder.Services.AddScoped<IShareService, ShareService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.Run();