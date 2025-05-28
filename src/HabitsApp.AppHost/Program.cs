var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.HabitsApp_WebAPI>("habitsapp-webapi");

builder.Build().Run();
