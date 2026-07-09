using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SettingsGearButton : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(SettingsPanel.Open);
    }
}
