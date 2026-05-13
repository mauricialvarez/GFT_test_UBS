# GFT_test_UBS

Projeto CLI em .NET desenvolvido como teste para a GFT, cliente UBS.

## Objetivo

Classificar operações de um portfólio bancário conforme regras de risco definidas no enunciado.

O enunciado original está disponível em:

```text
docs/TesteRiscoBR 1.pdf
```

## Estrutura

```text
src/
  GFT_test_UBS.Domain
  GFT_test_UBS.Application
  GFT_test_UBS.Infrastructure
  GFT_test_UBS.Interface

test/
  GFT_test_UBS.Domain.Tests
  GFT_test_UBS.Application.Tests
  GFT_test_UBS.Infrastructure.Tests
  GFT_test_UBS.Interface.Tests
```

## Comandos

Restaurar dependências:

```bash
dotnet restore GFT_test_UBS.sln
```

Compilar:

```bash
dotnet build GFT_test_UBS.sln
```

Executar testes:

```bash
dotnet test GFT_test_UBS.sln
```
