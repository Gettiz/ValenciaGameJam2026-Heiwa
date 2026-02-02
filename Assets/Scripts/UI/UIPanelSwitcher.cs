using UnityEngine;

public class UIPanelSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject[] panels;

    public void ShowPanel(GameObject panel)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i] != null)
            {
                panels[i].SetActive(panels[i] == panel);
            }
        }
    }

    public void HidePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }
}
