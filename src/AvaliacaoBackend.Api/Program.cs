using AvaliacaoBackend.Api.Services.Clientes;
using AvaliacaoBackend.Api.Services.DocuSign;
using AvaliacaoBackend.Api.Services.Force1;
using AvaliacaoBackend.Api.Services.GoogleMaps;
using AvaliacaoBackend.Api.Services.MicrosoftGraph;
using AvaliacaoBackend.Api.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection(MongoSettings.SectionName));
builder.Services.Configure<Force1Settings>(builder.Configuration.GetSection(Force1Settings.SectionName));
builder.Services.Configure<GoogleMapsSettings>(builder.Configuration.GetSection(GoogleMapsSettings.SectionName));
builder.Services.Configure<DocuSignSettings>(builder.Configuration.GetSection(DocuSignSettings.SectionName));
builder.Services.Configure<MicrosoftGraphSettings>(builder.Configuration.GetSection(MicrosoftGraphSettings.SectionName));

builder.Services.AddSingleton<IClienteService, ClienteMongoService>();
builder.Services.AddSingleton<IProdutoRepository, ProdutoEmMemoriaRepository>();

builder.Services.AddHttpClient<IForce1Service, Force1Service>();
builder.Services.AddHttpClient<IGoogleMapsService, GoogleMapsService>();
builder.Services.AddHttpClient<IDocuSignService, DocuSignService>();
builder.Services.AddHttpClient<IMicrosoftGraphService, MicrosoftGraphService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
