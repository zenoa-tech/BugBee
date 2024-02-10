var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.BugBee_ApiService>("apiservice");

builder.AddProject<Projects.BugBee_Web>("webfrontend")
    .WithReference(apiService);

builder.Build().Run();
