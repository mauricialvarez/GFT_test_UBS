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

Executar a aplicação informando os dados manualmente. A entrada segue o formato do enunciado: data de referência, quantidade de operações e uma operação por linha.

```bash
dotnet run --project src/GFT_test_UBS.Interface/GFT_test_UBS.Interface.csproj
```

A CLI processa a entrada por streaming, lendo uma operação por vez e escrevendo a categoria diretamente na saída.

Executar a aplicação com o arquivo de exemplo no CMD:

```cmd
dotnet run -c Release --project src\GFT_test_UBS.Interface\GFT_test_UBS.Interface.csproj < Data\input.txt
```

Executar a aplicação com o arquivo de exemplo no PowerShell:

```powershell
Get-Content .\Data\input.txt | dotnet run -c Release --project .\src\GFT_test_UBS.Interface\GFT_test_UBS.Interface.csproj
```

Executar o binário já compilado em Release:

```cmd
dotnet src\GFT_test_UBS.Interface\bin\Release\net8.0\GFT_test_UBS.Interface.dll < Data\input.txt
```

Executar a massa de 10M operações sem gravar a saída em disco:

```cmd
dotnet src\GFT_test_UBS.Interface\bin\Release\net8.0\GFT_test_UBS.Interface.dll < Data\input_10000000.txt > NUL
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

O benchmark compara dois fluxos:

- `Buffered`: lê todas as linhas com `File.ReadAllLines`, classifica tudo em memória e escreve as categorias em `TextWriter.Null`.
- `Streaming`: lê o arquivo com `StreamReader`, classifica uma operação por vez e escreve em `TextWriter.Null`.

### Resultado comparativo

Execução local com BenchmarkDotNet `ShortRun`, .NET 8.0.25, Windows 11, Intel Core i7-13700HX.

| Operações | Fluxo | Tempo médio | Ratio | Memória alocada | Alloc Ratio |
|---:|---|---:|---:|---:|---:|
| 1k | Buffered | 1.511 ms | 1.00 | 739.13 KB | 1.00 |
| 1k | Streaming | 2.008 ms | 1.33 | 622.67 KB | 0.84 |
| 10k | Buffered | 10.591 ms | 1.00 | 7445.68 KB | 1.00 |
| 10k | Streaming | 8.731 ms | 0.82 | 6139.99 KB | 0.82 |
| 100k | Buffered | 207.962 ms | 1.00 | 73593.38 KB | 1.00 |
| 100k | Streaming | 67.580 ms | 0.33 | 61312.83 KB | 0.83 |
| 1M | Buffered | 2.215 s | 1.00 | 729677.50 KB | 1.00 |
| 1M | Streaming | 962.796 ms | 0.44 | 613038.98 KB | 0.84 |
| 10M | Buffered | 20.833 s | 1.00 | 7444115.23 KB | 1.00 |
| 10M | Streaming | 9.555 s | 0.46 | 6130290.13 KB | 0.82 |

Observação: a saída do benchmark é descartada com `TextWriter.Null` para medir processamento e escrita sem custo de persistir milhões de linhas em disco.
