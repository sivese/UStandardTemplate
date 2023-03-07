using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadMe : MonoBehaviour
{
    private void OnEnable()
    {
        
    }

    [SerializeField] private string description;
    [SerializeField] private GUIStyle guiStyle;

    private void OnGUI()
    {
        GUI.Box(new Rect(180, 180, 1450F, 800f), string.Empty);

        GUI.Label(new Rect(200, 200, 1400f, 100f), "Read Me", guiStyle);

        var smallStyle = new GUIStyle();
        smallStyle.fontSize = 28;
    
        GUI.Label (new Rect(200, 350, 1400f, 700f), description, smallStyle);
    }
}
