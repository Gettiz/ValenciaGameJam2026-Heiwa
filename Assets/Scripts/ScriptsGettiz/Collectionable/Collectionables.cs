using System;
using TMPro;
using UnityEngine;

public class Collectionables : MonoBehaviour
{
    public GameObject[] prefabs;
    public int currentCount = 0;
    public int totalCount;
    
    public TextMeshProUGUI text;

    private void Start()
    {
        totalCount = prefabs.Length;
        Debug.Log($"Inventory initialized: 0/{totalCount}");
        text.text = $"{currentCount}/{totalCount}";
    }
    
    public void AddItem()
    {
        currentCount++;
        
        Debug.Log($"{currentCount}/{totalCount}");

        if (currentCount >= totalCount)
        {
            Debug.Log("All items collected!");
        }
        
        text.text = $"{currentCount}/{totalCount}";
    }
}
