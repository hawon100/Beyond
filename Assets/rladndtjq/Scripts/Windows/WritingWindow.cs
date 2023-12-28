using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WritingWindow : MonoBehaviour
{
    public static WritingWindow Instance { get; private set; }

    [SerializeField] private RectTransform bg;
    [SerializeField] private Button exit;
    [SerializeField] private TMP_InputField titleInput;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private TMP_InputField body;
    [SerializeField] private Button submit;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        titleInput.text = "";
        dropdown.value = 0;
        body.text = "";

        submit.onClick.AddListener(() =>
        {
            ArticleManager.Instance.AddArticle(titleInput.text, body.text, (ArticleType)dropdown.value);
            WindowManager.Instance.RefreshCurrentWindow();
            Close();
        });

        exit.onClick.AddListener(Close);
    }

    public void Open()
    {
        titleInput.text = "";
        dropdown.value = 0;
        body.text = "";

        bg.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutQuad);
    }

    public void Close()
    {
        bg.DOAnchorPosY(-1920, 0.5f).SetEase(Ease.OutQuad);
    }
}
