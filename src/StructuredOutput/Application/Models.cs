namespace StructuredOutput.Application;

public class OrdemServicoDto
{
    public EmpresaDto Empresa { get; set; } = default!;

    public string NumeroOs { get; set; } = default!;

    public DateTime Data { get; set; }

    public ClienteDto Cliente { get; set; } = default!;

    public List<ItemServicoDto> Itens { get; set; } = new();

    public string? Observacoes { get; set; }

    public decimal ValorTotal { get; set; }

    public AssinaturaDto TecnicoResponsavel { get; set; } = default!;

    public AssinaturaDto ClienteAssinatura { get; set; } = default!;

    public DateTime? PrevisaoEntrega { get; set; }

    public string? Termo { get; set; }
}

public class EmpresaDto
{
    public string Nome { get; set; } = default!;

    public string Cnpj { get; set; } = default!;

    public string Endereco { get; set; } = default!;
}

public class ClienteDto
{
    public string Nome { get; set; } = default!;

    public string Telefone { get; set; } = default!;

    public string Endereco { get; set; } = default!;

    public string Cpf { get; set; } = default!;
}

public class ItemServicoDto
{
    public int Item { get; set; }

    public string Descricao { get; set; } = default!;

    public int Quantidade { get; set; }

    public decimal ValorUnitario { get; set; }

    public decimal ValorTotal { get; set; }
}

public class AssinaturaDto
{
    public string Nome { get; set; } = default!;

    /// <summary>
    /// Conteúdo extraído da região da assinatura (se houver OCR / handwriting)
    /// </summary>
    public string? TextoAssinatura { get; set; }
}
