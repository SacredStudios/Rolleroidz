using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField] Texture2D img;
    [SerializeField] int size;
    [SerializeField] float maxAngle, minAngle;
    [SerializeField] Camera cam;


    float lookHeight;

    public void LookHeight(float value)
    {
        lookHeight += value;
        
        if (lookHeight > maxAngle || lookHeight < minAngle)
        {
            lookHeight -= value;
           
        }
    }
    private void OnGUI()
    {
        Vector3 screenPos = cam.WorldToScreenPoint(transform.position);
        screenPos.y =  screenPos.y- Screen.height;
        GUI.DrawTexture(new Rect(screenPos.x, screenPos.y/10 - lookHeight, size, size), img);
        Debug.Log(lookHeight);
    }
}
