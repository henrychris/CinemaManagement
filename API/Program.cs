using API.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCore();
var app = builder.Build();
app.AddCore();
app.Run();

// todo: Failed to determine the https port for redirect.
// todo: add custom password validator to prevent common password goofs

// for integration tests
public partial class Program;