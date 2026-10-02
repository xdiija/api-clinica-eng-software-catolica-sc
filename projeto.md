● O código deve estar funcional e será testado no Postman.
● A API deve ser implementada em C#, .NET 10.
● A API deve conter:
    ○ Diretório Models com Paciente, Médico e Consulta.
    ○ Diretório Controllers com PacientesController, MedicosController e ConsultasController.
    ○ Diretório Data com AppDbContext.
    ○ Diretório DTOs com:
        ■ PacienteCreateDTO, PacienteReadDTO, PacienteUpdateDTO
        ■ MedicoCreateDTO, MedicoReadDTO, MedicoUpdateDTO
        ■ ConsultaCreateDTO, ConsultaReadDTO, ConsultaUpdateDTO

    ○ Diretório Mappers com PacienteMapper, MedicoMapper e ConsultaMapper implementados de forma manual.
    ○ Controllers devem receber e retornar apenas DTOs.
    ○ PacientesController deve conter os métodos GET, GET by Id, POST, PATCH, e DELETE.
    ○ PacientesController deve validar as seguintes informações nos métodos PATCH e POST:
        ■ Email com formato válido.
        ■ Telefone com formato válido: (47) 98888-7777. Não use o atributo [Phone] do C#.
        ■ Data de nascimento não pode ser no futuro.
        ■ CPF válido numericamente (CPF não pode ser alterado no PATCH).
        ■ Ao criar Paciente (POST) deve validar se já existe um Paciente com o mesmo CPF.

    ○ MedicosController deve conter os métodos GET, GET by Id, POST, PATCH, e DELETE.
    ○ MedicosController deve validar as seguintes informações nos métodos PATCH e POST:
        ■ Email com formato válido.
        ■ Telefone com formato válido: (47) 98888-7777.

    ○ ConsultasController deve conter os métodos GET, GET by Id, POST, PATCH, e DELETE.
    ○ ConsultasController deve validar as seguintes informações nos métodos PATCH e POST:
        ■ Id do paciente é válido, paciente existe.
        ■ Id do médico é válido, médico existe.
        ■ Não deve ser possível agendar consulta no passado.
        ■ Não deve ser possível agendar consulta em dois horários iguais para o mesmo médico.
        ■ Não deve ser possível agendar consulta em dois horários iguais para o mesmo paciente.
        ■ Não pode haver sobreposição de horários.
            ● Cada consulta tem duração de 30 minutos.
            ● Exemplo: 10:00 - 10:30, 10:15 - 10:45 (sobreposição)
            ● Se o id do paciente ou do médico for alterado, essa regra deverá ser validada novamente.

    ○ No método PATCH, se o sistema receber null para algum campo, deve-se considerar que esse campo não deve ser atualizado.
    ○ Não deve ser possível remover (DELETE) um Paciente ou um Médico que tenha Consultas no futuro.
    ○ Atenção: todos os métodos (GET, POST, PATCH, DELETE) dos pacientes, médicos e consultas devem fazer uso do DbContext.
    ○ O banco de dados utilizado deve ser o SQLite.

● Estrutura do Paciente, Médico e Consulta:

Paciente {
    Id,
    Nome,
    Email,
    Telefone,
    DataNasc,
    Cpf
}

Médico {
    Id,
    Nome,
    Email,
    Telefone,
    CRM
}

Consulta {
    Id,
    PacienteId,
    MedicoId,
    DataHora
}


● Objetos de Valor (Value Objects) que serão implementados, substituindo tipos primitivos:

    ○ Cpf
        ■ Valida os dígitos verificadores.
        ■ Normaliza a entrada (aceita com ou sem máscara, armazena 11 dígitos).
        ■ Igualdade pelo valor normalizado (usada na checagem de CPF duplicado no POST).
        ■ Imutável: não pode ser alterado no PATCH.

    ○ Telefone
        ■ Valida o formato (47) 98888-7777, sem usar o atributo [Phone] do C#.
        ■ Expõe DDD e número separadamente e reconstrói a máscara na saída.

    ○ Email
        ■ Valida o formato.
        ■ Normaliza (Trim e minúsculas), tornando a igualdade insensível a maiúsculas.

    ○ JanelaConsulta
        ■ Duração fixa de 30 minutos; calcula o horário final a partir do inicial.
        ■ Sobreposição: método Sobrepoe, que cobre também o caso de horários idênticos.
        ■ Não permite horário no passado.

    ○ DataNascimento
        ■ Não pode ser no futuro.
        ■ Calcula a idade.

    ○ Fronteiras da arquitetura:
        ■ DTOs continuam usando tipos primitivos (contrato HTTP).
        ■ A conversão e a validação acontecem nos Mappers, via TryParse/Create que
          retorna sucesso ou erro (sem lançar exceção, para permitir resposta 400 por campo).
        ■ Models usam os objetos de valor, com init em vez de set onde o valor é imutável.
        ■ Persistência via HasConversion no AppDbContext, com construtor de
          reconstituição (sem validação) para o caminho banco → objeto.
