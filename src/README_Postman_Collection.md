# StoreManagement API & Functions - Postman Collection

Esta collection completa permite testar todas as APIs do projeto StoreManagement, incluindo a API REST e as Azure Functions, com testes automatizados abrangentes.

## 📁 Arquivos Incluídos

- `StoreManagement_API_Collection.postman_collection.json` - Collection principal com todos os endpoints e testes
- `StoreManagement_Environment.postman_environment.json` - Arquivo de ambiente com variáveis configuráveis
- `README_Postman_Collection.md` - Este arquivo com instruções de uso

## 🚀 Como Importar no Postman

### 1. Importar a Collection
1. Abra o Postman
2. Clique em **Import** no canto superior esquerdo
3. Selecione o arquivo `StoreManagement_API_Collection.postman_collection.json`
4. Clique em **Import**

### 2. Importar o Environment
1. No Postman, clique no ícone de **Environments** (engrenagem) no canto superior direito
2. Clique em **Import**
3. Selecione o arquivo `StoreManagement_Environment.postman_environment.json`
4. Clique em **Import**
5. Selecione o environment "StoreManagement Environment" no dropdown

## ⚙️ Configuração Inicial

### URLs Base
As seguintes variáveis estão pré-configuradas no environment:

- **apiBaseUrl**: `http://localhost:5076` (API REST)
- **apiBaseUrlHttps**: `https://localhost:7063` (API REST HTTPS)
- **functionsBaseUrl**: `http://localhost:7071` (Azure Functions)

### Function Key (Opcional)
Para testar as Azure Functions, você pode configurar a chave de função:
1. No environment, encontre a variável `functionKey`
2. Defina o valor da chave de função (se necessário)

## 📋 Estrutura da Collection

### 1. **Companies API** (9 endpoints)
- **GET** `/api/companies` - Listar todas as empresas
- **GET** `/api/companies/active` - Listar empresas ativas
- **GET** `/api/companies/{code}` - Buscar empresa por código
- **GET** `/api/companies/{code}/with-stores` - Buscar empresa com lojas
- **POST** `/api/companies` - Criar empresa
- **PUT** `/api/companies/{code}` - Atualizar empresa completa
- **PATCH** `/api/companies/{code}` - Atualizar empresa parcial
- **DELETE** `/api/companies/{code}` - Deletar empresa
- **GET** `/api/companies/999999` - Teste de erro 404

### 2. **Stores API** (7 endpoints)
- **GET** `/api/stores` - Listar todas as lojas
- **GET** `/api/stores/company/{companyCode}` - Listar lojas por empresa
- **GET** `/api/stores/{id}` - Buscar loja por ID
- **POST** `/api/stores` - Criar loja
- **PUT** `/api/stores/{id}` - Atualizar loja completa
- **PATCH** `/api/stores/{id}` - Atualizar loja parcial
- **DELETE** `/api/stores/{id}` - Deletar loja

### 3. **Products API** (6 endpoints)
- **GET** `/api/products/store/{storeCode}` - Listar produtos por loja
- **GET** `/api/products/{id}` - Buscar produto por ID
- **POST** `/api/products` - Criar produto
- **PUT** `/api/products/{id}` - Atualizar produto completo
- **PATCH** `/api/products/{id}` - Atualizar produto parcial
- **DELETE** `/api/products/{id}` - Deletar produto

### 4. **Azure Functions** (5 endpoints)
- **GET** `/api/products/store/{storeCode}` - Listar produtos por loja (Function)
- **GET** `/api/products/{id}` - Buscar produto por ID (Function)
- **POST** `/api/products` - Criar produto (Function)
- **PUT** `/api/products/{id}` - Atualizar produto (Function)
- **DELETE** `/api/products/{id}` - Deletar produto (Function)

### 5. **Error Scenarios** (3 endpoints)
- Teste de código de empresa inválido (400/404)
- Teste de código de loja inválido para produto (400/404)
- Teste de JSON inválido (400)

## 🧪 Testes Automatizados

Cada endpoint inclui testes automatizados que verificam:

### ✅ Testes de Status Code
- Códigos de resposta corretos (200, 201, 204, 400, 404)
- Validação de cenários de sucesso e erro

### ✅ Testes de Schema
- Estrutura correta dos objetos JSON
- Presença de campos obrigatórios
- Tipos de dados corretos

### ✅ Testes de Dados
- Validação de valores específicos
- Consistência de dados entre requests
- Relacionamentos entre entidades

### ✅ Testes de Performance
- Tempo de resposta (< 2000ms para API, < 5000ms para Functions)
- Validação de eficiência

### ✅ Testes de Negócio
- Regras específicas (ex: empresas ativas, relacionamentos)
- Validação de lógica de negócio

## 🔄 Fluxo de Variáveis Dinâmicas

A collection utiliza variáveis dinâmicas para criar um fluxo de testes integrado:

1. **Captura Automática**: IDs e códigos são capturados automaticamente dos responses
2. **Reutilização**: Variáveis são reutilizadas em requests subsequentes
3. **Cleanup**: Dados de teste são limpos automaticamente

### Variáveis Principais:
- `companyId`, `companyCode` - Dados da primeira empresa
- `storeId`, `storeCode` - Dados da primeira loja
- `productId` - ID do primeiro produto
- `createdXXXId` - IDs de entidades criadas para teste
- `newXXXCode` - Códigos únicos gerados dinamicamente

## 🎯 Como Executar os Testes

### Execução Individual
1. Selecione um request específico
2. Clique em **Send**
3. Verifique os resultados dos testes na aba **Test Results**

