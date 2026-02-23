# Chat Vue.js - Streaming API Client

Um cliente simples em Vue.js para testar os endpoints de chat com streaming.

## Como usar

### 1. Iniciar a API

```bash
cd /Users/carlosmachel/Projects/videos/MAF-Series/src/Streaming
dotnet run
```

A API estará disponível em `http://localhost:5290`

### 2. Abrir o cliente

Abra o arquivo `index.html` no navegador:

```bash
open /Users/carlosmachel/Projects/videos/MAF-Series/src/Streaming/client/index.html
```

Ou simplesmente abra o arquivo `index.html` diretamente no navegador.

## Funcionalidades

- **Modo Streaming (SSE)**: Recebe as respostas da IA em tempo real usando Server-Sent Events
- **Modo Normal**: Aguarda a resposta completa antes de exibir

## Endpoints consumidos

- `POST /run?userInput={mensagem}` - Requisição normal
- `POST /run-streaming?userInput={mensagem}` - Streaming via SSE

## Tecnologias

- Vue.js 3 (via CDN, sem build necessário)
- CSS puro (sem dependências)
- Fetch API para requisições
- ReadableStream para processar SSE

