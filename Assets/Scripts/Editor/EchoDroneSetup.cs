using UnityEngine;
using UnityEditor;

public static class EchoDroneSetup
{
    [MenuItem("Echo/Create Echo Drone Prototype")]
    public static void CreateEchoDrone()
    {
        Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
        if (litShader == null)
        {
            Debug.LogError("Could not find Universal Render Pipeline/Lit shader!");
            return;
        }

        // 1. Create Materials
        Material coreMat = GetOrCreateMaterial("Assets/Environment/Materials/M_Echo_Core.mat", litShader);
        coreMat.SetColor("_BaseColor", new Color(0.2f, 0.85f, 1f, 1f));
        coreMat.EnableKeyword("_EMISSION");
        coreMat.SetColor("_EmissionColor", new Color(0.2f, 0.85f, 1f, 1f) * 2.5f);
        coreMat.SetFloat("_Smoothness", 0.9f);
        coreMat.SetFloat("_Metallic", 0.2f);

        Material chassisMat = GetOrCreateMaterial("Assets/Environment/Materials/M_Echo_Chassis.mat", litShader);
        chassisMat.SetColor("_BaseColor", new Color(0.12f, 0.14f, 0.18f, 1f));
        chassisMat.SetFloat("_Metallic", 0.9f);
        chassisMat.SetFloat("_Smoothness", 0.85f);

        Material ringMat = GetOrCreateMaterial("Assets/Environment/Materials/M_Echo_Ring.mat", litShader);
        ringMat.SetColor("_BaseColor", new Color(0.6f, 0.65f, 0.75f, 1f));
        ringMat.SetFloat("_Metallic", 0.95f);
        ringMat.SetFloat("_Smoothness", 0.95f);

        AssetDatabase.SaveAssets();

        // 2. Build Drone Hierarchy
        // Check if Echo already exists in scene
        GameObject existingEcho = GameObject.Find("Echo_Companion");
        if (existingEcho != null)
        {
            Undo.DestroyObjectImmediate(existingEcho);
        }

        GameObject droneRoot = new GameObject("Echo_Companion");
        Undo.RegisterCreatedObjectUndo(droneRoot, "Create Echo Drone");

        // Visuals Container
        GameObject visuals = new GameObject("Visuals");
        visuals.transform.SetParent(droneRoot.transform, false);

        // Core Glowing Sphere
        GameObject core = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        core.name = "Glowing_Core";
        core.transform.SetParent(visuals.transform, false);
        core.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        RemoveCollider(core);
        core.GetComponent<Renderer>().sharedMaterial = coreMat;

        // Top Armor Shell
        GameObject topShell = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        topShell.name = "Top_Shell";
        topShell.transform.SetParent(visuals.transform, false);
        topShell.transform.localPosition = new Vector3(0f, 0.06f, 0f);
        topShell.transform.localScale = new Vector3(0.42f, 0.22f, 0.42f);
        RemoveCollider(topShell);
        topShell.GetComponent<Renderer>().sharedMaterial = chassisMat;

        // Stabilizer Ring (Equator)
        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = "Stabilizer_Ring";
        ring.transform.SetParent(visuals.transform, false);
        ring.transform.localScale = new Vector3(0.52f, 0.025f, 0.52f);
        RemoveCollider(ring);
        ring.GetComponent<Renderer>().sharedMaterial = ringMat;

        // Front Eye / Sensor Lens
        GameObject eye = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        eye.name = "Scanner_Eye";
        eye.transform.SetParent(visuals.transform, false);
        eye.transform.localPosition = new Vector3(0f, 0f, 0.16f);
        eye.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        eye.transform.localScale = new Vector3(0.12f, 0.06f, 0.12f);
        RemoveCollider(eye);
        eye.GetComponent<Renderer>().sharedMaterial = coreMat;

        // Bottom Thruster Nozzle
        GameObject thruster = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        thruster.name = "Bottom_Thruster";
        thruster.transform.SetParent(visuals.transform, false);
        thruster.transform.localPosition = new Vector3(0f, -0.12f, 0f);
        thruster.transform.localScale = new Vector3(0.18f, 0.06f, 0.18f);
        RemoveCollider(thruster);
        thruster.GetComponent<Renderer>().sharedMaterial = chassisMat;

        // Forward Spotlight (Tactical Flashlight)
        GameObject spotLightGo = new GameObject("Tactical_Flashlight");
        spotLightGo.transform.SetParent(droneRoot.transform, false);
        spotLightGo.transform.localPosition = new Vector3(0f, 0f, 0.15f);
        Light spot = spotLightGo.AddComponent<Light>();
        spot.type = LightType.Spot;
        spot.color = new Color(0.7f, 0.9f, 1f, 1f);
        spot.intensity = 3.5f;
        spot.range = 14f;
        spot.spotAngle = 55f;
        spot.innerSpotAngle = 35f;
        spot.shadows = LightShadows.Soft;

        // Core Aura Light (Soft Ambient Glow)
        GameObject auraLightGo = new GameObject("Aura_Light");
        auraLightGo.transform.SetParent(droneRoot.transform, false);
        Light aura = auraLightGo.AddComponent<Light>();
        aura.type = LightType.Point;
        aura.color = new Color(0.2f, 0.85f, 1f, 1f);
        aura.intensity = 1.2f;
        aura.range = 2.5f;
        aura.shadows = LightShadows.None;

        // 3. Attach EchoCompanion Script
        EchoCompanion companion = droneRoot.AddComponent<EchoCompanion>();
        companion.spotLight = spot;
        companion.auraLight = aura;
        companion.coreRenderer = core.GetComponent<Renderer>();

        // Auto-link Player if in scene
        PlayerMovement pm = Object.FindFirstObjectByType<PlayerMovement>();
        if (pm != null)
        {
            companion.playerTransform = pm.transform;
            if (pm.cameraTransform != null)
            {
                companion.cameraTransform = pm.cameraTransform;
            }
            droneRoot.transform.position = pm.transform.position + new Vector3(0.65f, 1.5f, -0.3f);
        }

        // 4. Save as Prefab
        System.IO.Directory.CreateDirectory("Assets/Prefabs");
        string prefabPath = "Assets/Prefabs/Echo_Drone.prefab";
        PrefabUtility.SaveAsPrefabAsset(droneRoot, prefabPath);
        Debug.Log($"Echo Drone created and saved as prefab to {prefabPath}!");

        Selection.activeGameObject = droneRoot;
    }

    private static Material GetOrCreateMaterial(string path, Shader shader)
    {
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, path);
        }
        return mat;
    }

    private static void RemoveCollider(GameObject go)
    {
        Collider col = go.GetComponent<Collider>();
        if (col != null)
        {
            Object.DestroyImmediate(col);
        }
    }
}
