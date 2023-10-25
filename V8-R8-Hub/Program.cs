using V8_R8_Hub.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddDistributedMemoryCache();

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

builder.Services.AddSession(options => {
	options.IdleTimeout = TimeSpan.FromHours(1);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

builder.Services.AddTransient<IPublicFileService, PublicFileService>();
builder.Services.AddTransient<ISafeFileService, SafeFileService>();
builder.Services.AddTransient<IGameAssetService, GameAssetService>();
builder.Services.AddTransient<IGameService, GameService>();
builder.Services.AddTransient<IDbConnector, DbConnector>();

var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapRazorPages();
app.MapControllers();

app.Run();
