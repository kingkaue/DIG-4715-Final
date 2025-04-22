#if UNITY_EDITOR
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;

public class SaveNavMesh
{
    [MenuItem("Tools/Save NavMesh To Asset")]
    public static void SaveNavMeshAsset() // Renamed method
    {
        var surface = GameObject.FindObjectOfType<NavMeshSurface>();
        if (surface == null)
        {
            Debug.LogError("No NavMeshSurface found!");
            return;
        }

        var data = surface.navMeshData;
        if (data == null)
        {
            Debug.LogError("No NavMeshData found! Bake the NavMesh first.");
            return;
        }

        // Create a new instance of the NavMeshData to avoid scene-linking issues
        var newData = Object.Instantiate(data);

        var path = "Assets/NavMesh.asset";
        AssetDatabase.CreateAsset(newData, path);
        AssetDatabase.SaveAssets();
        Debug.Log("NavMesh saved to " + path);
    }
}
#endif
