# Guia da apresentação: onde está cada requisito

Para cada coisa que a professora pediu, este guia diz **em qual arquivo e linha
está** e **o que falar**. Os links abrem o arquivo direto na linha, no VS Code.

> Dica: abram o projeto no VS Code antes de começar e deixem este arquivo aberto
> ao lado. Para saltar até um arquivo rápido, use `Ctrl+P` e digite o nome dele.

---

## Parte 1: Requisitos da N3

### 1. Service Pattern (Controller → Service)

| O que ela pode pedir | Onde fica |
|---|---|
| "Onde estão os Services?" | Pasta [Services/](Services/): [PacienteService.cs](Services/PacienteService.cs), [MedicoService.cs](Services/MedicoService.cs) e [ConsultaService.cs](Services/ConsultaService.cs) |
| "Mostra que o Controller não tem regra de negócio" | [PacientesController.cs:42-53](Controllers/PacientesController.cs#L42-L53): o POST só chama `_service.CriarAsync(dto)` e escolhe o status HTTP |
| "Onde ficou a regra do CPF duplicado?" | [PacienteService.cs:47](Services/PacienteService.cs#L47) |
| "Onde ficou a regra de não deletar com consulta futura?" | [PacienteService.cs:88](Services/PacienteService.cs#L88) e [MedicoService.cs:79](Services/MedicoService.cs#L79) |
| "Onde ficou a validação de conflito de horário?" | [ConsultaService.cs:91-127](Services/ConsultaService.cs#L91-L127) (`ValidarAgendamento`) |
| "Como o Service avisa o Controller que deu erro?" | [Resultado.cs](Services/Resultado.cs): três estados, `Sucesso`, `NaoEncontrado` e `Invalido`. O Controller transforma cada um em 200/201, 404 ou 400 ([PacientesController.cs:62-67](Controllers/PacientesController.cs#L62-L67)) |

**O que falar:** "O fluxo é Requisição → Controller → Service. O Controller só
cuida do HTTP: recebe o DTO, chama o Service e devolve o status. Toda a regra
de negócio e o acesso ao banco (DbContext) ficam no Service."

### 2. Pasta Interfaces com IPacienteMapper, IMedicoMapper e IConsultaMapper

| O que ela pode pedir | Onde fica |
|---|---|
| "Onde estão as interfaces dos mappers?" | [Interfaces/IPacienteMapper.cs](Interfaces/IPacienteMapper.cs), [Interfaces/IMedicoMapper.cs](Interfaces/IMedicoMapper.cs) e [Interfaces/IConsultaMapper.cs](Interfaces/IConsultaMapper.cs) |
| "Quem implementa?" | [PacienteMapper.cs:7](Mappers/PacienteMapper.cs#L7) (`public class PacienteMapper : IPacienteMapper`). Os outros dois seguem o mesmo padrão |
| "Tem mais interfaces?" | Sim, as dos Services também: [IPacienteService.cs](Interfaces/IPacienteService.cs), [IMedicoService.cs](Interfaces/IMedicoService.cs), [IConsultaService.cs](Interfaces/IConsultaService.cs), [IAuthService.cs](Interfaces/IAuthService.cs) e [ITokenService.cs](Interfaces/ITokenService.cs) |

**O que falar:** "Na N2 os mappers eram `static`. Agora são classes que
implementam uma interface. Quem usa o mapper depende só da interface e não da
classe concreta, e é isso que permite injetar o mapper."

### 3. Mappers injetados nos Services (DI)

| O que ela pode pedir | Onde fica |
|---|---|
| "Onde o mapper é injetado no Service?" | No construtor: [PacienteService.cs:15](Services/PacienteService.cs#L15) (`PacienteService(AppDbContext context, IPacienteMapper mapper)`). Também em [MedicoService.cs:14](Services/MedicoService.cs#L14) e [ConsultaService.cs:17](Services/ConsultaService.cs#L17) |
| "Onde está registrado?" | [Program.cs:23-25](Program.cs#L23-L25) |

### 4. Services injetados nos Controllers (DI)

| O que ela pode pedir | Onde fica |
|---|---|
| "Onde o Service é injetado no Controller?" | No construtor: [PacientesController.cs:18](Controllers/PacientesController.cs#L18), [MedicosController.cs:17](Controllers/MedicosController.cs#L17), [ConsultasController.cs:17](Controllers/ConsultasController.cs#L17) e [AuthController.cs:16](Controllers/AuthController.cs#L16) |
| "Onde está registrado?" | [Program.cs:28-30](Program.cs#L28-L30). Os serviços de autenticação estão em [Program.cs:33-35](Program.cs#L33-L35) |

**O que falar sobre DI:** "Ninguém faz `new` das dependências. A classe pede o
que precisa no construtor e o container de DI do ASP.NET entrega. O registro
fica no `Program.cs`: `AddScoped<Interface, Implementação>()`."

**Pergunta provável: "Por que `AddScoped`?"** Porque cria uma instância por
requisição HTTP. O `AppDbContext` também é Scoped, e os Services dependem dele.
`Singleton` seria uma instância só para a aplicação inteira, o que dá erro ao
usar um DbContext Scoped. `Transient` cria uma instância nova cada vez que alguém pede.

### 5. Autenticação JWT

| O que ela pode pedir | Onde fica |
|---|---|
| "Onde está o Usuario.cs?" | [Models/Usuario.cs](Models/Usuario.cs) |
| "Onde estão RegisterDTO e LoginDTO?" | [DTOs/RegisterDTO.cs](DTOs/RegisterDTO.cs) e [DTOs/LoginDTO.cs](DTOs/LoginDTO.cs) |
| "Onde está o AuthController?" | [Controllers/AuthController.cs](Controllers/AuthController.cs): `register` na [linha 22](Controllers/AuthController.cs#L22) e `login` na [linha 34](Controllers/AuthController.cs#L34) |
| "Onde o token é gerado?" | [TokenService.cs:20](Services/TokenService.cs#L20). As claims (o que vai dentro do token) estão nas [linhas 31-37](Services/TokenService.cs#L31-L37) |
| "Onde a API valida o token?" | [Program.cs:40-56](Program.cs#L40-L56) (`AddAuthentication` + `AddJwtBearer`) e [Program.cs:69](Program.cs#L69) (`app.UseAuthentication()`) |
| "Onde está a chave secreta?" | [appsettings.json:9](appsettings.json#L9) (seção `Jwt`) |
| "Como a senha é guardada?" | Como hash, nunca em texto puro: [AuthService.cs:56](Services/AuthService.cs#L56) (`HashPassword`). No login a conferência é em [AuthService.cs:79](Services/AuthService.cs#L79) (`VerifyHashedPassword`) |
| "Onde a tabela Usuarios foi criada?" | [AppDbContext.cs:17](Data/AppDbContext.cs#L17) e a migration [Migrations/20261107131720_AddUsuarios.cs](Migrations/20261107131720_AddUsuarios.cs) |
| "Mostra que só funciona com token" | `[Authorize]` na classe: [PacientesController.cs:12](Controllers/PacientesController.cs#L12), [MedicosController.cs:12](Controllers/MedicosController.cs#L12) e [ConsultasController.cs:12](Controllers/ConsultasController.cs#L12) |

**O que falar:** "O usuário faz login em `/api/auth/login` e recebe um token
JWT assinado com a nossa chave secreta (HMAC-SHA256), que vale 120 minutos. Nas
outras requisições ele manda `Authorization: Bearer <token>`. O
`UseAuthentication` valida a assinatura, o emissor, o público e a validade.
Sem token ou com token inválido, a resposta é **401**."

### 6. Autorização: usuário comum × admin

| O que ela pode pedir | Onde fica |
|---|---|
| "Onde estão os dois tipos de usuário?" | [Usuario.cs:19-23](Models/Usuario.cs#L19-L23) (`Perfis.Comum` e `Perfis.Admin`) e a propriedade `Perfil` na [linha 15](Models/Usuario.cs#L15) |
| "Onde só admin pode fazer PATCH e DELETE?" | `[Authorize(Roles = Perfis.Admin)]` em: Pacientes [L57](Controllers/PacientesController.cs#L57) e [L72](Controllers/PacientesController.cs#L72), Médicos [L56](Controllers/MedicosController.cs#L56) e [L71](Controllers/MedicosController.cs#L71), Consultas [L56](Controllers/ConsultasController.cs#L56) e [L71](Controllers/ConsultasController.cs#L71) |
| "Como a API sabe se o usuário é admin?" | A role vai dentro do token: [TokenService.cs:36](Services/TokenService.cs#L36) (`ClaimTypes.Role`) |
| "Qual a estratégia escolhida?" | Autorização baseada em papéis (roles). Comentário em [Program.cs:58-60](Program.cs#L58-L60) |
| "Como o admin é criado?" | [Data/SeedAdmin.cs](Data/SeedAdmin.cs), chamado na subida em [Program.cs:65](Program.cs#L65). Os dados vêm de [appsettings.json:15](appsettings.json#L15) |
| "E se alguém se registrar como admin?" | Não tem como. O registro sempre cria usuário Comum: [AuthService.cs:54](Services/AuthService.cs#L54) |

**O que falar:** "Autenticação responde 'quem é você?' e autorização responde
'o que você pode fazer?'. Usuário comum pode GET e POST. Se tentar PATCH ou
DELETE, recebe **403 Forbidden**: a API sabe quem ele é, mas ele não tem permissão."

**401 × 403:** 401 = não autenticado (sem token ou token inválido).
403 = autenticado, mas sem permissão (usuário comum tentando PATCH/DELETE).

---

## Parte 2: Requisitos da N2 (continuam valendo)

| Requisito | Onde fica |
|---|---|
| Models: Paciente, Médico, Consulta | [Models/](Models/) |
| Controllers dos três recursos | [Controllers/](Controllers/) |
| AppDbContext e SQLite | [Data/AppDbContext.cs](Data/AppDbContext.cs) e [Program.cs:18-19](Program.cs#L18-L19) (`UseSqlite`) |
| DTOs Create, Read e Update | [DTOs/](DTOs/) |
| Mappers manuais | [Mappers/](Mappers/) |
| GET, GET por id, POST, PATCH e DELETE | Cada Controller, marcados com `[HttpGet]`, `[HttpPost]` etc. Exemplo: [PacientesController.cs](Controllers/PacientesController.cs) |
| Email válido | [Email.cs:7-8](Models/ValueObjects/Email.cs#L7-L8) (regex), usado em [ValidacaoDeCampos.cs:29](Mappers/ValidacaoDeCampos.cs#L29) |
| Telefone `(47) 98888-7777` sem `[Phone]` | [Telefone.cs:7-8](Models/ValueObjects/Telefone.cs#L7-L8) (regex) |
| Data de nascimento não pode ser no futuro | [DataNascimento.cs:9](Models/ValueObjects/DataNascimento.cs#L9) |
| CPF válido (dígitos verificadores) | [Cpf.cs:13](Models/ValueObjects/Cpf.cs#L13) |
| CPF não pode ser alterado no PATCH | [PacienteMapper.cs:33-34](Mappers/PacienteMapper.cs#L33-L34) |
| CPF duplicado no POST | [PacienteService.cs:47](Services/PacienteService.cs#L47). No banco também há índice único: [AppDbContext.cs:40](Data/AppDbContext.cs#L40) |
| PATCH: campo `null` não é atualizado | [PacienteMapper.cs:30-51](Mappers/PacienteMapper.cs#L30-L51) (cada campo só é aplicado se `is not null`) |
| Consulta: paciente e médico existem | [ConsultaService.cs:96-100](Services/ConsultaService.cs#L96-L100) |
| Consulta: não pode ser no passado | [ConsultaService.cs:102](Services/ConsultaService.cs#L102) |
| Consulta: 30 minutos e sem sobreposição | [JanelaConsulta.cs:5](Models/ValueObjects/JanelaConsulta.cs#L5) (duração) e [JanelaConsulta.cs:15](Models/ValueObjects/JanelaConsulta.cs#L15) (`Sobrepoe`), usados em [ConsultaService.cs:112-116](Services/ConsultaService.cs#L112-L116) |
| PATCH da consulta valida tudo de novo | [ConsultaService.cs:62-72](Services/ConsultaService.cs#L62-L72) (se der erro, `ReloadAsync` desfaz a alteração) |
| Não deletar paciente ou médico com consulta futura | [PacienteService.cs:88](Services/PacienteService.cs#L88) e [MedicoService.cs:79](Services/MedicoService.cs#L79) |

---

## Parte 3: Roteiro da demonstração no Postman

Antes de começar: banco limpo (pare a API, apague o `clinica.db` e suba de novo)
e a collection `ApiClinica.postman_collection.json` importada.

1. **`0. Saúde`**: a API está no ar.
2. **`GET pacientes sem token`** (pasta 8): mostrar o **401**. "Sem token, nada funciona."
3. **`0.1 Autenticação`**: login do admin. Mostrar o token na resposta e,
   se der tempo, colar em [jwt.io](https://jwt.io) para mostrar a claim de role.
   Fazer também o registro e o login do usuário comum.
4. **Pastas 1 a 3**: CRUD funcionando com o token do admin.
5. **Pastas 4 a 6**: as validações da N2 dando 400.
6. **Pasta 7**: DELETE bloqueado por consulta futura.
7. **Pasta 8**: usuário comum consegue GET e POST, mas **PATCH e DELETE dão 403**.

Para rodar tudo de uma vez: clique com o botão direito na collection e escolha **Run collection**.

---

## Parte 4: O caminho de uma requisição

Exemplo: `PATCH /api/pacientes/1` com token de admin.

1. **`UseAuthentication`** ([Program.cs:69](Program.cs#L69)) lê o header
   `Authorization`, valida o token e monta o usuário com as claims.
2. **`UseAuthorization`** ([Program.cs:70](Program.cs#L70)) vê o
   `[Authorize(Roles = "Admin")]` e confere a role. Se não for Admin, devolve 403 e para aqui.
3. **Controller** ([PacientesController.cs:58](Controllers/PacientesController.cs#L58))
   recebe o `PacienteUpdateDTO` e chama `_service.AtualizarAsync(id, dto)`.
4. **Service** ([PacienteService.cs:59](Services/PacienteService.cs#L59)) busca o
   paciente no banco (404 se não existir), usa o **mapper** para validar e aplicar
   as mudanças, e salva com `SaveChangesAsync`.
5. **Mapper** ([PacienteMapper.cs:30](Mappers/PacienteMapper.cs#L30)) valida os
   campos com os Value Objects e só altera os que não vieram `null`.
6. O Service devolve um `Resultado`, e o Controller responde **200** com o
   `PacienteReadDTO`, **400** com os erros ou **404**.

---

## Sugestão de divisão para estudar

Todos precisam conhecer o projeto inteiro, porque as perguntas são aleatórias,
mas cada um pode aprofundar um tema:

- **Pessoa 1:** Service Pattern e o tipo `Resultado` (Parte 1, item 1)
- **Pessoa 2:** Interfaces e Injeção de Dependências (itens 2 a 4)
- **Pessoa 3:** JWT: login, token e senha com hash (item 5)
- **Pessoa 4:** Autorização, roles, 401 × 403 e SeedAdmin (item 6)
- **Pessoa 5:** Requisitos da N2 e a demonstração no Postman (Partes 2 e 3)
