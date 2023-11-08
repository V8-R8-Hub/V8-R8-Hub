using Microsoft.OpenApi.Models;
using V8_R8_Hub.Middleware;
using V8_R8_Hub.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSwaggerGen(c => {
	c.SwaggerDoc("v1",
		new OpenApiInfo {
			Title = "V8-R8-Hub",
			Version = "v1"
		}
	);

	var filePath = Path.Combine(AppContext.BaseDirectory, "V8-R8-Hub.xml");
	c.IncludeXmlComments(filePath);
});

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

builder.Services.AddSession(options => {
	options.IdleTimeout = TimeSpan.FromHours(1);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = false;
});

builder.Services.AddTransient<IPublicFileService, PublicFileService>();
builder.Services.AddTransient<ISafeFileService, SafeFileService>();
builder.Services.AddTransient<IGameAssetService, GameAssetService>();
builder.Services.AddTransient<IGameService, GameService>();
builder.Services.AddTransient<IDbConnector, DbConnector>();
builder.Services.AddTransient<IMetricService, MetricService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

if (app.Environment.IsDevelopment()) {
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.UsePersistentAuth();

app.UseWhen(ctx => {
	return ctx.Request.Path.StartsWithSegments("/api/user", StringComparison.OrdinalIgnoreCase);
	}, builder => {
	builder.UseUserTracking();
});

app.MapRazorPages();
app.MapControllers();

app.Run();
