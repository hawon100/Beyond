using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowManager : ScrollRect
{
    public static WindowManager Instance { get; private set; }

    private bool isDragging;
    private int index = 2;
    private List<RectTransform> windows = new();

    public Action<int, int> OnIndexChange { get; set; }
    public int Index
    {
        get => index;
        set
        {
            OnIndexChange?.Invoke(index, value);
            index = value;
        }
    }

    protected override void Awake()
    {
        Instance = this;

        foreach (RectTransform t in content)
            windows.Add(t);

        foreach (var window in windows)
            window.sizeDelta = new Vector2(1080, 1920 - 500);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        base.OnBeginDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        if (Mathf.Abs(eventData.delta.x) > 15)
        {
            if (eventData.delta.x < -15) index++;
            if (eventData.delta.x > 15) index--;
        }
        else
        {
            index = Mathf.RoundToInt(Mathf.Abs(content.anchoredPosition.x / 1080));
        }

        index = Mathf.Clamp(index, 0, windows.Count - 1);
        base.OnEndDrag(eventData);
    }

    private void Update()
    {
        if (!isDragging)
        {
            content.anchoredPosition =
                Vector2.Lerp(content.anchoredPosition, new Vector2(-1080 * index, 0), Time.deltaTime * 10);
        }
    }
}
