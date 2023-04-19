#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(MeshFilter))]
[CanEditMultipleObjects]
public class VisualizeVertexLength : UnityEditor.Editor
{
    [SerializeField] private MeshFilter[] _selectedMeshes = new MeshFilter[2];
    static bool _toggle = false;

    void DrawPoint()
    {
        if (_selectedMeshes == null) return;
        if (_selectedMeshes[0] == null)
        {
            _selectedMeshes = null;
            return;
        }

        if (_selectedMeshes[0].sharedMesh != _selectedMeshes[1].sharedMesh)
            return;

        Color[] colorSet = new[] { Color.green, Color.yellow, Color.red, Color.magenta };
        for (int i = 0; i < _selectedMeshes[0].sharedMesh.vertexCount; i++)
        {
            var curColor = colorSet[i % colorSet.Length];
            Handles.color = curColor;
            var vertexA = _selectedMeshes[0].sharedMesh.vertices[i];
            var vertexB = _selectedMeshes[1].sharedMesh.vertices[i];
            var pointA = _selectedMeshes[0].transform.localToWorldMatrix.MultiplyPoint(vertexA);
            var pointB = _selectedMeshes[1].transform.localToWorldMatrix.MultiplyPoint(vertexB);


            Handles.DrawWireCube(pointA, Vector3.one * .03f);
            Handles.DrawWireCube(pointB, Vector3.one * .03f);
            Handles.DrawDottedLine(pointA, pointB, 4);
            Handles.Label(Vector3.Lerp(pointA, pointB, 0.5f), Vector3.Distance(pointA, pointB).ToString("0.000"));
        }
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        bool prev = _toggle;
        _toggle = GUILayout.Toggle(EditorPrefs.GetBool("VertexLineDrawer"), "Enable Vertex Drawer");
        if (prev != _toggle)
        {
            EditorPrefs.SetBool("VertexLineDrawer", _toggle);
            SceneView.RepaintAll();
        }
    }
    // Start is called before the first frame update

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnCustomSceneGUI;
        Selection.selectionChanged += OnChangeSelectedObject;
    }

    private void OnDisable()
    {
        _selectedMeshes = null;
        SceneView.duringSceneGui -= OnCustomSceneGUI;
        Selection.selectionChanged -= OnChangeSelectedObject;
    }


    void OnChangeSelectedObject()
    {
        _selectedMeshes = null;
        if (Selection.gameObjects.Length == 2)
        {
            var filters = Selection.gameObjects.Select((x => x.GetComponent<MeshFilter>()));
            MeshFilter[] meshFilters = filters.ToArray();
            if (meshFilters.Length == 2)
            {
                _selectedMeshes = meshFilters;
            }
        }
    }

    private void OnDestroy()
    {
        _selectedMeshes = null;
    }

    void OnCustomSceneGUI(SceneView view)
    {
        if (_toggle)
        {
            if (_selectedMeshes != null && _selectedMeshes.Length == 2)
            {
                DrawPoint();
            }
        }
    }
}
#endif
