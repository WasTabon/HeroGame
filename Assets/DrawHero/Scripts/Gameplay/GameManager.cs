using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float defaultInk = 100f;

    public bool CanInteract { get; private set; } = true;

    private List<GameObject> drawnObjects = new List<GameObject>();

    private void Start()
    {
        if (InkManager.Instance != null)
            InkManager.Instance.SetMaxInk(defaultInk);
    }

    public void RegisterDrawnObject(GameObject obj)
    {
        drawnObjects.Add(obj);
    }

    public void ClearDrawnObjects()
    {
        for (int i = 0; i < drawnObjects.Count; i++)
        {
            if (drawnObjects[i] != null)
                Destroy(drawnObjects[i]);
        }
        drawnObjects.Clear();
    }

    public void RestartLevel()
    {
        ClearDrawnObjects();

        if (InkManager.Instance != null)
            InkManager.Instance.ResetInk();
    }

    public void SetInteractable(bool value)
    {
        CanInteract = value;
    }
}