### Execução em Lote (Pasta)
1. Clique com o botão direito em uma pasta (ex: "Companies API")
2. Selecione **Run collection**
3. Configure as opções de execução
4. Clique em **Run**

### Execução Completa da Collection
1. Clique com o botão direito na collection principal
2. Selecione **Run collection**
3. Configure as opções (delay entre requests, iterações, etc.)
4. Clique em **Run**

## 📊 Interpretando os Resultados

### ✅ Teste Passou
- Indicado por um checkmark verde
- Todos os critérios foram atendidos

### ❌ Teste Falhou
- Indicado por um X vermelho
- Verifique a mensagem de erro para detalhes
- Pode indicar problema na API ou dados de teste

### ⚠️ Avisos
- Alguns testes podem ser condicionais (ex: só executam se há dados)
- Verifique os logs para entender o comportamento

## 🛠️ Personalização

### Modificar URLs
1. Vá para o environment "StoreManagement Environment"
2. Modifique as variáveis `apiBaseUrl` ou `functionsBaseUrl`
3. Salve as alterações

### Adicionar Novos Testes
1. Selecione um request
2. Vá para a aba **Tests**
3. Adicione novos scripts de teste em JavaScript
4. Use a sintaxe do Postman: `pm.test("Nome do teste", function() { ... })`

### Modificar Dados de Teste
1. Selecione um request POST/PUT/PATCH
2. Vá para a aba **Body**
3. Modifique os dados JSON conforme necessário

## 🔧 Troubleshooting

### Problema: "Cannot read property of undefined"
- **Causa**: Variável não foi definida ou capturada
- **Solução**: Execute primeiro os requests que capturam as variáveis (ex: "Get All Companies")

### Problema: Testes falhando com 404
- **Causa**: APIs não estão rodando ou URLs incorretas
- **Solução**: Verifique se as APIs estão rodando nas portas corretas

### Problema: Function Key inválida
- **Causa**: Chave de função não configurada ou incorreta
- **Solução**: Configure a variável `functionKey` no environment

### Problema: Dados inconsistentes
- **Causa**: Banco de dados vazio ou dados corrompidos
- **Solução**: Execute as migrações do banco e popule com dados de teste

### Problema: AutoMapper Error - "Mapping types: Product -> ProductDto"
- **Causa**: O AutoMapper está tentando mapear Product para ProductDto, mas as propriedades de navegação (Store) não estão carregadas
- **Sintomas**: Erro ao criar/atualizar produtos via API ou Functions
- **Soluções**:
  1. **Solução Imediata**: Modifique o AutoMapperProfile para ignorar propriedades que podem ser null:
     ```csharp
     CreateMap<Product, ProductDto>()
         .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : string.Empty))
         .ForMember(dest => dest.StoreCode, opt => opt.MapFrom(src => src.Store != null ? src.Store.Code : 0));
     ```
  2. **Solução Recomendada**: No ProductService, carregue a entidade Store após criar/atualizar:
     ```csharp
     await _productRepository.AddAsync(product);
     await _unitOfWork.CommitAsync();
     
     // Recarregar com Store incluído
     var productWithStore = await _productRepository.GetByIdWithStoreAsync(product.Id);
     return _mapper.Map<ProductDto>(productWithStore);
     ```
  3. **Solução Alternativa**: Mapear manualmente no ProductService:
     ```csharp
     return new ProductDto
     {
         Id = product.Id,
         Name = product.Name,
         Code = product.Code,
         Description = product.Description,
         Price = product.Price,
         IsActive = product.IsActive,
         CreatedAt = product.CreatedAt,
         StoreId = product.StoreId,
         StoreName = store.Name,
         StoreCode = store.Code
     };
     ```

### Problema: Entity Framework Navigation Properties não carregadas
- **Causa**: Lazy loading desabilitado ou Include() não utilizado
- **Solução**: Adicione métodos no repository para carregar com Include:
  ```csharp
  public async Task<Product?> GetByIdWithStoreAsync(Guid id)
  {
      return await _context.Products
          .Include(p => p.Store)
          .FirstOrDefaultAsync(p => p.Id == id);
  }
  ```

## 📝 Exemplos de Uso

### Cenário 1: Teste Completo de CRUD
1. Execute "Get All Companies" para capturar dados existentes
2. Execute "Create Company" para criar nova empresa
3. Execute "Update Company (PUT)" para atualizar
4. Execute "Patch Company (PATCH)" para atualização parcial
5. Execute "Delete Company" para limpar

### Cenário 2: Teste de Relacionamentos
1. Execute "Get All Companies" para capturar companyCode
2. Execute "Get Stores by Company Code" para verificar relacionamento
3. Execute "Create Store" usando o companyCode capturado
4. Execute "Create Product" usando o storeCode da loja criada

### Cenário 3: Comparação API vs Functions
1. Execute requests da pasta "Products API"
2. Execute requests equivalentes da pasta "Azure Functions"
3. Compare os resultados e performance

## 🎉 Recursos Avançados

### Pre-request Scripts
- Geração automática de códigos únicos
- Configuração dinâmica de dados
- Validações pré-execução

### Collection Variables
- Compartilhamento de dados entre requests
- Persistência durante a sessão
- Cleanup automático

### Conditional Testing
- Testes que se adaptam aos dados disponíveis
- Validações condicionais baseadas no response
- Tratamento de cenários edge cases

---

## 📞 Suporte

Para dúvidas ou problemas:
1. Verifique este README primeiro
2. Consulte a documentação oficial do Postman
3. Verifique os logs da API para erros específicos
4. Teste individualmente cada endpoint para isolar problemas

**Versão da Collection**: 1.0  
**Compatibilidade**: Postman 9.0+  
**Última Atualização**: Janeiro 2025
