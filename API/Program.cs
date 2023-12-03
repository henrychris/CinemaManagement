using API.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCore();
var app = builder.Build();
app.AddCore();
app.Run();