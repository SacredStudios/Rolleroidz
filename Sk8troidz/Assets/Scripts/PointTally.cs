using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTally : MonoBehaviour
{
    public int points;
    void Start()
    {
        points = 0;  
    }
    public void ChangePoints(int x)
    {
        points += x;
        if (points < 0) points = 0;
    }
  
}
