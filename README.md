# Blockchain didatica em C# (.NET 8)

Projeto educacional para aprender arquitetura, criptografia basica, rede P2P, persistencia e engenharia em C#.

## Status atual
- Base da solucao criada com `Blockchain.sln` e projetos em `src/` e `tests/`.
- Dominio core inicial (modelos, hashing e validacoes).
- API inicial com Swagger/Serilog e endpoint `/health`.


## Quickstart (copy/paste)

### Restaurar, buildar e testar
```bash
dotnet restore Blockchain.sln
dotnet build Blockchain.sln --no-restore
dotnet test Blockchain.sln --no-build
```

### Rodar API (apenas health/raiz por enquanto)
```bash
dotnet run --project src/Blockchain.Api/Blockchain.Api.csproj
```

## Use cases 
- Arquitetura limpa por camadas: Core, Infrastructure, Node, Api e Cli.
- Criptografia aplicada: hashing e assinatura digital.
- Persistencia com EF Core e SQLite.
- Rede P2P via gRPC (planejado).
- Mineração PoW simples (planejado).
- Observabilidade basica com logs estruturados.

## Exemplos (futuros, quando a V1 estiver pronta)
Estes comandos serao validos depois da implementacao dos endpoints reais.

### Enviar transacao
```bash
curl -X POST http://localhost:8080/tx \
  -H "Content-Type: application/json" \
  -d "{\"from\":\"addr1\",\"to\":\"addr2\",\"amount\":10,\"nonce\":0,\"publicKeyHex\":\"...\",\"signatureHex\":\"...\"}"
```

### Minerar bloco
```bash
curl -X POST http://localhost:8080/mine \
  -H "Content-Type: application/json" \
  -d "{\"minerAddress\":\"addr1\"}"
```

### Ver cadeia
```bash
curl http://localhost:8080/chain
```

### Ver mempool
```bash
curl http://localhost:8080/mempool
```

### Consultar saldo
```bash
curl http://localhost:8080/balance/addr1
```

## Gotchas 
- O ambiente precisa de acesso ao NuGet para restaurar pacotes.
- Apesar do SDK 9 estar instalado, o alvo do projeto e .NET 8.
- Sem endpoints completos ainda: somente `/` e `/health` estao ativos.
- Quando comecar EF Core, fixe versoes 8.x para compatibilidade com net8.0.
- Nao commite `bin/`, `obj/`, `.vs/`, `.idea/` (ja cobertos por `.gitignore`).


