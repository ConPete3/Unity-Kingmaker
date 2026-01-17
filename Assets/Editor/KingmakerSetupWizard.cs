using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class KingmakerSetupWizard : EditorWindow
{
    [MenuItem("Kingmaker/Setup Wizard")]
    public static void ShowWindow()
    {
        GetWindow<KingmakerSetupWizard>("Kingmaker Setup");
    }

    [MenuItem("Kingmaker/Run Full Setup")]
    public static void RunFullSetup()
    {
        CreateFolders();
        CreateSprites();
        CreateMaterials();
        CreateTileDataAssets();
        CreateHexGridConfig();
        CreatePrefabs();
        SetupScene();
        SaveCurrentScene();
        Debug.Log("Kingmaker setup completed successfully! Scene saved.");
    }

    [MenuItem("Kingmaker/Setup Scene Only")]
    public static void SetupSceneOnly()
    {
        SetupScene();
        SaveCurrentScene();
        Debug.Log("Scene setup completed and saved!");
    }

    private static void SaveCurrentScene()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private void OnGUI()
    {
        GUILayout.Label("Kingmaker Setup Wizard", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Run Full Setup to create all assets and configure the scene.\n" +
            "The GameManager will initialize the hex grid with 7 territories on Play.",
            MessageType.Info);

        GUILayout.Space(10);

        if (GUILayout.Button("Run Full Setup", GUILayout.Height(40)))
        {
            RunFullSetup();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Setup Scene Only (if assets exist)", GUILayout.Height(30)))
        {
            SetupSceneOnly();
        }

        GUILayout.Space(20);
        GUILayout.Label("Individual Steps:", EditorStyles.boldLabel);

        if (GUILayout.Button("1. Create Folders"))
            CreateFolders();
        if (GUILayout.Button("2. Create Sprites"))
            CreateSprites();
        if (GUILayout.Button("3. Create Materials"))
            CreateMaterials();
        if (GUILayout.Button("4. Create TileData Assets"))
            CreateTileDataAssets();
        if (GUILayout.Button("5. Create HexGridConfig"))
            CreateHexGridConfig();
        if (GUILayout.Button("6. Create Prefabs"))
            CreatePrefabs();
        if (GUILayout.Button("7. Setup Scene"))
        {
            SetupScene();
            SaveCurrentScene();
        }
    }

    private static void CreateFolders()
    {
        string[] folders = new string[]
        {
            "Assets/Sprites",
            "Assets/Materials",
            "Assets/Prefabs",
            "Assets/Data",
            "Assets/Data/TileData"
        };

        foreach (var folder in folders)
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                string parent = Path.GetDirectoryName(folder).Replace("\\", "/");
                string newFolder = Path.GetFileName(folder);
                AssetDatabase.CreateFolder(parent, newFolder);
            }
        }
        AssetDatabase.Refresh();
        Debug.Log("Folders created.");
    }

    private static void CreateSprites()
    {
        CreateHexSprite("Assets/Sprites/Hex_Base.png", 256, Color.white, false);
        CreateHexSprite("Assets/Sprites/Hex_Highlight.png", 256, new Color(1, 1, 1, 0.5f), false);
        CreateHexSprite("Assets/Sprites/Hex_Border.png", 256, Color.white, true);
        CreatePartyTokenSprite("Assets/Sprites/Party_Token.png", 128);

        AssetDatabase.Refresh();
        ConfigureSpriteImportSettings();
        Debug.Log("Sprites created.");
    }

    private static void CreateHexSprite(string path, int size, Color color, bool borderOnly)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];

        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.clear;

        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size * 0.45f;
        float borderWidth = size * 0.04f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                if (IsInsideHex(pos, center, radius))
                {
                    if (borderOnly)
                    {
                        if (!IsInsideHex(pos, center, radius - borderWidth))
                        {
                            pixels[y * size + x] = color;
                        }
                    }
                    else
                    {
                        pixels[y * size + x] = color;
                    }
                }
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();

        byte[] pngData = tex.EncodeToPNG();
        File.WriteAllBytes(path, pngData);
        Object.DestroyImmediate(tex);
    }

    private static bool IsInsideHex(Vector2 point, Vector2 center, float radius)
    {
        // Pointy-top hexagon
        float dx = Mathf.Abs(point.x - center.x);
        float dy = Mathf.Abs(point.y - center.y);

        float q2x = dx / radius;
        float q2y = dy / radius;

        // Pointy-top hex check
        return q2x <= 1f && q2y <= Mathf.Sqrt(3f) / 2f &&
               (Mathf.Sqrt(3f) * q2x + q2y) <= Mathf.Sqrt(3f);
    }

    private static void CreatePartyTokenSprite(string path, int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];

        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.clear;

        Vector2 center = new Vector2(size / 2f, size / 2f);
        float outerRadius = size * 0.4f;
        float innerRadius = size * 0.25f;

        // Draw a simple shield/marker shape
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                float dist = Vector2.Distance(pos, center);

                // Outer circle
                if (dist <= outerRadius && dist >= innerRadius)
                {
                    pixels[y * size + x] = Color.white;
                }
                // Inner diamond/marker
                else if (dist < innerRadius * 0.8f)
                {
                    float dx = Mathf.Abs(x - center.x);
                    float dy = Mathf.Abs(y - center.y);
                    if (dx + dy < innerRadius * 0.7f)
                    {
                        pixels[y * size + x] = Color.white;
                    }
                }
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();

        byte[] pngData = tex.EncodeToPNG();
        File.WriteAllBytes(path, pngData);
        Object.DestroyImmediate(tex);
    }

    private static void ConfigureSpriteImportSettings()
    {
        string[] spritePaths = new string[]
        {
            "Assets/Sprites/Hex_Base.png",
            "Assets/Sprites/Hex_Highlight.png",
            "Assets/Sprites/Hex_Border.png",
            "Assets/Sprites/Party_Token.png"
        };

        foreach (var path in spritePaths)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 256;
                importer.filterMode = FilterMode.Bilinear;
                importer.SaveAndReimport();
            }
        }
    }

    private static void CreateMaterials()
    {
        CreateMaterial("Mat_Capital", new Color(1f, 0.84f, 0f, 1f));
        CreateMaterial("Mat_Plains_North", new Color(0.6f, 0.8f, 0.4f, 1f));
        CreateMaterial("Mat_Forest_East", new Color(0.2f, 0.5f, 0.2f, 1f));
        CreateMaterial("Mat_Mountain_South", new Color(0.5f, 0.5f, 0.55f, 1f));
        CreateMaterial("Mat_River_West", new Color(0.3f, 0.5f, 0.8f, 1f));
        CreateMaterial("Mat_Village_NE", new Color(0.76f, 0.7f, 0.5f, 1f));
        CreateMaterial("Mat_Wilderness_SE", new Color(0.55f, 0.4f, 0.3f, 1f));

        AssetDatabase.Refresh();
        Debug.Log("Materials created.");
    }

    private static void CreateMaterial(string name, Color color)
    {
        string path = $"Assets/Materials/{name}.mat";
        if (File.Exists(path)) return;

        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = color;
        AssetDatabase.CreateAsset(mat, path);
    }

    private static void CreateTileDataAssets()
    {
        CreateTileData("TD_Capital_Calmafar", "Calmafar", TileType.Capital, new Color(1f, 0.84f, 0f, 1f), true, 1);
        CreateTileData("TD_Plains_North", "Northern Plains", TileType.Plains, new Color(0.6f, 0.8f, 0.4f, 1f), true, 1);
        CreateTileData("TD_Forest_East", "Eastern Forest", TileType.Forest, new Color(0.2f, 0.5f, 0.2f, 1f), true, 2);
        CreateTileData("TD_Mountain_South", "Southern Mountains", TileType.Mountain, new Color(0.5f, 0.5f, 0.55f, 1f), false, 99);
        CreateTileData("TD_River_West", "Western River", TileType.River, new Color(0.3f, 0.5f, 0.8f, 1f), true, 2);
        CreateTileData("TD_Village_NE", "Northeast Village", TileType.Village, new Color(0.76f, 0.7f, 0.5f, 1f), true, 1);
        CreateTileData("TD_Wilderness_SE", "Southeast Wilderness", TileType.Plains, new Color(0.55f, 0.4f, 0.3f, 1f), true, 1);

        AssetDatabase.Refresh();
        Debug.Log("TileData assets created.");
    }

    private static void CreateTileData(string fileName, string tileName, TileType type, Color color, bool passable, int cost)
    {
        string path = $"Assets/Data/TileData/{fileName}.asset";
        if (File.Exists(path)) return;

        TileData data = ScriptableObject.CreateInstance<TileData>();

        SerializedObject so = new SerializedObject(data);
        so.FindProperty("tileName").stringValue = tileName;
        so.FindProperty("tileType").enumValueIndex = (int)type;
        so.FindProperty("description").stringValue = $"The {tileName} territory.";
        so.FindProperty("tileColor").colorValue = color;
        so.FindProperty("isPassable").boolValue = passable;
        so.FindProperty("movementCost").intValue = cost;
        so.ApplyModifiedPropertiesWithoutUndo();

        AssetDatabase.CreateAsset(data, path);
    }

    private static void CreateHexGridConfig()
    {
        string path = "Assets/Data/HexGridConfig_Main.asset";
        if (File.Exists(path))
        {
            Debug.Log("HexGridConfig already exists.");
            return;
        }

        HexGridConfig config = ScriptableObject.CreateInstance<HexGridConfig>();

        SerializedObject so = new SerializedObject(config);
        so.FindProperty("radius").intValue = 1;
        so.FindProperty("hexSize").floatValue = 1f;
        so.ApplyModifiedPropertiesWithoutUndo();

        AssetDatabase.CreateAsset(config, path);
        AssetDatabase.Refresh();
        Debug.Log("HexGridConfig created.");
    }

    private static void CreatePrefabs()
    {
        CreateHexTilePrefab();
        CreatePartyTokenPrefab();
        AssetDatabase.Refresh();
        Debug.Log("Prefabs created.");
    }

    private static void CreateHexTilePrefab()
    {
        string prefabPath = "Assets/Prefabs/HexTile.prefab";
        if (File.Exists(prefabPath)) return;

        // Create root object
        GameObject hexRoot = new GameObject("HexTile");

        // Add main sprite renderer
        SpriteRenderer mainRenderer = hexRoot.AddComponent<SpriteRenderer>();
        Sprite hexSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Hex_Base.png");
        if (hexSprite != null)
            mainRenderer.sprite = hexSprite;
        mainRenderer.sortingOrder = 0;

        // Add collider
        PolygonCollider2D collider = hexRoot.AddComponent<PolygonCollider2D>();

        // Add HexTile component
        HexTile hexTile = hexRoot.AddComponent<HexTile>();

        // Create Highlight child
        GameObject highlight = new GameObject("Highlight");
        highlight.transform.SetParent(hexRoot.transform);
        highlight.transform.localPosition = Vector3.zero;
        SpriteRenderer highlightRenderer = highlight.AddComponent<SpriteRenderer>();
        Sprite highlightSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Hex_Highlight.png");
        if (highlightSprite != null)
            highlightRenderer.sprite = highlightSprite;
        highlightRenderer.sortingOrder = 1;
        highlightRenderer.enabled = false;

        // Create Border child
        GameObject border = new GameObject("Border");
        border.transform.SetParent(hexRoot.transform);
        border.transform.localPosition = Vector3.zero;
        SpriteRenderer borderRenderer = border.AddComponent<SpriteRenderer>();
        Sprite borderSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Hex_Border.png");
        if (borderSprite != null)
            borderRenderer.sprite = borderSprite;
        borderRenderer.sortingOrder = 2;

        // Wire up serialized fields
        SerializedObject so = new SerializedObject(hexTile);
        so.FindProperty("spriteRenderer").objectReferenceValue = mainRenderer;
        so.FindProperty("highlightRenderer").objectReferenceValue = highlightRenderer;
        so.FindProperty("borderRenderer").objectReferenceValue = borderRenderer;
        so.ApplyModifiedPropertiesWithoutUndo();

        // Save prefab
        PrefabUtility.SaveAsPrefabAsset(hexRoot, prefabPath);
        Object.DestroyImmediate(hexRoot);
    }

    private static void CreatePartyTokenPrefab()
    {
        string prefabPath = "Assets/Prefabs/PartyToken.prefab";
        if (File.Exists(prefabPath)) return;

        GameObject tokenRoot = new GameObject("PartyToken");

        SpriteRenderer renderer = tokenRoot.AddComponent<SpriteRenderer>();
        Sprite tokenSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Party_Token.png");
        if (tokenSprite != null)
            renderer.sprite = tokenSprite;
        renderer.sortingOrder = 10;
        renderer.color = new Color(0.2f, 0.6f, 1f, 1f); // Blue tint for visibility

        PartyController controller = tokenRoot.AddComponent<PartyController>();

        // Wire partyToken reference to self
        SerializedObject so = new SerializedObject(controller);
        so.FindProperty("partyToken").objectReferenceValue = tokenRoot.transform;
        so.ApplyModifiedPropertiesWithoutUndo();

        PrefabUtility.SaveAsPrefabAsset(tokenRoot, prefabPath);
        Object.DestroyImmediate(tokenRoot);
    }

    private static void SetupScene()
    {
        // Find or create main camera
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            mainCam = camObj.AddComponent<Camera>();
            mainCam.orthographic = true;
            mainCam.orthographicSize = 5;
            mainCam.backgroundColor = new Color(0.2f, 0.2f, 0.25f);
            camObj.transform.position = new Vector3(0, 0, -10);
        }

        // Add CameraController
        if (mainCam.GetComponent<CameraController>() == null)
        {
            mainCam.gameObject.AddComponent<CameraController>();
        }

        // Create GameManager
        GameObject gameManager = GameObject.Find("GameManager");
        if (gameManager == null)
        {
            gameManager = new GameObject("GameManager");
            gameManager.AddComponent<GameManager>();
        }

        // Create World container
        GameObject world = GameObject.Find("World");
        if (world == null)
        {
            world = new GameObject("World");
        }

        // Create HexGrid
        GameObject hexGrid = GameObject.Find("HexGrid");
        if (hexGrid == null)
        {
            hexGrid = new GameObject("HexGrid");
            hexGrid.transform.SetParent(world.transform);
            hexGrid.AddComponent<HexGridManager>();

            // Create GridContainer
            GameObject gridContainer = new GameObject("GridContainer");
            gridContainer.transform.SetParent(hexGrid.transform);
        }

        // Instantiate PartyToken
        GameObject partyToken = GameObject.Find("PartyToken");
        if (partyToken == null)
        {
            GameObject tokenPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/PartyToken.prefab");
            if (tokenPrefab != null)
            {
                partyToken = (GameObject)PrefabUtility.InstantiatePrefab(tokenPrefab);
                partyToken.name = "PartyToken";
                partyToken.transform.SetParent(world.transform);
            }
        }

        // Create Canvas for UI
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            canvas = new GameObject("Canvas");
            Canvas canvasComp = canvas.AddComponent<Canvas>();
            canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            canvas.AddComponent<ScreenManager>();

            // Create screen containers
            CreateScreenContainer(canvas.transform, "GlobalMapScreen");
            CreateScreenContainer(canvas.transform, "TileViewScreen");
            CreateScreenContainer(canvas.transform, "CapitalViewScreen");
        }

        // Create EventSystem if needed
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // Wire references
        WireReferences();

        Debug.Log("Scene setup completed.");
    }

    private static void CreateScreenContainer(Transform parent, string name)
    {
        GameObject screen = new GameObject(name);
        screen.transform.SetParent(parent);
        RectTransform rt = screen.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // Add the screen component and set screen type
        ScreenBase screenBase = null;
        switch (name)
        {
            case "GlobalMapScreen":
                screenBase = screen.AddComponent<GlobalMapScreen>();
                SetScreenType(screenBase, GameScreen.GlobalMap);
                break;
            case "TileViewScreen":
                screenBase = screen.AddComponent<TileViewScreen>();
                SetScreenType(screenBase, GameScreen.TileView);
                screen.SetActive(false);
                break;
            case "CapitalViewScreen":
                screenBase = screen.AddComponent<CapitalViewScreen>();
                SetScreenType(screenBase, GameScreen.CapitalView);
                screen.SetActive(false);
                break;
        }
    }

    private static void SetScreenType(ScreenBase screen, GameScreen type)
    {
        SerializedObject so = new SerializedObject(screen);
        so.FindProperty("screenType").enumValueIndex = (int)type;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void WireReferences()
    {
        // Load assets
        HexGridConfig config = AssetDatabase.LoadAssetAtPath<HexGridConfig>("Assets/Data/HexGridConfig_Main.asset");
        GameObject hexTilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/HexTile.prefab");
        TileData capitalTileData = AssetDatabase.LoadAssetAtPath<TileData>("Assets/Data/TileData/TD_Capital_Calmafar.asset");
        TileData defaultTileData = AssetDatabase.LoadAssetAtPath<TileData>("Assets/Data/TileData/TD_Plains_North.asset");

        // Wire HexGridConfig
        if (config != null)
        {
            // Load all territory TileData assets
            TileData plainsNorthData = AssetDatabase.LoadAssetAtPath<TileData>("Assets/Data/TileData/TD_Plains_North.asset");
            TileData forestEastData = AssetDatabase.LoadAssetAtPath<TileData>("Assets/Data/TileData/TD_Forest_East.asset");
            TileData villageNEData = AssetDatabase.LoadAssetAtPath<TileData>("Assets/Data/TileData/TD_Village_NE.asset");
            TileData riverWestData = AssetDatabase.LoadAssetAtPath<TileData>("Assets/Data/TileData/TD_River_West.asset");
            TileData mountainSouthData = AssetDatabase.LoadAssetAtPath<TileData>("Assets/Data/TileData/TD_Mountain_South.asset");
            TileData wildernessSEData = AssetDatabase.LoadAssetAtPath<TileData>("Assets/Data/TileData/TD_Wilderness_SE.asset");

            SerializedObject configSO = new SerializedObject(config);
            configSO.FindProperty("hexTilePrefab").objectReferenceValue = hexTilePrefab;
            configSO.FindProperty("capitalTileData").objectReferenceValue = capitalTileData;
            configSO.FindProperty("defaultTileData").objectReferenceValue = defaultTileData;
            configSO.FindProperty("plainsNorthData").objectReferenceValue = plainsNorthData;
            configSO.FindProperty("forestEastData").objectReferenceValue = forestEastData;
            configSO.FindProperty("villageNEData").objectReferenceValue = villageNEData;
            configSO.FindProperty("riverWestData").objectReferenceValue = riverWestData;
            configSO.FindProperty("mountainSouthData").objectReferenceValue = mountainSouthData;
            configSO.FindProperty("wildernessSEData").objectReferenceValue = wildernessSEData;
            configSO.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(config);
        }

        // Wire scene objects
        GameObject gameManager = GameObject.Find("GameManager");
        GameObject hexGrid = GameObject.Find("HexGrid");
        GameObject partyToken = GameObject.Find("PartyToken");
        GameObject canvas = GameObject.Find("Canvas");
        GameObject gridContainer = GameObject.Find("GridContainer");
        Camera mainCam = Camera.main;

        // Wire GameManager
        if (gameManager != null)
        {
            GameManager gm = gameManager.GetComponent<GameManager>();
            if (gm != null)
            {
                SerializedObject gmSO = new SerializedObject(gm);
                if (hexGrid != null)
                    gmSO.FindProperty("hexGridManager").objectReferenceValue = hexGrid.GetComponent<HexGridManager>();
                if (partyToken != null)
                    gmSO.FindProperty("partyController").objectReferenceValue = partyToken.GetComponent<PartyController>();
                if (canvas != null)
                    gmSO.FindProperty("screenManager").objectReferenceValue = canvas.GetComponent<ScreenManager>();
                gmSO.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        // Wire HexGridManager
        if (hexGrid != null)
        {
            HexGridManager hgm = hexGrid.GetComponent<HexGridManager>();
            if (hgm != null)
            {
                SerializedObject hgmSO = new SerializedObject(hgm);
                hgmSO.FindProperty("config").objectReferenceValue = config;
                if (gridContainer != null)
                    hgmSO.FindProperty("gridContainer").objectReferenceValue = gridContainer.transform;
                hgmSO.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        // Wire PartyController
        if (partyToken != null)
        {
            PartyController pc = partyToken.GetComponent<PartyController>();
            if (pc != null && hexGrid != null)
            {
                SerializedObject pcSO = new SerializedObject(pc);
                pcSO.FindProperty("hexGridManager").objectReferenceValue = hexGrid.GetComponent<HexGridManager>();
                pcSO.FindProperty("partyToken").objectReferenceValue = partyToken.transform;
                pcSO.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        // Wire CameraController
        if (mainCam != null)
        {
            CameraController cc = mainCam.GetComponent<CameraController>();
            if (cc != null && partyToken != null)
            {
                SerializedObject ccSO = new SerializedObject(cc);
                ccSO.FindProperty("target").objectReferenceValue = partyToken.transform;
                ccSO.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        // Wire ScreenManager's screens list
        if (canvas != null)
        {
            ScreenManager sm = canvas.GetComponent<ScreenManager>();
            if (sm != null)
            {
                SerializedObject smSO = new SerializedObject(sm);
                SerializedProperty screensProp = smSO.FindProperty("screens");
                screensProp.ClearArray();

                GlobalMapScreen globalMapScreen = canvas.GetComponentInChildren<GlobalMapScreen>(true);
                TileViewScreen tileViewScreen = canvas.GetComponentInChildren<TileViewScreen>(true);
                CapitalViewScreen capitalViewScreen = canvas.GetComponentInChildren<CapitalViewScreen>(true);

                int index = 0;
                if (globalMapScreen != null)
                {
                    screensProp.InsertArrayElementAtIndex(index);
                    screensProp.GetArrayElementAtIndex(index).objectReferenceValue = globalMapScreen;
                    index++;
                }
                if (tileViewScreen != null)
                {
                    screensProp.InsertArrayElementAtIndex(index);
                    screensProp.GetArrayElementAtIndex(index).objectReferenceValue = tileViewScreen;
                    index++;
                }
                if (capitalViewScreen != null)
                {
                    screensProp.InsertArrayElementAtIndex(index);
                    screensProp.GetArrayElementAtIndex(index).objectReferenceValue = capitalViewScreen;
                    index++;
                }

                smSO.ApplyModifiedPropertiesWithoutUndo();

                // Wire GlobalMapScreen references
                if (globalMapScreen != null && hexGrid != null && partyToken != null)
                {
                    SerializedObject gmsSO = new SerializedObject(globalMapScreen);
                    gmsSO.FindProperty("hexGridManager").objectReferenceValue = hexGrid.GetComponent<HexGridManager>();
                    gmsSO.FindProperty("partyController").objectReferenceValue = partyToken.GetComponent<PartyController>();
                    gmsSO.ApplyModifiedPropertiesWithoutUndo();
                }
            }
        }

        AssetDatabase.SaveAssets();
    }
}
