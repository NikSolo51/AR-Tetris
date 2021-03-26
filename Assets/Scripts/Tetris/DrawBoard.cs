using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using Image = UnityEngine.UI.Image;

public class DrawBoard : MonoBehaviour
{
    public static UnityEngine.UI.Image[,] images;
    
    private void Awake()
    {
        images = new UnityEngine.UI.Image[20,10];
        
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                images[i, j] = transform.GetChild(j + i * 10).GetComponent<Image>();
            }
        }
        
    }
}
