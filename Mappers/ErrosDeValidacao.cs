using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ApiClinica.Mappers;

public sealed class ErrosDeValidacao
{
    private readonly Dictionary<string, List<string>> _erros = new();

    public bool Algum => _erros.Count > 0;

    public void Adicionar(string campo, string mensagem)
    {
        if (!_erros.TryGetValue(campo, out var mensagens))
        {
            mensagens = [];
            _erros[campo] = mensagens;
        }

        mensagens.Add(mensagem);
    }

    public ModelStateDictionary ParaModelState()
    {
        var modelState = new ModelStateDictionary();

        foreach (var (campo, mensagens) in _erros)
            foreach (var mensagem in mensagens)
                modelState.AddModelError(campo, mensagem);

        return modelState;
    }
}
