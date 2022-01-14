
using ReverseProxy;
using ReverseProxy.Interfaces;
using ReverseProxy.Lib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IUriBuilder, UriBuilderService>();
builder.Services.AddTransient<IRemoteHost, RemoteHostService>();
builder.Services.AddTransient<IHostMapsRepository, FileRepositoryService>();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.EnableReverseProxy();

app.Run();

