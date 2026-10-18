# MAF Playlist — Episódios 1 a 7

> Série **Microsoft Agent Framework** no [AzureBrasil.Cloud](https://azurebrasil.cloud). Episódios 8 em diante: [maf-azurebrasil-hub](https://carlosmachel.github.io/maf-azurebrasil-hub/)

Repositório com exemplos práticos do **Microsoft Agent Framework (MAF)** desenvolvidos para a série de vídeos no YouTube.

## 📋 Sobre o Projeto

Este repositório contém exemplos progressivos de implementação do Microsoft Agent Framework, demonstrando desde conceitos básicos até funcionalidades avançadas como persistência, middlewares e Azure Functions.

## 🛠️ Tecnologias

- **.NET 10.0**
- **Microsoft.Agents.AI** (preview)
- **Azure OpenAi**

## 🎬 Episódios

| Ep. | Tema | Pasta |
|-----|------|-------|
| 01 | Primeiro agente com `ChatClientAgent` | [`src/MAF.01.Console`](src/MAF.01.Console) |
| 02 | Sessões: salvar e retomar a conversa | [`src/MAF.02.Console`](src/MAF.02.Console) · [`src/MAF.02.AzureAiAgentPersistent.Console`](src/MAF.02.AzureAiAgentPersistent.Console) |
| 03 | Agente numa API com tool e aprovação humana | [`src/MAF.03.Functions.Api`](src/MAF.03.Functions.Api) |
| 04 | Middlewares (chat client, agente, function call) | [`src/UsingMiddlewares`](src/UsingMiddlewares) |
| 05 | Structured Output | [`src/StructuredOutput`](src/StructuredOutput) |
| 06 | Memória e histórico com vetores | [`Memory`](Memory) |
| 07 | Streaming de respostas | [`src/Streaming`](src/Streaming) |

Vídeos: [playlist no YouTube](https://www.youtube.com/playlist?list=PLagWy4RVE5D-zG5tHWz5f_Vt8Mp6p1F--)

## 📂 Estrutura do Projeto

```
src/
├── MAF.01.Console/                         # Agente básico
├── MAF.02.Console/                         # Agente com recursos intermediários
├── MAF.02.AzureAiAgentPersistent.Console/  # Agente persistente (Azure AI Agent)
├── MAF.03.Functions.Api/                   # Agente hospedado em Azure Functions
├── UsingMiddlewares/                       # Middlewares de agente
├── Streaming/                              # Respostas em streaming
└── StructuredOutput/                       # Saída estruturada (ordem de serviço a partir de imagem)
Memory/                                     # Memória com VectorChatHistoryProvider
```

## 🚀 Começando

### Pré-requisitos

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- Conta no [Azure](https://azure.microsoft.com/) com acesso ao Azure OpenAI
- Visual Studio 2022, JetBrains Rider ou VS Code

### Configuração

1. Clone o repositório:
```bash
git clone https://github.com/carlosmachel/maf-playlist.git
cd maf-playlist
```

2. Configure as variáveis de ambiente criando um arquivo `.env` em cada projeto que precisar:
```env
AZURE_OPENAI_ENDPOINT=https://seu-recurso.openai.azure.com/
AZURE_OPENAI_API_KEY=sua-chave-api
AZURE_OPENAI_DEPLOYMENT_NAME=gpt-4.1-mini
```

3. Restaure as dependências:
```bash
dotnet restore
```

4. Execute um dos projetos:
```bash
cd src/MAF.01.Console
dotnet run
```

## 📚 Exemplos

### MAF.01.Console - Agente Básico

Exemplo introdutório demonstrando a criação de um agente de IA simples com o MAF.

```csharp
AIAgent agent = new AzureOpenAIClient(
    new Uri(endpoint),
    new AzureKeyCredential(apiKey))
    .GetChatClient(deploymentName)
    .CreateAIAgent(instructions: instructions, name: "LearnBuddy");
```

### Outros Exemplos

- **MAF.02**: Funcionalidades avançadas e persistência
- **MAF.03**: Integração com Azure Functions
- **UsingMiddlewares**, **Streaming**, **StructuredOutput** e **Memory**: middlewares, streaming, saída estruturada e memória

## 🎥 Série de Vídeos

Acompanhe a série completa no canal [AzureBrasil.Cloud](https://azurebrasil.cloud). A lista de todos os episódios e repositórios está no [hub da série](https://carlosmachel.github.io/maf-azurebrasil-hub/).

## 📝 Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 👤 Autor

**Carlos Machel**

- GitHub: [@carlosmachel](https://github.com/carlosmachel)
- LinkedIn: [carlosmachel](https://www.linkedin.com/in/carlosmachel/)

## 🤝 Contribuindo

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues ou pull requests.

## ⚠️ Observações

- Os projetos utilizam versões preview do Microsoft Agent Framework
- É necessário ter créditos Azure para utilizar o Azure OpenAI
- Estes exemplos usam chave de API para simplificar. Em produção, prefira Entra ID (`DefaultAzureCredential` / Managed Identity), como nos episódios mais recentes
- Nunca faça commit do `.env` com chaves
