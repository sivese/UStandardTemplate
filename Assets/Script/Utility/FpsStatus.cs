using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsStatus : MonoBehaviour //Frames per second
{
    private const float TIME_SCALE = 1000.0f;

    [Range(10, 150), SerializeField]
    private int fontSize = 45;

    [SerializeField] private Color color = new Color(.0f, .0f, .0f, 1.0f);
    [SerializeField] float width, height;

    private void OnGUI()
    {
        var position = new Rect(width, height, Screen.width, Screen.height);

        var fps = 1.0f / Time.deltaTime;
        var ms = Time.deltaTime * TIME_SCALE;

        var text = string.Format("{0:N1} FPS ({1:N1}ms)", fps, ms);
        var style = new GUIStyle();

        style.fontSize = fontSize;
        style.normal.textColor = color;

        GUI.Label(position, text, style);
    }
}