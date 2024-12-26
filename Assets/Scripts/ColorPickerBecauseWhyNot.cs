using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPickerBecauseWhyNot : MonoBehaviour
{
    [SerializeField] private SpriteRenderer rend; // Reference to the Image component
    [SerializeField] private Color myColor; // Reference to the red Slider component

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.color = myColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Method to change the color of the image
    [ContextMenu("Change Color")]
    public void ChangeColor()
    {
        rend.color = myColor;
    }
}