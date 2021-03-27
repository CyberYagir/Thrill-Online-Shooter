using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopUp : MonoBehaviour
{
    public Vector3 worldpoint;
    public Camera camera;
    public Color color = Color.white;
    public TMP_Text text;
    private void Update()
    {
        GetComponent<RectTransform>().position = camera.WorldToScreenPoint(worldpoint, Camera.MonoOrStereoscopicEye.Mono);
        text.color = color;
        color = Color.Lerp(color, new Color(0, 0, 0, 0), 1 * Time.deltaTime);
        if (color.a == 0)
        {
            Destroy(gameObject);
        }
    }
}
