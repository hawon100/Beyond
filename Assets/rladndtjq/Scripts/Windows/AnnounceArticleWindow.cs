using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnounceArticleWindow : ArticleWindow
{
    protected override Func<Article, bool> predicate => article => article.articleType == ArticleType.Announce;
}
