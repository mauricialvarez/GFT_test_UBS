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

## Docker

O Dockerfile fica em:

```text
docker/Dockerfile
```

Construir a imagem:

```bash
docker build -f docker/Dockerfile -t gft-test-ubs:local .
```

Executar a imagem recebendo a entrada via `stdin`:

```bash
docker run --rm -i gft-test-ubs:local < Data/input.txt
```

Executar montando a pasta `Data` como volume somente leitura:

```bash
docker run --rm --entrypoint sh -v "$(pwd)/Data:/data:ro" gft-test-ubs:local -c "dotnet GFT_test_UBS.Interface.dll < /data/input.txt"
```

No PowerShell, use `${PWD}`:

```powershell
docker run --rm --entrypoint sh -v "${PWD}/Data:/data:ro" gft-test-ubs:local -c "dotnet GFT_test_UBS.Interface.dll < /data/input.txt"
```

Executar via Docker Compose:

```bash
docker compose -f docker/compose.yaml run --rm gft-test-ubs
```

O Compose monta:

```text
Data/ -> /data:ro
```

Por padrão, o serviço lê `/data/input.txt`. Para usar outro arquivo, altere o `command` e a variável `INPUT_FILE` em `docker/compose.yaml`.

### Logs com Seq

O `docker/compose.yaml` também sobe uma instância do Seq para observabilidade local. A aplicação envia logs estruturados para o Seq quando a variável `SEQ_URL` está configurada.

Os resultados da classificação continuam em `stdout`. Erros de validação continuam em `stderr`. O Seq recebe apenas logs operacionais, como início, fim, sucesso, warnings e falhas.

Subir o Seq:

```bash
docker compose -f docker/compose.yaml up -d seq
```

Acessar a UI:

```text
http://localhost:5341
```

Executar a aplicação enviando logs para o Seq e lendo `Data/input.txt` pelo volume `/data`:

```bash
docker compose -f docker/compose.yaml run --rm gft-test-ubs
```

Cada execução recebe um `TracingId`. Se a variável `TRACING_ID` não for informada, a aplicação gera um GUID automaticamente.

Para informar um `TracingId` manual, útil quando um orquestrador externo já possui um id da execução:

```bash
TRACING_ID=execucao-local-001 docker compose -f docker/compose.yaml run --rm gft-test-ubs
```

No PowerShell:

```powershell
$env:TRACING_ID = "execucao-local-001"
docker compose -f docker/compose.yaml run --rm gft-test-ubs
Remove-Item Env:\TRACING_ID
```

No Seq, filtre uma execução específica com:

```text
TracingId = 'execucao-local-001'
```

Campos enviados nos logs:

| Campo | Descrição |
|---|---|
| `TracingId` | Identificador da execução. Pode ser gerado automaticamente ou informado por variável de ambiente. |
| `InputFile` | Arquivo de entrada processado pela execução. No Compose padrão, `/data/input.txt`. |
| `Status` | Status final da execução, como `Success` ou `Failure`. |

Eventos registrados:

| Evento | Nível |
|---|---|
| Início da execução | Information |
| Arquivo processado com sucesso | Information |
| Falha ao processar arquivo | Warning |
| Erro exibido ao usuário | Error |
| Fim da execução | Information |

Parar o Seq:

```bash
docker compose -f docker/compose.yaml down
```

Remover também o volume local de dados do Seq:

```bash
docker compose -f docker/compose.yaml down -v
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
