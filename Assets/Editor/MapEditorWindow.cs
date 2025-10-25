using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEditor.ShaderGraph.Internal;

public class EditorMapTool : EditorWindow
{
    //Gestion de prefabs (activo y lista)
    private List<GameObject> prefabs = new List<GameObject>();
    private string prefabFolder = "Assets/Game/Prefabs/mapAssets";
    private GameObject selectedPrefab;
    private GameObject previewObject;

    //Propiedades de grid (chatarra)
    private bool useGrid = false;
    private bool isIsometric = true;
    private float gridSize = 1f;

    //Controles del tansform
    private float depth = 0f;
    private Vector2 previewScale = Vector2.one;
    private float scaleIncrement = 0.1f;
    private float rotationIncrement = 15f;
    private float depthIncrement = 0.5f;
    private bool snapTransformations = true;

    //Controles de snapping
    private bool enableObjectSnapping = false;
    private float objectSnapDistance = 0.5f;
    private bool dynamicSnapDistance = true;
    private float snapDistanceMultiplier = 1.5f;
    private bool snapToCenter = true;
    private bool snapToEdges = true;
    private LayerMask snapLayers = -1;
    private bool showSnapGizmos = true;

    //Controles de visibilidad (transparencia de preview y toggle de objetos etiquetados)
    private bool toolEnabled = true;
    private bool areTaggedObjectsVisible = true;
    private readonly Dictionary<Material, Material> transparentMaterials = new Dictionary<Material, Material>();
    private const float PREVIEW_TRANSPARENCY = 0.5f;
    private const string HIDE_MAP_TAG = "HideMapTag";

    //Creación de ventana
    [MenuItem("Tools/Map Editor Tool")]
    public static void ShowWindow()
    {
        GetWindow<EditorMapTool>("Map Editor");
    }

