using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeUI : MonoBehaviour
{
    [SerializeField]
    private Scrollbar scrollbar;
    [SerializeField]
    private Transform[] circleContents;
    [SerializeField]
    private float swipeTime = 0.2f;
    [SerializeField]
    private float swipeDistance = 50.0f;

    private float[] scrollPageValues;
    private float valueDistance = 0;
    private int currentPage = 0;
    private int maxPage = 0;
    private float startTouchX;
    private float endTouchX;
    private bool isSwipeMode = false;
    private float circleContentScale = 1.6f;

    private void Awake()
    {
        scrollPageValues = new float[transform.childCount];

        valueDistance = 1f / (scrollPageValues.Length - 1f);

        for(int i = 0; i < scrollPageValues.Length; ++i)
        {
            scrollPageValues[i] = valueDistance * i;
        }

        maxPage = transform.childCount;
    }

    private void Start()
    {
        SetScrollBarValue(0);
    }

    public void SetScrollBarValue(int index)
    {
        currentPage = index;
        scrollbar.value = scrollPageValues[index];
    }

    private void Update()
    {
        UpdateInput();
        UpdateCircleContent();
    }

    private void UpdateInput()
    {
        if (isSwipeMode) return;

#if UNITY_EDITOR
        if(Input.GetMouseButtonDown(0))
        {
            startTouchX = Input.mousePosition.x;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            endTouchX = Input.mousePosition.x;
            UpdateSwipe();
        }
#endif
#if UNITY_ANDROID
        if(Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                startTouchX = touch.position.x;
            }
            else if(touch.phase == TouchPhase.Ended)
            {
                endTouchX = touch.position.x;

                UpdateSwipe();
            }
        }
#endif
    }

    private void UpdateSwipe()
    {
        if(Mathf.Abs(startTouchX - endTouchX) < swipeDistance)
        {
            StartCoroutine(OnSwipeOneStep(currentPage));
            return;
        }

        bool isLeft = startTouchX < endTouchX ? true : false;

        if (isLeft)
        {
            if (currentPage == 0) return;
            currentPage--;
        }
        else
        {
            if (currentPage == maxPage - 1) return;
            currentPage++;
        }

        StartCoroutine(OnSwipeOneStep(currentPage));
    }

    private IEnumerator OnSwipeOneStep(int index)
    {
        float start = scrollbar.value;
        float current = 0;
        float percent = 0;

        isSwipeMode = true;

        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current / swipeTime;

            scrollbar.value = Mathf.Lerp(start, scrollPageValues[index], percent);

            yield return null;
        }

        isSwipeMode = false;
    }

    private void UpdateCircleContent()
    {
        for(int i = 0; i < scrollPageValues.Length; ++i)
        {
            circleContents[i].localScale = Vector2.one;
            circleContents[i].GetComponent<Image>().color = Color.white;

            if(scrollbar.value < scrollPageValues[i] + (valueDistance / 2) && scrollbar.value > scrollPageValues[i] - (valueDistance / 2))
            {
                circleContents[i].localScale = Vector2.one * circleContentScale;
                circleContents[i].GetComponent<Image>().color = Color.black;
            }
        }
    }
}
