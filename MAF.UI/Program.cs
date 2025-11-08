using MAF.UI.Components;
using MAF.UI.Services;
using MAF.Assistants.Agents;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add custom services
builder.Services.AddSingleton<ChatStorageService>();
builder.Services.AddSingleton<MarkdownService>();
builder.Services.AddSingleton<AdaptiveCardService>();
builder.Services.AddSingleton<ChatService>();

// Add Agents
builder.Services.AddSingleton<SimpleChatAgent>();

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
