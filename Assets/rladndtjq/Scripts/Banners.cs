using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Banners : MonoBehaviour
{
    [SerializeField] private Image[] banners;
    
    private void Start()
    {
        WindowManager.Instance.OnIndexChange += (pre, cur) =>
        {
            banners[pre].DOColor(new Color(1, 1, 1, 0), 0.25f);
            banners[cur].DOColor(Color.white, 0.25f);
        };
    }
}
