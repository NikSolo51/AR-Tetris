using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public int amount = 2;
    public float fragmentSize = 0.5f;
    public float explosionForce = 50f;
    public float explosionRadius = 4f;
    public GameObject prefab;

    private float cubesPivotDistance;
    private Vector3 cubesPivot;
    private Renderer _renderer;

    public void ExplodeObjects(List<GameObject> objects)
    {
        foreach (GameObject o in objects)
        {
            cubesPivotDistance = fragmentSize * amount / 2;
            cubesPivot = new Vector3(cubesPivotDistance, cubesPivotDistance, cubesPivotDistance);
            Explode(o);
        }
    }

    public void Explode(GameObject obj)
    {
        Vector3 pos = obj.transform.position;
        _renderer = obj.GetComponent<Renderer>();
        _renderer.material.color = new Color(0, 0, 0, 0);

        for (int x = 0; x < amount; x++)
        {
            for (int y = 0; y < amount; y++)
            {
                for (int z = 0; z < amount; z++)
                {
                    CreateFragment(pos, x, y, z);
                }
            }
        }

        Vector3 explosionPos = pos;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, pos, explosionRadius, 1);
            }
        }
    }

    void CreateFragment(Vector3 pos, int x, int y, int z)
    {
        GameObject frag;
        frag = Instantiate(prefab);
        frag.transform.localPosition = pos + new Vector3(fragmentSize * x, fragmentSize * y, fragmentSize * z) - cubesPivot;
        frag.transform.localScale = new Vector3(fragmentSize * .2f, fragmentSize * .2f, fragmentSize * .2f);
        Destroy(frag,3);
    }
}