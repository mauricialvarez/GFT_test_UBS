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

benchmarks/
  GFT_test_UBS.Benchmarks
```

## Como executar

Restaurar dependências:

```bash
dotnet restore GFT_test_UBS.sln
```

Build em Debug:

```bash
dotnet build GFT_test_UBS.sln
```

Build em Release:

```bash
dotnet build GFT_test_UBS.sln -c Release
```

Executar os testes:

```bash
dotnet test GFT_test_UBS.sln
```

Executar a aplicação informando os dados manualmente:

```bash
dotnet run --project src/GFT_test_UBS.Interface/GFT_test_UBS.Interface.csproj
```

Executar a aplicação com o arquivo de exemplo no CMD:

```cmd
dotnet run --project src\GFT_test_UBS.Interface\GFT_test_UBS.Interface.csproj < input.txt
```

Executar a aplicação com o arquivo de exemplo no PowerShell:

```powershell
Get-Content .\input.txt | dotnet run --project .\src\GFT_test_UBS.Interface\GFT_test_UBS.Interface.csproj
```

## Benchmarks

Os benchmarks ficam em:

```text
benchmarks/GFT_test_UBS.Benchmarks
```

Os arquivos de massa ficam em `benchmarks/data`, mas essa pasta é ignorada pelo Git porque pode passar de centenas de MB.

Gerar massas de 1k, 10k, 100k, 1M e 10M operações:

PowerShell:

```powershell
.\benchmarks\generate-inputs.ps1
```

Bash:

```bash
bash benchmarks/generate-inputs.sh
```

Executar benchmarks:

```bash
dotnet run -c Release --project benchmarks/GFT_test_UBS.Benchmarks/GFT_test_UBS.Benchmarks.csproj
```

No Windows/CMD:

```cmd
dotnet run -c Release --project benchmarks\GFT_test_UBS.Benchmarks\GFT_test_UBS.Benchmarks.csproj
```

### Resultado base

Execução local com BenchmarkDotNet `ShortRun`, .NET 8.0.25, Windows 11, Intel Core i7-13700HX.

| Operações | Tempo médio | Memória alocada |
|---:|---:|---:|
| 1k | 458.8 us | 603.62 KB |
| 10k | 5.325 ms | 6.018 MB |
| 100k | 161.991 ms | 60.161 MB |
| 1M | 1.371 s | 601.575 MB |
| 10M | 7.165 s | 5.875 GB |

Observação: os benchmarks medem o core da aplicação (`ClassifyPortfolioUseCase`) com os dados já carregados em memória no setup do BenchmarkDotNet.
