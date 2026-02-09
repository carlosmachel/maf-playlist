# MAF-Series

Repositório com exemplos práticos do **Microsoft Agent Framework (MAF)** desenvolvidos para a série de vídeos no YouTube.

## 📋 Sobre o Projeto

Este repositório contém exemplos progressivos de implementação do Microsoft Agent Framework, demonstrando desde conceitos básicos até funcionalidades avançadas como persistência, middlewares e Azure Functions.

## 🛠️ Tecnologias

- **.NET 10.0**
- **Microsoft.Agents.AI** (preview)
- **Azure OpenAi**

## 📂 Estrutura do Projeto

```
src/
├── MAF.01.Console/                      # Exemplo básico de agente
├── MAF.02.Console/                      # Agente com recursos intermediários
├── MAF.02.AzureAiAgentPersistent.Console/ # Agente com persistência
├── MAF.03.Functions.Api/                # Azure Functions com MAF
├── MAF.03.Middleware/                   # Implementação de middlewares
├── MAF.03.ResponseClient.Api/           # Cliente de resposta
└── UsingMiddlewares/                    # Exemplos de uso de middlewares
```

## 🚀 Começando

### Pré-requisitos

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- Conta no [Azure](https://azure.microsoft.com/) com acesso ao Azure OpenAI
- Visual Studio 2022, JetBrains Rider ou VS Code

### Configuração

1. Clone o repositório:
```bash
git clone https://github.com/seu-usuario/MAF-Series.git
cd MAF-Series
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
- **MAF.03**: Integração com Azure Functions e middlewares

## 🎥 Série de Vídeos

Acompanhe a série completa no YouTube para explicações detalhadas de cada exemplo.

## 📝 Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 👤 Autor

**Carlos Machel**

- GitHub: [@carlosmachel](https://github.com/carlosmachel)

## 🤝 Contribuindo

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues ou pull requests.

## ⚠️ Observações

- Os projetos utilizam versões preview do Microsoft Agent Framework
- É necessário ter créditos Azure para utilizar o Azure OpenAI
- Lembre-se de nunca compartilhar suas chaves de API
