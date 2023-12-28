using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyArticleWindow : ArticleWindow
{
    protected override Func<Article, bool> predicate => article => article.isMine;
}
