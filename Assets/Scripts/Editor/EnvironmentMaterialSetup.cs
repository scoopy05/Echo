using UnityEngine;
using UnityEditor;

public static class EnvironmentMaterialSetup
{
    [MenuItem("Echo/Setup Sci-Fi Materials")]
    public static void CreateMaterials()
    {
        Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
        if (litShader == null)
        {
            Debug.LogError("Could not find Universal Render Pipeline/Lit shader!");
            return;
        }

        // Setup MI_Trim_01 (contains glowing cyan emissive lights)
        CreateTrimMaterial("MI_Trim_01", "T_Trim_01_BaseColor", "T_Trim_01_Normal", "T_Trim_01_Emissive", litShader);

        // Setup MI_Trim_02
        CreateTrimMaterial("MI_Trim_02", "T_Trim_02_BaseColor", "T_Trim_02_Normal", null, litShader);

        // Setup MI_Trim_03
        CreateTrimMaterial("MI_Trim_03", "T_Trim_03_BaseColor", "T_Trim_03_Normal", null, litShader);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Sci-Fi Materials created successfully in Assets/Environment/Materials!");
    }

    private static void CreateTrimMaterial(string matName, string baseColorName, string normalMapName, string emissiveName, Shader shader)
    {
        string path = $"Assets/Environment/Materials/{matName}.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, path);
        }
        else
        {
            mat.shader = shader;
        }

        Texture2D baseColor = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/Environment/Textures/{baseColorName}.png");
        if (baseColor != null)
        {
            mat.SetTexture("_BaseMap", baseColor);
        }

        Texture2D normalMap = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/Environment/Textures/{normalMapName}.png");
        if (normalMap != null)
        {
            mat.SetTexture("_BumpMap", normalMap);
            mat.EnableKeyword("_NORMALMAP");
        }

        if (!string.IsNullOrEmpty(emissiveName))
        {
            Texture2D emissive = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/Environment/Textures/{emissiveName}.png");
            if (emissive != null)
            {
                mat.SetTexture("_EmissionMap", emissive);
                mat.SetColor("_EmissionColor", Color.white * 2.5f); // 2.5x HDR intensity for strong sci-fi glow
                mat.EnableKeyword("_EMISSION");
            }
        }

        EditorUtility.SetDirty(mat);
    }
}
