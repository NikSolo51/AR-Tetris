using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndAnim : MonoBehaviour
{
    private GameObject[] objects;
    private GameObject[] duplicates;
    public GameObject prefab;
    public GameObject origin;
    private float time = 0.4f;
    private Renderer _renderer;
    
    
    private void OnEnable()
    {
        Board.OnGameOver += CreateGameOver;
    }

    private void OnDisable()
    {
        Board.OnGameOver -= CreateGameOver;
    }
    

    private void Start()
    {
        objects = new GameObject[transform.childCount];
        duplicates = new GameObject[transform.childCount];
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i] = transform.GetChild(i).gameObject;
        }
    }
    
    public void CreateGameOver()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            duplicates[i] = Instantiate(prefab, origin.transform);
            duplicates[i].transform.localScale = new Vector3(1f, 1f, 1f);
            
            _renderer = duplicates[i].GetComponent<Renderer>();
            _renderer.material.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            
            if (i < 50)
                duplicates[i].transform.localPosition = new Vector3(-5, UnityEngine.Random.Range(-10, 10),
                    duplicates[i].transform.localPosition.z);
            else
                duplicates[i].transform.localPosition = new Vector3(20, UnityEngine.Random.Range(-10, 10),
                    duplicates[i].transform.localPosition.z);
        }

        StartCoroutine("MoveDuplicates");
    }

    private IEnumerator MoveDuplicates()
    {
        yield return new WaitForSeconds(0.9f);

        while (time < 20)
        {
            for (int i = 0; i < duplicates.Length; i++)
            {
                if (duplicates[i].transform.position != objects[i].transform.position)
                {
                    duplicates[i].transform.position = Vector3.MoveTowards(duplicates[i].transform.position,
                        objects[i].transform.position,
                        0.7f * origin.transform.localScale.x);
                }
            }

            yield return new WaitForSeconds(0.01f);
            time += .1f;
        }

        SceneManager.LoadScene(0);
    }
}