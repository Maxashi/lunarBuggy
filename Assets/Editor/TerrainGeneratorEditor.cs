using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    TerrainGenerator terrainGenerator;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();


        if(GUILayout.Button("Generate Mesh"))
        {
            terrainGenerator.GenerateHeightmap();
            terrainGenerator.ConstructMesh();
        }
    }
    void OnEnable()
    {
        terrainGenerator = (TerrainGenerator)target;
        Tools.hidden = true;
    }

    void OnDisable()
    {
        Tools.hidden = false;
    }

}
