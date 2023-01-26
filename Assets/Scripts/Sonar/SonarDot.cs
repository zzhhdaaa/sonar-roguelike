using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarDot : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float alpha = 1f;

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SonarManager.instance.SonarUpdated.AddListener(UpdateDotColor);
        //SonarManager.instance.SonarUpdated += UpdateDotColor;
    }

    private void UpdateDotColor()
    {
        if (alpha > 0.2f)
        {
            alpha -= 0.1f;
            spriteRenderer.color = new Color(1, 1, 1, alpha);
        }
        if (alpha <= 0.2f)
        {
            SonarManager.instance.SonarUpdated.RemoveListener(this.UpdateDotColor);
        }
    }
}
