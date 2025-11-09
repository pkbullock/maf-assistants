using MAF.UI.Components;
using MAF.UI.Services;
using MAF.Assistants.Interfaces;
using MAF.Assistants.Factories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add custom services
builder.Services.AddSingleton<ChatStorageService>();
builder.Services.AddSingleton<MarkdownService>();
builder.Services.AddSingleton<AdaptiveCardService>();
builder.Services.AddSingleton<ChatService>();

// Add Agent Factory
builder.Services.AddSingleton<IChatAgentFactory, ChatAgentFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
