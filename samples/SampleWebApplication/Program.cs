using Swashbuckle.AspNetCore.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.DocInclusionPredicate(
            (docName, apiDescription) =>
            {
                return apiDescription.GroupName == null || apiDescription.GroupName == docName;
            });
    });
builder.Services.AddHealthChecks().AddSwagger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHealthChecks("/healthz").WithApiDescription(
    metadata =>
    {
        metadata.Summary = "Returns information about the health of the system";
        metadata.DisplayName = "Health Check";
        //metadata.GroupName = "HealthChecks";
    });
app.MapControllers();

app.Run();
