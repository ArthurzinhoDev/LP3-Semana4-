using skymoon.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.WebHost.UseUrls("http://0.0.0.0:8000");

var app = builder.Build();

app.UseCors("AllowAll");

Funcionario[] listaFuncionarios = new Funcionario[100];
int totalFuncionarios = 0;

app.MapGet("/", () =>
{
    return Results.Ok("API SkyMoon funcionando com sucesso!");
});

app.MapPost("/funcionario", (JsonElement body) =>
{
    Random random = new();

    Funcionario funcionario = new Funcionario ();

    funcionario.Id = random.Next(1000,9999);
    funcionario.Nome = body.GetProperty("nome").GetString();
    funcionario.Idade = body.GetProperty("idade").GetInt32();
    funcionario.Cargo = body.GetProperty("cargo").GetString();
    funcionario.Departamento = body.GetProperty("departamento").GetString();
    funcionario.Salario = body.GetProperty("salario").GetDouble();

    listaFuncionarios[totalFuncionarios] = funcionario;
    totalFuncionarios++;

    return Results.Ok( new {
    funcionario
        }
    );
});

app.MapGet("/funcionario", () =>
{
    Funcionario[] funcionariosCadastrados = new Funcionario[totalFuncionarios];

    for (int i = 0; i < totalFuncionarios; i++)
    {
        funcionariosCadastrados[i] = listaFuncionarios[i];
    }

    return Results.Ok(new
    {
        funcionariosCadastrados
    });
});
app.MapPatch("/funcionario/{id}", (int id, JsonElement body) =>
{
    double novo_salario = body.GetProperty("salario").GetDouble();

    for (int i = 0; i < totalFuncionarios; i++)
    {
        if (listaFuncionarios[i].Id == id)
        {
            listaFuncionarios[i].Salario = novo_salario;
            return Results.Ok(
                new
                {
                    funcionario = listaFuncionarios[i]
                }
            );
        }
    }

    return Results.NotFound(new
    {
        message = "Funcionário não encontrado."
    });
});


app.MapPut("/funcionario/{id}", (int id, JsonElement body) =>
{   
    for (int i = 0; i < totalFuncionarios; i++)
    {
        if (listaFuncionarios[i].Id == id)
        {
            listaFuncionarios[i].Nome = body.GetProperty("nome").GetString();
            listaFuncionarios[i].Idade = body.GetProperty("idade").GetInt32();
            listaFuncionarios[i].Cargo = body.GetProperty("cargo").GetString();
            listaFuncionarios[i].Departamento = body.GetProperty("departamento").GetString();
            listaFuncionarios[i].Salario = body.GetProperty("salario").GetDouble();

            return Results.Ok(
                new
                {
                    funcionario = listaFuncionarios[i]
                }
            );
        }
    }

    return Results.NotFound(new
    {
        message = "Funcionário não encontrado."
    });
});

app.MapDelete("/funcionario/{id}", (int id) =>
{
    for (int i = 0; i < totalFuncionarios; i++)
    {
        if (listaFuncionarios[i].Id == id)
        {
            Funcionario funcionarioRemovido = listaFuncionarios[i];
            
            for (int j = i; j < totalFuncionarios - 1; j++)
            {
                listaFuncionarios[j] = listaFuncionarios[j + 1];
            }            

            totalFuncionarios--;

            return Results.Ok(new
            {
                mensagem = "Funcionário removido com sucesso.",
                funcionario = funcionarioRemovido
            });
        }
    }

    return Results.NotFound(new
    {
        message = "Funcionário não encontrado."
    });
});

app.MapGet("/funcionario/departamento/busca", (string departamento) =>
{
    Funcionario[] funcionariosEncontrados = new Funcionario[totalFuncionarios];

    int totalEncontrados = 0;

    for (int i = 0; i < totalFuncionarios; i++)
    {
        if (listaFuncionarios[i].Departamento?.ToLower() == departamento.ToLower())
        {
            funcionariosEncontrados[totalEncontrados] = listaFuncionarios[i];
            totalEncontrados++;
        }
    }

    if (totalEncontrados > 0)
    {
        Funcionario[] resultadoFinal = new Funcionario[totalEncontrados];

        for (int i = 0; i < totalEncontrados; i++)
        {
            resultadoFinal[i] = funcionariosEncontrados[i];
        }        

        return Results.Ok(new
        {
            departamento,
            funcionarios = resultadoFinal
        });
    } 

    return Results.NotFound(new
    {
        message = "Nenhum funcionário encontrado para esse departamento."
    });
});

app.MapGet("/funcionario/busca", (string nome) =>
{
    Funcionario[] funcionariosEncontrados = new Funcionario[totalFuncionarios];

    int totalEncontrados = 0;

    for (int i = 0; i < totalFuncionarios; i++)
    {
        if (listaFuncionarios[i].Nome?.ToLower() == nome.ToLower())
        {
            funcionariosEncontrados[totalEncontrados] = listaFuncionarios[i];
            totalEncontrados++;
        }
    }

    if (totalEncontrados > 0)
    {
        Funcionario[] resultadoFinal = new Funcionario[totalEncontrados];

        for (int i = 0; i < totalEncontrados; i++)
        {
            resultadoFinal[i] = funcionariosEncontrados[i];
        }        

        return Results.Ok(new
        {
            nome,
            funcionarios = resultadoFinal
        });
    } 

    return Results.NotFound(new
    {
        message = "Nenhum funcionário encontrado esse nome."
    });
});

app.Run();