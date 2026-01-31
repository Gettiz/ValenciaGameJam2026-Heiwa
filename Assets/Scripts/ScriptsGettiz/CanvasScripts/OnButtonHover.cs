using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OnButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI label;
    
    [Header("Vertex Color Settings")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.pink;

    private bool isHovered = false;

    private void Start()
    {
        label = GetComponent<TextMeshProUGUI>();
        
        if (label != null)
            label.color = normalColor;
    }

    private void Update()
    {
        if (label == null) return;

        Color targetColor;
        if (isHovered)
            targetColor = hoverColor;
        else
            targetColor = normalColor;
        label.color = targetColor;
    }

    // These MUST be public to work with the Event System
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }
}