using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Article
{
    public bool isMyFavorite;
    public int checkCount;
    public string title;
    public string body;
    public string date;
}

[System.Serializable]
public class Articles
{
    public List<Article> articles = new();
}

public class ArticleManager : MonoBehaviour
{
    public Articles articles = new();
}
