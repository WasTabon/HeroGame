using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SetupIteration6
{
    [MenuItem("Tools/DrawHero/Setup Iteration 6 (IAP + PowerUps)")]
    public static void SetupAll()
    {
        string scenePath = "Assets/DrawHero/Scenes/Game.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        if (GameObject.Find("PowerUpBar") == null)
        {
            GameObject bar = new GameObject("PowerUpBar");
            bar.AddComponent<PowerUpBar>();
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("DrawHero Iteration 6: PowerUpBar added to Game scene. IAPManager auto-bootstraps. Configure IAP in Services window + Product ID '" + IAPManager.POWER_PACK_ID + "'.");
    }
}
