using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class ArticleWindow : Window
{
    protected abstract Func<Article, bool> predicate { get; }

    [SerializeField] protected ArticleSlot articleSlotPrefab;
    [SerializeField] protected int pageArticleCount;
    [SerializeField] protected int buttonCount;
    [SerializeField] protected Button[] preButtons;
    [SerializeField] protected Button[] nextButtons;
    [SerializeField] protected RectTransform articleSlotGroup;
    [SerializeField] protected TextMeshProUGUI currentPageIndexText;
    [SerializeField] protected Button writingButton;

    protected int currentDisplayPageIndex;
    protected Article[] filtered;
    protected List<ArticleSlot> articleSlots = new();
    protected TextMeshProUGUI[] preButtonTexts;
    protected TextMeshProUGUI[] nextButtonTexts;

    protected int MaxPage => filtered.Length / pageArticleCount;

    protected virtual void Start()
    {
        preButtonTexts = new TextMeshProUGUI[buttonCount];
        nextButtonTexts = new TextMeshProUGUI[buttonCount];

        for(int i = 0; i < buttonCount; i++)
        {
            preButtonTexts[i] = preButtons[i].GetComponent<TextMeshProUGUI>();
            nextButtonTexts[i] = nextButtons[i].GetComponent<TextMeshProUGUI>();
        }

        if(writingButton)
            writingButton.onClick.AddListener(() => WritingWindow.Instance.Open());

        WindowManager.Instance.OnIndexChange += (pre, cur) =>
        {
            if (cur != WindowIndex) return;
            Refresh();
        };

        ArticleManager.Instance.OnArticleRemove += article => Refresh();
    }

    public override void Refresh()
    {
        filtered = ArticleManager.Instance.Articles.Where(item => predicate(item)).ToArray();

        if(filtered.Length == 0)
        {
            foreach(var btn in preButtons) btn.gameObject.SetActive(false);
            foreach(var btn in nextButtons) btn.gameObject.SetActive(false);
            currentPageIndexText.text = "1";
            
            articleSlots.Clear();
            foreach(Transform t in articleSlotGroup)
                Destroy(t.gameObject);

            return;
        }

        var displayCount =
            currentDisplayPageIndex == MaxPage ? filtered.Length % pageArticleCount : pageArticleCount;

        while (articleSlots.Count != displayCount)
        {
            if (articleSlots.Count > displayCount)
            {
                Destroy(articleSlots[^1].gameObject);
                articleSlots.RemoveAt(articleSlots.Count - 1);
            }
            if (articleSlots.Count < displayCount)
            {
                articleSlots.Add(Instantiate(articleSlotPrefab, articleSlotGroup));
            }
        }

        for (int i = 0; i < displayCount; i++)
            articleSlots[i].Init(filtered[^(currentDisplayPageIndex * pageArticleCount + i + 1)]);

        for (int i = 0; i < buttonCount; i++)
        {
            var targetIndex = currentDisplayPageIndex - (i + 1);

            preButtonTexts[i].text = (targetIndex + 1).ToString();
            preButtons[i].gameObject.SetActive(targetIndex >= 0);
            preButtons[i].onClick.RemoveAllListeners();
            preButtons[i].onClick.AddListener(() =>
            {
                currentDisplayPageIndex = targetIndex;
                Refresh();
            });
        }

        currentPageIndexText.text = (currentDisplayPageIndex + 1).ToString();

        for (int i = 0; i < buttonCount; i++)
        {
            var targetIndex = currentDisplayPageIndex + i + 1;

            nextButtonTexts[i].text = (targetIndex + 1).ToString();
            nextButtons[i].gameObject.SetActive(targetIndex <= MaxPage);
            nextButtons[i].onClick.RemoveAllListeners();
            nextButtons[i].onClick.AddListener(() =>
            {
                currentDisplayPageIndex = targetIndex;
                Refresh();
            });
        }
    }
}
