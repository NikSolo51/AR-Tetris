using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
    Renderer r;
    private void Start()
    {
         r = GetComponent<Renderer>();
        StartCoroutine("Fade");
    }
    IEnumerator Fade()
    {
        for (float ft = 1f; true; ft -= 0.2f)
        {
            Color c = r.material.color;
            c.a = ft;
            r.material.color = c;
            if (ft <= 0)
                Destroy(this.gameObject);
            yield return new WaitForSeconds(.2f);
        }
    }
}
