# ApiClinica — Trabalho N3

API REST em C# / .NET 10 com EF Core + SQLite, DTOs, Mappers, Service Pattern,
Injeção de Dependências, autenticação JWT e autorização por papéis.

## Como rodar

**Opção 1: .NET 10 instalado na máquina**

```sh
dotnet run --launch-profile http
```

**Opção 2: Docker** (ver [README.docker.md](README.docker.md))

```sh
docker compose up -d
```

Nas duas opções a API fica em **http://localhost:5070** (http, não https).
Para conferir se subiu: `GET http://localhost:5070/alive`.

Ao subir, a API aplica sozinha as migrations pendentes e cria o usuário Admin
inicial se ele ainda não existir. Não é preciso rodar `dotnet ef database update`.

## Usuários

| Perfil | Como obter                                      | Pode fazer                          |
|--------|-------------------------------------------------|-------------------------------------|
| Admin  | Criado na subida: `admin@clinica.com` / `Admin@123` | GET, POST, PATCH e DELETE          |
| Comum  | `POST /api/auth/register`                       | GET e POST (PATCH/DELETE → **403**) |

Sem token, qualquer rota de pacientes, médicos ou consultas responde **401**.
O Admin inicial pode ser alterado em `appsettings.json`, na seção `AdminInicial`.

## Testando no Postman

1. Importe `ApiClinica.postman_collection.json`.
2. Comece com o banco limpo (o `clinica.db` entregue já está limpo, só com o Admin).
3. Rode as pastas em ordem (ou o Collection Runner inteiro). A pasta
   **0.1 Autenticação** faz login e guarda os tokens; o resto da collection usa
   `Bearer {{token}}` automaticamente. A pasta **8. Segurança** testa os 401 e 403.

Para voltar o banco ao estado limpo: pare a API, apague `clinica.db` e suba de novo.

## Endpoints

| Método | Rota                       | Acesso         |
|--------|----------------------------|----------------|
| POST   | `/api/auth/register`       | Público        |
| POST   | `/api/auth/login`          | Público        |
| GET    | `/api/{recurso}`           | Logado         |
| GET    | `/api/{recurso}/{id}`      | Logado         |
| POST   | `/api/{recurso}`           | Logado         |
| PATCH  | `/api/{recurso}/{id}`      | Somente Admin  |
| DELETE | `/api/{recurso}/{id}`      | Somente Admin  |

`{recurso}` = `pacientes`, `medicos` ou `consultas`.
O token vai no header: `Authorization: Bearer <token>`.

## Arquitetura

```
Requisição → Controller → Service → Mapper / AppDbContext → SQLite
```

- **Controllers/**: só recebem a requisição, chamam o Service e escolhem o
  status HTTP. Não têm regra de negócio. `AuthController` faz registro e login.
- **Services/**: regras de negócio (CPF duplicado, conflito de horário,
  bloqueio de DELETE com consulta futura, login/registro, geração do token).
  Devolvem um `Resultado` (Sucesso, NaoEncontrado ou Invalido) que o Controller
  traduz em 200/201, 404 ou 400.
- **Interfaces/**: `IPacienteMapper`, `IMedicoMapper`, `IConsultaMapper` e as
  interfaces dos Services e do token.
- **Mappers/**: conversão manual DTO ↔ Model, com validação dos campos.
- **Models/**: `Paciente`, `Medico`, `Consulta`, `Usuario` e os Value Objects.
- **DTOs/**: Create/Read/Update de cada entidade, `RegisterDTO`, `LoginDTO`,
  `TokenDTO` e `UsuarioReadDTO`.
- **Data/**: `AppDbContext` e `SeedAdmin` (cria o Admin inicial).

### Injeção de Dependências

Tudo é registrado em `Program.cs` com `AddScoped`:

- Mappers → injetados nos Services (`PacienteService(AppDbContext, IPacienteMapper)`).
- Services → injetados nos Controllers (`PacientesController(IPacienteService)`).
- `IPasswordHasher<Usuario>` e `ITokenService` → injetados no `AuthService`.

### Segurança

- **Autenticação (JWT):** o login confere a senha (salva como hash pelo
  `PasswordHasher` do ASP.NET Core, nunca em texto puro) e devolve um token
  assinado com HMAC-SHA256, válido por 120 minutos. Chave, issuer e audience
  ficam em `appsettings.json`, seção `Jwt`.
- **Autorização (roles):** o token carrega a claim de role (`Comum` ou `Admin`).
  Os controllers têm `[Authorize]` na classe e `[Authorize(Roles = "Admin")]`
  nos métodos PATCH e DELETE.
- O registro público sempre cria usuário **Comum**, para ninguém se cadastrar
  como Admin pela API. O primeiro Admin é criado na subida (`Data/SeedAdmin.cs`).
