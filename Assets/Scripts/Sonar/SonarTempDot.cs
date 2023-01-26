using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarTempDot : MonoBehaviour
{
    private float alpha = 1f;
    private SpriteRenderer spriteRenderer;

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SonarManager.instance.SonarUpdated.AddListener(DestroySelf);
        StartCoroutine(getDarker());
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    private IEnumerator getDarker()
    {
        while (alpha > 0.1f)
        {
            yield return new WaitForSeconds(0.1f);
            alpha -= 0.1f;
            spriteRenderer.color = new Color(1,1,1,alpha);
        }
        Destroy(gameObject);
    }
}
