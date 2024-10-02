using Azure.Identity;
using Microsoft.Azure.Cosmos;
using azure_exam.Hubs;
using azure_exam.Services;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

var keyVaultEndpoint = new Uri(builder.Configuration["VaultUri"]);
builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());

var cosmosConnectionString = builder.Configuration["CosmosDb"];
builder.Services.AddSingleton(s => new CosmosClient(cosmosConnectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSignalR();

builder.Services.AddSingleton<BlobStorageService>();
builder.Services.AddSingleton<CosmosDbService>();


builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["BlobStorage"]!, preferMsi: true);
    clientBuilder.AddTableServiceClient(builder.Configuration["BlobStorage"]!);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
       name: "default",
       pattern: "{controller=Chat}/{action=Index}/{id?}");

//app.MapHub<ChatHub>("/chathub");

app.Run();
