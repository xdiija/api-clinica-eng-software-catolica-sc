# Ambiente Docker

O projeto exige .NET 10 e a máquina só tem o SDK 8, então o desenvolvimento roda
em container. O código fica no host (bind mount) — você edita normalmente na IDE
e o `dotnet watch` recompila dentro do container.

Defina o UID/GID em cada comando para que os arquivos gerados dentro do
container (migrations, `clinica.db`) pertençam ao seu usuário, e não ao root:

```sh
export DOCKER_UID=$(id -u) DOCKER_GID=$(id -g)
```

## Subir e derrubar

```sh
docker compose up -d          # sobe a API em http://localhost:5070
docker compose logs -f api    # acompanha o build e os logs
docker compose down           # derruba
docker compose up -d --build  # rebuild da imagem (ex.: mudou o Dockerfile.dev)
```

Base da URL para o Postman: `http://localhost:5070/api/...` — **http**, não https.

Para saber se subiu:

```sh
curl http://localhost:5070/alive     # {"status":"alive","utc":"..."}
docker compose ps                    # a coluna STATUS mostra (healthy)
```

O Compose usa `/alive` como healthcheck, então `docker compose ps` distingue
"container no ar" de "API respondendo" — útil porque o primeiro build do
`dotnet watch` leva alguns segundos depois do `up`.

## Comandos avulsos

```sh
docker compose run --rm --no-deps api dotnet build ApiClinica.csproj
docker compose exec api dotnet --version
```

## Migrations

Com o container no ar (`up -d`), use `exec`. O `--no-build` evita conflito com o
build que o `dotnet watch` já mantém em andamento:

```sh
docker compose exec api dotnet ef migrations list --no-build
docker compose exec api dotnet ef migrations add NomeDaMigration
docker compose exec api dotnet ef database update
```

## Detalhes que importam

- **Portas:** `5070` no host → `8080` no container. A porta interna vem de
  `ASPNETCORE_URLS=http://+:8080` no `Dockerfile.dev`.
- **`--no-launch-profile`:** o `CMD` ignora `Properties/launchSettings.json` de
  propósito. Aquele arquivo fixa a URL em `localhost:5070`, e "localhost" dentro
  do container não é alcançável pelo mapeamento de portas. O arquivo continua
  valendo se você rodar pela IDE no host.
- **`bin/` e `obj/` são volumes nomeados**, não o diretório do host. Isso isola
  os artefatos do .NET 10 de qualquer build feito com outro SDK. Para limpar:
  `docker compose down -v` (isso também apaga o cache do NuGet do volume).
- **`clinica.db` fica no host**, via bind mount, então os dados sobrevivem a
  `down`/`up`.
