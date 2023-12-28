using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArticleType { Normal, Announce }

[Serializable]
public class Article
{
    public bool isMyFavorite;
    public bool isMine;
    public int checkCount;
    public string title;
    public ArticleType articleType;
    public string body;
    public string date;
}

[Serializable]
public class Articles
{
    public List<Article> articles = new();
}

public class ArticleManager : MonoBehaviour
{
    public static ArticleManager Instance { get; private set; }

    private Articles articles = new();

    public List<Article> Articles => articles.articles;

    private void Awake()
    {
        Instance = this;
    }

    public void AddArticle(string title, string body, ArticleType articleType)
    {
        articles.articles.Add(new Article() { title = title, body = body, articleType = articleType, date = DateTime.Now.ToString("yyyy-MM-dd hh:mm"), isMine = true });
    }
}
