using ResponsibilityHub.Patterns;
using Microsoft.Extensions.Azure;
using ResponsibilityHub.ServiceBus;
using ResponsibilityHub.DDD.AppointmentAggregate;
using ResponsibilityHub.DDD.Infrastructure;
using ResponsibilityHub.DDD.Base;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddSingleton(sp=>
    new StorageConfig("BlobEndpoint=https://storageworkshops.blob.core.windows.net/;QueueEndpoint=https://storageworkshops.queue.core.windows.net/;FileEndpoint=https://storageworkshops.file.core.windows.net/;TableEndpoint=https://storageworkshops.table.core.windows.net/;SharedAccessSignature=sv=2022-11-02&ss=b&srt=co&sp=rwdlaciytfx&se=2024-10-11T17:29:43Z&st=2024-07-27T09:29:43Z&spr=https&sig=bUETH2qRpLaC5c1oouU6%2Fbo%2FiifplrQCJTLlcjMiCH0%3D", 
    "sebdzi"));
builder.Services.AddAzureClients(builder =>
{
    builder.AddServiceBusClient("Endpoint=sb://sbworkshops.servicebus.windows.net/;SharedAccessKeyName=sbworshops-sas;SharedAccessKey=LeIyfXU81aJ9khizi7BLU7IrGSPUMIUI7+ASbMBiAbs=")
    .WithName("sbClient");
});
builder.Services.AddHostedService<ServiceBusManager>();
builder.Services.AddSingleton<ServiceBusConfiguration>(sp =>
    new ServiceBusConfiguration
    {
        Name = "sbClient",
        QueueName = "sebdzi-q"
    });



builder.Services.AddScoped<IRepository<AppointmentApp>, AppointmentRepository>();
builder.Services.AddScoped<AppointmentInfrastructure>();

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

app.MapPut("/{id}", () =>
{
    Results.Ok();
});

app.Run();
