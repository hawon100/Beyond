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
    private List<Window> windows = new();

    public Action<int, int> OnIndexChange { get; set; }
    public int Index
    {
        get => index;
        set
        {
            var v = Mathf.Clamp(value, 0, windows.Count - 1);
            if(index != v)
            {
                OnIndexChange?.Invoke(index, v);
                index = v;
            }
        }
    }

    protected override void Awake()
    {
        Instance = this;

        var index = 0;
        foreach (RectTransform t in content)
        {
            windows.Add(t.GetComponent<Window>());
            windows[^1].WindowIndex = index++;
            t.sizeDelta = new Vector2(1080, 1920 - 500);
        }
    }

    public void RefreshCurrentWindow()
    {
        windows[index].Refresh();
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
            if (eventData.delta.x < -15) Index++;
            if (eventData.delta.x > 15) Index--;
        }
        else
        {
            Index = Mathf.RoundToInt(Mathf.Abs(content.anchoredPosition.x / 1080));
        }

        base.OnEndDrag(eventData);
    }

    private void Update()
    {
        if (!isDragging)
        {
            content.anchoredPosition =
                Vector2.Lerp(content.anchoredPosition, new Vector2(-1080 * Index, 0), Time.deltaTime * 10);
        }
    }
}
