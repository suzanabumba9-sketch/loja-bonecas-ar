using UnityEngine;

// Coloque este script em CADA boneca (e no Funcionario).
// Preencha os campos no Inspector do Unity.
public class ProductInfo : MonoBehaviour
{
    public string nome = "Boneca";
    public string preco = "R$ 0,00";
    public int estoque = 0;
    [TextArea(2, 6)] public string descricao = "Informacoes sobre o produto.";

    [Tooltip("Marque no Funcionario: ele mostra uma fala em vez de preco e estoque.")]
    public bool isFuncionario = false;
}
