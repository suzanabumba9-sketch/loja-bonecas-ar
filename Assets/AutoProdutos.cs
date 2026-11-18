using UnityEngine;

// Preenche automaticamente os dados das bonecas ao iniciar.
// Coloque no MESMO objeto vazio do ProductClickSimple ("GerenteDeCliques").
// Edite os textos abaixo (nome, preco, estoque, descricao) como quiser.
[DefaultExecutionOrder(-100)]
public class AutoProdutos : MonoBehaviour
{
    [Tooltip("Nome do objeto da loja na Hierarchy.")]
    public string nomeDaLoja = "loja_bonecas";

    [Tooltip("Simbolo da moeda, ex.: R$, MT, Kz, EUR")]
    public string moeda = "$";

    [System.Serializable]
    public class Dado
    {
        public string nome;
        public float preco;
        public int estoque;
        [TextArea(2, 5)] public string descricao;
    }

    public Dado[] catalogo = new Dado[]
    {
        new Dado { nome = "Princesa Aurora",  preco = 45.90f, estoque = 12, descricao = "Boneca de princesa com vestido brilhante e coroa. Ideal para brincar de faz de conta. Indicada a partir de 3 anos." },
        new Dado { nome = "Boneca de Pano",   preco = 29.90f, estoque = 20, descricao = "Boneca macia e lavavel, feita de tecido. Perfeita para criancas pequenas e para dormir abracada." },
        new Dado { nome = "Fada Luna",        preco = 52.00f, estoque = 8,  descricao = "Fada com asinhas coloridas. Estimula a imaginacao e as brincadeiras de historias." },
        new Dado { nome = "Boneca Bailarina", preco = 39.50f, estoque = 15, descricao = "Bailarina articulada, com sapatilhas e tutu. Ajuda a desenvolver a coordenacao motora." },
        new Dado { nome = "Boneca Astronauta",preco = 49.90f, estoque = 6,  descricao = "Astronauta explorador do espaco. Incentiva a curiosidade pela ciencia." },
        new Dado { nome = "Boneca Cozinheira",preco = 34.90f, estoque = 10, descricao = "Pequena chef com avental e chapeu. Combina com kits de cozinha de brinquedo." },
        new Dado { nome = "Boneca Exploradora", preco = 42.00f, estoque = 9, descricao = "Exploradora de aventuras com mochila e chapeu. Para brincar ao ar livre e imaginar viagens." },
        new Dado { nome = "Boneca Cantora",   preco = 55.00f, estoque = 5,  descricao = "Cantora com microfone de brinquedo. Perfeita para shows e festas." },
    };

    [TextArea(2, 5)]
    public string falaDoFuncionario = "Ola! Bem-vindo a nossa loja de bonecas. Toque em qualquer boneca para ver o preco, o estoque e para que ela serve. Posso ajudar a escolher o presente ideal!";

    void Awake()
    {
        GameObject loja = GameObject.Find(nomeDaLoja);
        if (loja == null)
        {
            Debug.LogWarning("AutoProdutos: objeto '" + nomeDaLoja + "' nao encontrado na cena.");
            return;
        }

        int i = 0;
        bool achouFuncionario = false;

        // GetComponentsInChildren devolve o pai antes dos filhos.
        foreach (Transform t in loja.GetComponentsInChildren<Transform>(true))
        {
            if (t == loja.transform) continue;
            if (t.GetComponentInParent<ProductInfo>() != null) continue; // ja tem dados (a mao ou no pai)

            string n = t.name.ToLowerInvariant();

            if (n.Contains("funcion"))
            {
                var f = t.gameObject.AddComponent<ProductInfo>();
                f.nome = "Atendente da loja";
                f.isFuncionario = true;
                f.descricao = falaDoFuncionario;
                achouFuncionario = true;
            }
            else if (n.StartsWith("boneca"))
            {
                Dado d = catalogo[i % catalogo.Length];
                var p = t.gameObject.AddComponent<ProductInfo>();
                p.nome = d.nome;
                p.preco = moeda + " " + d.preco.ToString("0.00");
                p.estoque = d.estoque;
                p.descricao = d.descricao;
                i++;
            }
        }

        Debug.Log("AutoProdutos: " + i + " bonecas configuradas. Funcionario encontrado: " + achouFuncionario);
    }
}
