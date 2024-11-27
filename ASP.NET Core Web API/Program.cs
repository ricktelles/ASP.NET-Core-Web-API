var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configura��o de reposit�rios para inje��o de depend�ncia (Scoped)
builder.Services.AddScoped<ASP.NET_Core_Web_API.Repository.FornecedorRepository>();
builder.Services.AddScoped<ASP.NET_Core_Web_API.Repository.FuncionarioRepository>();
builder.Services.AddScoped<ASP.NET_Core_Web_API.Repository.ProdutoRepository>();
builder.Services.AddScoped<ASP.NET_Core_Web_API.Repository.VendasRepository>();

// Configura��o do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Ativa o uso do CORS
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
