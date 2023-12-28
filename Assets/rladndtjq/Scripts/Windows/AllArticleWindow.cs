using System;
using UnityEngine;

public class AllArticleWindow : ArticleWindow
{
    protected override Func<Article, bool> predicate => article => true;
}