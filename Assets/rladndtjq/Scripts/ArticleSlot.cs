using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArticleSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI date;
    [SerializeField] private TextMeshProUGUI check;
    [SerializeField] private Image heart;
    [SerializeField] private Sprite fillHeart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private RectTransform infos;

    public void Init(Article article)
    {
        title.text = article.title;
        date.text = article.date;
        check.text = article.checkCount.ToString("#,##0");
        heart.sprite = article.isMyFavorite ? fillHeart : emptyHeart;

        LayoutRebuilder.ForceRebuildLayoutImmediate(infos);
    }
}
