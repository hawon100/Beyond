using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReadingWindow : MonoBehaviour
{
    public static ReadingWindow Instance { get; private set; }

    [SerializeField] private RectTransform bg;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI count;
    [SerializeField] private TextMeshProUGUI date;
    [SerializeField] private Button heart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private Sprite fillHeart;
    [SerializeField] private Button remove;
    [SerializeField] private Button close;
    [SerializeField] private TextMeshProUGUI body;

    private Article article;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        close.onClick.AddListener(Close);
    }

    public void Open(Article article)
    {
        this.article = article;
        article.checkCount++;

        title.text = article.title;
        count.text = article.checkCount.ToString("#,##0");
        date.text = article.date;

        heart.onClick.RemoveAllListeners();
        heart.onClick.AddListener(() => article.isFavorite = !article.isFavorite);

        remove.onClick.RemoveAllListeners();
        remove.onClick.AddListener(() =>
        {
            ArticleManager.Instance.RemoveArticle(article);
            Close();
            this.article = null;
        });

        body.text = article.body;

        bg.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutQuad);
    }

    public void Close()
    {
        WindowManager.Instance.RefreshCurrentWindow();
        bg.DOAnchorPosY(-1920, 0.5f).SetEase(Ease.OutQuad);
    }

    private void Update()
    {
        if(ReferenceEquals(article, null)) return;

        heart.image.sprite = article.isFavorite ? fillHeart : emptyHeart;
    }
}
