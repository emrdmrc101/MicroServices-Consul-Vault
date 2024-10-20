using Autofac;
using Autofac.Extensions.DependencyInjection;
using Core.Consul;
using Core.HostedService;
using Core.Identity;
using Core.Middlewares;
using Core.Modules;
using Core.Tracing;
using Lesson.Api.Modules;
using Lesson.Application.Modules;
using Lesson.Infrastructure.Data;
using Lesson.Infrastructure.Modules;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddMyConsul();
VaultService.SetVaultSecrets(builder.Configuration);
builder.Services.AddHostedService<AppHostedService>();

#region [Register Modules]

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>(
    b =>
    {
        b.RegisterModule<LessonApiModules>();
        b.RegisterModule<BaseModule>();
        b.RegisterModule<ApplicationModule>();
        b.RegisterModule<InfrastructureModules>();
        b.RegisterType<LessonDbContext>().AsSelf();
        b.RegisterModule(new CoreModule(builder.Configuration));
    });

#endregion

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMassTransit();
builder.Services.AddMassTransitHostedService();
builder.Services.AddOpenTelemetryAndJaeger(builder.Configuration);
builder.Services.AddJwt(builder.Configuration);
builder.Services.AddHttpClient();


#region [Entityframework]

builder.Services.AddDbContext<LessonDbContext>(o =>
{
    o.UseNpgsql(builder.Configuration.GetValue<string>("Database:connectionString"));
});

#endregion

var app = builder.Build();

app.AddExceptionHandlingMiddleware();
app.AddUserClaimsMiddleware();
app.AddTraceMiddleware();

var dbContext = app.Services.GetService<LessonDbContext>();
await dbContext.Database.MigrateAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
// app.UseSerilogRequestLogging();
app.MapControllers();
app.Run();