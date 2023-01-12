IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<Worker>();
        services.AddHttpClient();
        services.RemoveAll<IHttpMessageHandlerBuilderFilter>();
    })
    .Build();

await host.Services.GetRequiredService<Worker>().ExecuteAsync(args);
