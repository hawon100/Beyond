using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArticleSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI body;
    [SerializeField] private TextMeshProUGUI date;
    [SerializeField] private TextMeshProUGUI check;
    [SerializeField] private Image heart;
    [SerializeField] private Sprite fillHeart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private RectTransform infos;

    private Button btn;
    private Article article;

    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(() => ReadingWindow.Instance.Open(article));
    }

    public void Init(Article article)
    {
        this.article = article;

        title.text = article.title;
        body.text = article.body.Length > 30 ? article.body[0..30] + "..." : article.body;
        date.text = article.date;
        check.text = article.checkCount.ToString("#,##0");
        heart.sprite = article.isFavorite ? fillHeart : emptyHeart;

        LayoutRebuilder.ForceRebuildLayoutImmediate(infos);
    }
}
