using System.Collections.Generic;
using UnityEngine;

public class RandomizeCharacter : MonoBehaviour
{
    public List<CharacterOption> CharacterOptions;

    [Tooltip("Lista de SkinnedMeshRenderers a los que se aplicarán colores aleatorios")]
    public List<SkinnedMeshRenderer> renderers;

    [Tooltip("Lista de SkinnedMeshRenderers a los que se aplicará una textura aleatoria")]
    public List<SkinnedMeshRenderer> bodyrenderers;

    [Tooltip("Lista de Texturas posibles para asignar")]
    public List<Texture> textures;

    // Paleta fija de 16 colores
    private static readonly Color[] limitedPalette = new Color[]
    {
        Color.red, Color.green, Color.blue,       // Primarios
        Color.yellow, Color.magenta, Color.cyan, // Secundarios
        new Color(1f, 0.5f, 0f),   // Naranja
        new Color(0.5f, 0f, 1f),   // Violeta
        new Color(0f, 1f, 0.5f),   // Verde lima
        new Color(1f, 1f, 0.5f),   // Amarillo claro
        Color.white, Color.black, Color.gray,    // Neutros
        new Color(0.5f, 0.25f, 0f), // Marrón
        new Color(0.75f, 0.75f, 1f), // Azul claro
        new Color(1f, 0.75f, 0.8f)   // Rosado
    };

    public string currentCode;

    private void OnEnable()
    {
        GenerateRandomCharacter();
    }

    [ContextMenu("Randomize")]
    /// <summary>
    /// Genera un personaje aleatorio y crea un código corto que lo representa.
    /// </summary>
    public void GenerateRandomCharacter()
    {
        string code = "";

        // Opciones de malla
        foreach (var option in CharacterOptions)
        {
            // Apagamos todo
            foreach (var variantCollection in option.variants)
                foreach (var variant in variantCollection.variants)
                    variant.SetActive(false);

            // Elegimos una colección
            int collectionIndex = Random.Range(0, option.variants.Length);
            var randomVariantCollection = option.variants[collectionIndex];

            // Encendemos
            foreach (var variant in randomVariantCollection.variants)
                variant.SetActive(true);

            // Añadimos al código en base36 (0-9,A-Z → 36 símbolos, eficiente)
            code += EncodeValue(collectionIndex, 36);
        }

        // Colores
        foreach (var smr in renderers)
        {
            if (smr == null) continue;

            int rootIndex = Random.Range(0, limitedPalette.Length);
            int tipIndex = Random.Range(0, limitedPalette.Length);

            Material mat = smr.material;
            mat.SetColor("_Root_Color", limitedPalette[rootIndex]);
            mat.SetColor("_Tip_Color", limitedPalette[tipIndex]);

            code += EncodeValue(rootIndex, 16); // 0–F
            code += EncodeValue(tipIndex, 16);
        }

        // Texturas
        foreach (var smr in bodyrenderers)
        {
            if (smr == null || textures.Count == 0) continue;

            int texIndex = Random.Range(0, textures.Count);
            Texture randomTex = textures[texIndex];

            smr.material.SetTexture("_BaseColor_Map", randomTex);
            code += EncodeValue(texIndex, 36);
        }

        currentCode = code;
        Debug.Log("Generated Code: " + code);
    }

    public string TestingCode;
    [ContextMenu("Apply Testing Code")]
    public void TestDecode()
    {
        ApplyCharacterFromCode(TestingCode);
    }

    /// <summary>
    /// Replica un personaje a partir de un código.
    /// </summary>
    public void ApplyCharacterFromCode(string code)
    {
        int index = 0;

        // Opciones de malla
        foreach (var option in CharacterOptions)
        {
            foreach (var variantCollection in option.variants)
                foreach (var variant in variantCollection.variants)
                    variant.SetActive(false);

            int collectionIndex = DecodeValue(code[index++].ToString());
            var selectedCollection = option.variants[Mathf.Clamp(collectionIndex, 0, option.variants.Length - 1)];

            foreach (var variant in selectedCollection.variants)
                variant.SetActive(true);
        }

        // Colores
        foreach (var smr in renderers)
        {
            if (smr == null) continue;

            int rootIndex = DecodeValue(code[index++].ToString());
            int tipIndex = DecodeValue(code[index++].ToString());

            Material mat = smr.material;
            mat.SetColor("_Root_Color", limitedPalette[rootIndex % limitedPalette.Length]);
            mat.SetColor("_Tip_Color", limitedPalette[tipIndex % limitedPalette.Length]);
        }

        // Texturas
        foreach (var smr in bodyrenderers)
        {
            if (smr == null || textures.Count == 0) continue;

            int texIndex = DecodeValue(code[index++].ToString());
            Texture tex = textures[texIndex % textures.Count];
            smr.material.SetTexture("_BaseColor_Map", tex);
        }
    }

    /// Codificación simple en base36
    private string EncodeValue(int value, int baseN)
    {
        const string symbols = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        return symbols[Mathf.Clamp(value, 0, baseN - 1)].ToString();
    }

    private int DecodeValue(string symbol)
    {
        const string symbols = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        return symbols.IndexOf(symbol);
    }

    [System.Serializable]
    public class CharacterOption
    {
        public string optionName;
        public VariantColection[] variants;
    }

    [System.Serializable]
    public class VariantColection
    {
        public string optionName;
        public List<GameObject> variants;
    }
}