    //Crea la lista de prefabs
    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        LoadPrefabs();
    }

    //Limpia objetos y materiales al cerrar la ventana
    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        if (previewObject != null)
            DestroyImmediate(previewObject);

        foreach (var mat in transparentMaterials.Values)
            if (mat != null) DestroyImmediate(mat);

        transparentMaterials.Clear();
    }

    //Saca los prefabs de la carpeta indicada
    private void LoadPrefabs()
    {
        prefabs.Clear();
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { prefabFolder });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null) prefabs.Add(prefab);
        }
    }

    //Pinta la ventana del editor
    private void OnGUI()
    {
        GUILayout.Label("Map Editor Tool", EditorStyles.boldLabel);

        //Enabke/Disable tool
        bool newToolEnabled = GUILayout.Toggle(toolEnabled, "Activar Editor");
        if (newToolEnabled != toolEnabled)
        {
            toolEnabled = newToolEnabled;
            if (!toolEnabled && previewObject != null)
            {
                DestroyImmediate(previewObject);
                selectedPrefab = null;
            }
            Repaint();
        }
        GUI.enabled = toolEnabled;

        //Seleccion de prefab
        GUILayout.Space(10);
        GUILayout.Label("Prefabs Disponibles:", EditorStyles.boldLabel);

        if (prefabs.Count == 0)
        {
            EditorGUILayout.HelpBox("No se encontraron prefabs en la carpeta especificada.", MessageType.Warning);
        }
        else
        {
            foreach (GameObject prefab in prefabs)
            {
                if (GUILayout.Button(prefab.name))
                {
                    selectedPrefab = prefab;
                    CreatePreviewObject();
                    Repaint();
                }
            }
        }

        if (selectedPrefab != null && GUILayout.Button("Deseleccionar Prefab"))
        {
            selectedPrefab = null;
            if (previewObject != null) DestroyImmediate(previewObject);
            Repaint();
        }

        //Controles de grid (chatarra)
        GUILayout.Space(10);
        GUILayout.Label("Opciones de grid", EditorStyles.boldLabel);
        useGrid = GUILayout.Toggle(useGrid, "Usar Grid");
        isIsometric = GUILayout.Toggle(isIsometric, "Grid Isométrico");
        gridSize = Mathf.Max(0.1f, EditorGUILayout.FloatField("Tamaño del Grid", gridSize));

        //Controles de snapping
        GUILayout.Space(10);
        GUILayout.Label("Opciones de snapping", EditorStyles.boldLabel);
        enableObjectSnapping = GUILayout.Toggle(enableObjectSnapping, "Activar snapping");

        if (enableObjectSnapping)
        {
            EditorGUI.indentLevel++;
            objectSnapDistance = EditorGUILayout.Slider("Distancia de snap base", objectSnapDistance, 0.1f, 5f);
            dynamicSnapDistance = GUILayout.Toggle(dynamicSnapDistance, "Distancia de snap dinámica(Scale-Based)");

            if (dynamicSnapDistance)
            {
                snapDistanceMultiplier = EditorGUILayout.Slider("Multiplicador de distancia", snapDistanceMultiplier, 0.5f, 3f);
                if (previewObject != null)
                    EditorGUILayout.LabelField($"Distancia actual: {GetDynamicSnapDistance():F2}");
            }

            snapToCenter = GUILayout.Toggle(snapToCenter, "Snap con centro");
            snapToEdges = GUILayout.Toggle(snapToEdges, "Snap con aristas");
            showSnapGizmos = GUILayout.Toggle(showSnapGizmos, "Mostrar gizmos");
            snapLayers = LayerMaskField("Capas de snap", snapLayers);
            EditorGUI.indentLevel--;
        }

        //Controles de transformacion
        GUILayout.Space(10);
        GUILayout.Label("Controles de Transformación", EditorStyles.boldLabel);
        snapTransformations = GUILayout.Toggle(snapTransformations, "Activar Snap en Transformaciones");
        scaleIncrement = EditorGUILayout.Slider("Incremento Escala", scaleIncrement, 0.05f, 1f);
        rotationIncrement = EditorGUILayout.Slider("Incremento Rotación", rotationIncrement, 1f, 45f);
        depthIncrement = EditorGUILayout.Slider("Incremento Profundidad", depthIncrement, 1f, 2f);

        if (previewObject != null)
        {
            EditorGUILayout.LabelField($"Escala: X={previewScale.x:F2}, Y={previewScale.y:F2}");
            EditorGUILayout.LabelField($"Rotación: {previewObject.transform.eulerAngles.z:F2}°");
            EditorGUILayout.LabelField($"Profundidad: {depth:F2}");
            EditorGUILayout.HelpBox("Alt + Rueda: Escala X | Ctrl + Rueda: Escala Y | Q/E: Rotación | Bloq Mayús + Rueda: Profundidad", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("Selecciona un prefab para ajustar transformaciones.", MessageType.Info);
        }

        GUILayout.Space(10);
        GUILayout.Label("Alineamiento de Posición", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("X")) AlignObjectsPosition(new Vector3(1, 0, 0));
        if (GUILayout.Button("Y")) AlignObjectsPosition(new Vector3(0, 1, 0));
        if (GUILayout.Button("Z")) AlignObjectsPosition(new Vector3(0, 0, 1));

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.Label("Alineamiento de Rotación", EditorStyles.boldLabel);
        if (GUILayout.Button("Z")) AlignObjectsRotationZ();

        GUILayout.Space(10);
        if (GUILayout.Button("Alternar Visibilidad"))
        {
            areTaggedObjectsVisible = !areTaggedObjectsVisible;
            GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(HIDE_MAP_TAG);

            foreach (GameObject obj in taggedObjects)
                if (obj != null)
                    foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
                        renderer.enabled = areTaggedObjectsVisible;

            SceneView.RepaintAll();
            Repaint();
        }

        GUI.enabled = true;
    }

    //Crea una máscara de capa para filtrar el snap
    private LayerMask LayerMaskField(string label, LayerMask layerMask)
    {
        List<string> layers = new List<string>();
        List<int> layerNumbers = new List<int>();

        for (int i = 0; i < 32; i++)
        {
            string layerName = LayerMask.LayerToName(i);
            if (!string.IsNullOrEmpty(layerName))
            {
                layers.Add(layerName);
                layerNumbers.Add(i);
            }
        }

        int maskWithoutEmpty = 0;
        for (int i = 0; i < layerNumbers.Count; i++)
            if (((1 << layerNumbers[i]) & layerMask.value) != 0)
                maskWithoutEmpty |= (1 << i);

        maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers.ToArray());

        int mask = 0;
        for (int i = 0; i < layerNumbers.Count; i++)
            if ((maskWithoutEmpty & (1 << i)) != 0)
                mask |= (1 << layerNumbers[i]);

        layerMask.value = mask;
        return layerMask;
    }

    //Funciones sobre la escena
    private void OnSceneGUI(SceneView sceneView)
    {
        if (!toolEnabled || selectedPrefab == null || previewObject == null) return;

        Event e = Event.current;
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        Plane plane = new Plane(Vector3.back, Vector3.zero);

        if (!plane.Raycast(ray, out float enter)) return;

        //Distancia del ratón al plano
        Vector3 mousePos = ray.GetPoint(enter);
        Vector3 targetPos = ApplyGrid(mousePos);

        if (enableObjectSnapping)
        {
            Vector3 snapPos = TrySnapToObjects(targetPos);
            if (snapPos != targetPos) targetPos = snapPos;
        }

        targetPos.z = depth;

        //Actualiza la posición del preview
        if (previewObject.transform.position != targetPos)
        {
            previewObject.transform.position = targetPos;
            SceneView.RepaintAll();
        }

        //Dibuja gizmo de snap
        if (enableObjectSnapping && showSnapGizmos)
        {
            Handles.color = new Color(0, 1, 0, 0.3f);
            Handles.DrawWireDisc(previewObject.transform.position, Vector3.forward, GetDynamicSnapDistance());
        }

        //Coloca el prefab al hacer click
        if (e.type == EventType.MouseDown && e.button == 0 && !e.alt && !e.control)
        {
            GameObject placed = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
            placed.transform.position = targetPos;
            placed.transform.rotation = previewObject.transform.rotation;
            placed.transform.localScale = new Vector3(previewScale.x, previewScale.y, 1f);
            placed.tag = HIDE_MAP_TAG;
            Undo.RegisterCreatedObjectUndo(placed, "Place Prefab");
            e.Use();
            Repaint();
        }

        //Gestiona la escala y profundidad con la rueda del ratón
        if (e.type == EventType.ScrollWheel && previewObject != null)
        {
            float scrollDir = -Mathf.Sign(e.delta.y);
            bool needsRepaint = false;

            if (e.alt && e.keyCode != KeyCode.Period) // Scale X
            {
                float change = snapTransformations ? scaleIncrement : 0.01f;
                previewScale.x = Mathf.Max(0.1f, previewScale.x + scrollDir * change);
                if (snapTransformations)
                    previewScale.x = Mathf.Round(previewScale.x / scaleIncrement) * scaleIncrement;
                previewObject.transform.localScale = new Vector3(previewScale.x, previewScale.y, 1f);
                needsRepaint = true;
            }
            else if (e.control) // Scale Y
            {
                float change = snapTransformations ? scaleIncrement : 0.01f;
                previewScale.y = Mathf.Max(0.1f, previewScale.y + scrollDir * change);
                if (snapTransformations)
                    previewScale.y = Mathf.Round(previewScale.y / scaleIncrement) * scaleIncrement;
                previewObject.transform.localScale = new Vector3(previewScale.x, previewScale.y, 1f);
                needsRepaint = true;
            }
            else if (e.capsLock) // Depth
            {
                float change = snapTransformations ? depthIncrement : 0.1f;
                depth = Mathf.Clamp(depth + scrollDir * change, -10f, 10f);
                if (snapTransformations)
                    depth = Mathf.Round(depth / depthIncrement) * depthIncrement;
                needsRepaint = true;
            }

            if (needsRepaint)
            {
                SceneView.RepaintAll();
                Repaint();
                e.Use();
            }
        }

        //Rotacion con Q/E
        if (e.type == EventType.KeyDown && e.delta == Vector2.zero)
        {
            bool needsRepaint = false;
            float rotationDelta = 0f;

            if (e.keyCode == KeyCode.Q)
                rotationDelta = snapTransformations ? rotationIncrement : 1f;
            else if (e.keyCode == KeyCode.E)
                rotationDelta = snapTransformations ? -rotationIncrement : -1f;

            if (rotationDelta != 0f)
            {
                previewObject.transform.Rotate(0, 0, rotationDelta);
                if (snapTransformations)
                {
                    Vector3 euler = previewObject.transform.eulerAngles;
                    euler.z = Mathf.Round(euler.z / rotationIncrement) * rotationIncrement;
                    previewObject.transform.eulerAngles = euler;
                }
                needsRepaint = true;
                e.Use();
            }

            if (needsRepaint)
            {
                SceneView.RepaintAll();
                Repaint();
            }
        }

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
    }

    //Calkula la distancia de snap dinámica basada en la escala del preview
    private float GetDynamicSnapDistance()
    {
        if (!dynamicSnapDistance || previewObject == null)
            return objectSnapDistance;

        float avgScale = (previewScale.x + previewScale.y) / 2f;
        return objectSnapDistance * avgScale * snapDistanceMultiplier;
    }

    //Busca objetos cercanos para snap y devuelve la posición ajustada
    private Vector3 TrySnapToObjects(Vector3 position)
    {
        if (!enableObjectSnapping) return position;

        float currentSnapDist = GetDynamicSnapDistance();
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(
            new Vector2(position.x, position.y),
            currentSnapDist,
            snapLayers
        );

        Vector3 closestSnapPoint = position;
        float closestDistance = float.MaxValue;
        bool foundSnapPoint = false;

        foreach (Collider2D col in nearbyColliders)
        {
            if (col.gameObject == previewObject) continue;

            //Snap con centro de objeto
            if (snapToCenter)
            {
                Vector3 snapPoint = col.transform.position;
                float distance = Vector2.Distance(new Vector2(position.x, position.y), new Vector2(snapPoint.x, snapPoint.y));

                if (distance < closestDistance && distance < currentSnapDist)
                {
                    closestDistance = distance;
                    closestSnapPoint = new Vector3(snapPoint.x, snapPoint.y, position.z);
                    foundSnapPoint = true;
                }
            }

            //Snap con aristas de objeto
            if (snapToEdges)
            {
                foreach (Vector3 edgePoint in GetEdgePoints(col))
                {
                    float distance = Vector2.Distance(new Vector2(position.x, position.y), new Vector2(edgePoint.x, edgePoint.y));

                    if (distance < closestDistance && distance < currentSnapDist)
                    {
                        closestDistance = distance;
                        closestSnapPoint = new Vector3(edgePoint.x, edgePoint.y, position.z);
                        foundSnapPoint = true;
                    }
                }
            }
        }

        //Ajuste de gizmos de snap
        if (foundSnapPoint && showSnapGizmos)
        {
            Handles.color = Color.green;
            Handles.DrawWireDisc(closestSnapPoint, Vector3.forward, 0.2f);
            Handles.DrawLine(position, closestSnapPoint);
        }

        return foundSnapPoint ? closestSnapPoint : position;
    }

    //Obtiene los vertices y puntos medios de las aristas del collider
    private Vector3[] GetEdgePoints(Collider2D col)
    {
        if (col is BoxCollider2D box)
        {
            Vector2 size = box.size;
            Vector3 localCenter = box.offset;
            List<Vector3> edgePoints = new List<Vector3>();

            //Toma los vertices locales (falsos)
            Vector3[] localVertices = new Vector3[]
            {
            new Vector3(-size.x / 2, -size.y / 2, 0), // Bottom-left
            new Vector3(size.x / 2, -size.y / 2, 0),  // Bottom-right
            new Vector3(-size.x / 2, size.y / 2, 0),  // Top-left
            new Vector3(size.x / 2, size.y / 2, 0)    // Top-right
            };

            //Transforma los vertices locales a coordenadas globales
            foreach (Vector3 localPoint in localVertices)
            {
                Vector3 worldPoint = col.transform.TransformPoint(localPoint + localCenter);
                edgePoints.Add(worldPoint);
            }

            //Desde los vertives globales calcula los puntos medios de las aristas
            edgePoints.Add(col.transform.TransformPoint(new Vector3(0, -size.y / 2, 0) + localCenter)); // Bottom
            edgePoints.Add(col.transform.TransformPoint(new Vector3(0, size.y / 2, 0) + localCenter));  // Top
            edgePoints.Add(col.transform.TransformPoint(new Vector3(-size.x / 2, 0, 0) + localCenter)); // Left
            edgePoints.Add(col.transform.TransformPoint(new Vector3(size.x / 2, 0, 0) + localCenter));  // Right

            return edgePoints.ToArray();
        }

        //Por si no fuera un box collider, usa los bounds
        Bounds bounds = col.bounds;
        return new Vector3[]
        {
        new Vector3(bounds.min.x, bounds.min.y, bounds.center.z),
        new Vector3(bounds.max.x, bounds.min.y, bounds.center.z),
        new Vector3(bounds.min.x, bounds.max.y, bounds.center.z),
        new Vector3(bounds.max.x, bounds.max.y, bounds.center.z),
        new Vector3(bounds.center.x, bounds.min.y, bounds.center.z),
        new Vector3(bounds.center.x, bounds.max.y, bounds.center.z),
        new Vector3(bounds.min.x, bounds.center.y, bounds.center.z),
        new Vector3(bounds.max.x, bounds.center.y, bounds.center.z)
        };
    }

    //applica el grid a una posición dada (chatarra)
    private Vector3 ApplyGrid(Vector3 position)
    {
        if (!useGrid) return position;

        if (isIsometric)
        {
            float isoX = (position.x - position.y) * 0.5f;
            float isoY = (position.x + position.y) * 0.25f;
            isoX = Mathf.Round(isoX / gridSize) * gridSize;
            isoY = Mathf.Round(isoY / gridSize) * gridSize;
            float cartX = isoX + 2f * isoY;
            float cartY = -isoX + 2f * isoY;
            return new Vector3(cartX, cartY, position.z);
        }

        float x = Mathf.Round(position.x / gridSize) * gridSize;
        float y = Mathf.Round(position.y / gridSize) * gridSize;
        return new Vector3(x, y, position.z);
    }

    public static void AlignObjectsPosition(Vector3 axisMask)
    {
        if (Selection.gameObjects.Length < 2)
        {
            Debug.LogWarning("Selecciona al menos dos objetos para alinear.");
            return;
        }

        // El primer objeto seleccionado es la referencia
        Transform reference = Selection.gameObjects[0].transform;
        Vector3 refPos = reference.position;

        foreach (GameObject obj in Selection.gameObjects)
        {
            if (obj == reference.gameObject) continue;

            Vector3 pos = obj.transform.position;

            // Solo copiamos las coordenadas que correspondan al eje (máscara 1 = copia, 0 = mantiene)
            pos.x = axisMask.x != 0 ? refPos.x : pos.x;
            pos.y = axisMask.y != 0 ? refPos.y : pos.y;
            pos.z = axisMask.z != 0 ? refPos.z : pos.z;

            Undo.RecordObject(obj.transform, "Align Position");
            obj.transform.position = pos;
        }
    }

    // Alinear rotaciones solo en Z
    public static void AlignObjectsRotationZ()
    {
        if (Selection.gameObjects.Length < 2)
            return;

        Transform reference = Selection.gameObjects[0].transform;
        float refRotZ = reference.eulerAngles.z;

        foreach (GameObject obj in Selection.gameObjects)
        {
            if (obj == reference.gameObject) continue;

            Vector3 rot = obj.transform.eulerAngles;
            rot.z = refRotZ;

            Undo.RecordObject(obj.transform, "Align Rotation Z");
            obj.transform.eulerAngles = rot;
        }
    }

    //Gestiona la creación del objeto preview
    private void CreatePreviewObject()
    {
        if (previewObject != null)
            DestroyImmediate(previewObject);

        if (selectedPrefab != null)
        {
            previewObject = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
            previewObject.name = selectedPrefab.name + "_Preview";
            previewObject.transform.localScale = new Vector3(previewScale.x, previewScale.y, 1f);
            SetTransparent(previewObject, PREVIEW_TRANSPARENCY);
            EditorUtility.SetDirty(previewObject);
            SceneView.RepaintAll();
            Repaint();
        }
    }

    //Vuelve transparente el objeto dado para la preview
    private void SetTransparent(GameObject obj, float alpha)
    {
        foreach (var r in obj.GetComponentsInChildren<Renderer>())
        {
            Material[] originalMaterials = r.sharedMaterials;
            Material[] newMaterials = new Material[originalMaterials.Length];

            for (int i = 0; i < originalMaterials.Length; i++)
            {
                Material mat = originalMaterials[i];
                if (mat == null || !mat.HasProperty("_Color"))
                {
                    newMaterials[i] = mat;
                    continue;
                }

                if (!transparentMaterials.TryGetValue(mat, out Material transparentMat))
                {
                    transparentMat = new Material(mat);
                    if (transparentMat.shader.name.Contains("Transparent") || transparentMat.HasProperty("_Mode"))
                    {
                        transparentMat.SetFloat("_Mode", 3);
                        transparentMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        transparentMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        transparentMat.SetInt("_ZWrite", 0);
                        transparentMat.DisableKeyword("_ALPHATEST_ON");
                        transparentMat.EnableKeyword("_ALPHABLEND_ON");
                        transparentMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        transparentMat.renderQueue = 3000;
                    }
                    transparentMaterials[mat] = transparentMat;
                }

                Color c = transparentMat.color;
                c.a = alpha;
                transparentMat.color = c;
                newMaterials[i] = transparentMat;
            }

            r.sharedMaterials = newMaterials;
        }
    }
}