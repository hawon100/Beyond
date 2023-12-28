using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public abstract class TaskData
{
    public abstract UniTask Task();
}

public class LoadingTaskData : TaskData
{
    public LoadingTaskData(Func<UniTask> task, Action onComplete = null)
    {
        this.task = UniTask.Defer(task);
        this.onComplete = onComplete;
    }

    public override async UniTask Task()
    {
        await task;
        onComplete?.Invoke();
    }

    public UniTask task;
    public Action onComplete;
}

public class LoadingTaskData<T> : TaskData
{
    public LoadingTaskData(Func<UniTask<T>> task, Action<T> onComplete = null)
    {
        this.task = UniTask.Defer(task);
        this.onComplete = onComplete;
    }

    public override async UniTask Task()
    {
        onComplete?.Invoke(await task);
    }

    public UniTask<T> task;
    public Action<T> onComplete;
}

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance { get; private set; }

    [SerializeField] private Image bg;
    [SerializeField] private Image fader;
    [SerializeField] private Image progressbar;
    [SerializeField] private RectTransform logo;
    [SerializeField] private RectTransform blackboard;

    private List<TaskData> loadingTasks = new();
    private float progress;

    public Action OnLoadComplete { get; set; }

    public void AddTask(TaskData taskData)
        => loadingTasks.Add(taskData);

    private void Awake()
    {
        Screen.SetResolution(540, 960, false);
        Instance = this;
    }

    private async void Start()
    {
        var color = fader.color;
        color.a = 0;
        fader.DOColor(color, 1f);
        await UniTask.WaitForSeconds(1f);
        fader.gameObject.SetActive(false);

        var count = 0;
        try
        {
            foreach (var taskData in loadingTasks)
            {
                await taskData.Task();
                count++;
                progress = (float)count / loadingTasks.Count;
            }
        }
        catch
        {
            Application.Quit();
            return;
        }

        await UniTask.WaitForSeconds(2f);
        progressbar.DOColor(new Color(1, 0.9f, 0, 0), 2f);
        logo.DOAnchorPosY(216, 1.3f).SetEase(Ease.InQuad);
        logo.DORotate(new Vector3(0, 0, -180), 4);
        await UniTask.WaitForSeconds(1.3f);
        logo.DOAnchorPos(new Vector2(948, 600), 1.5f).SetEase(Ease.OutQuad);
        blackboard.DOAnchorPos(new Vector2(-117, -1612), 1.3f).SetEase(Ease.InQuad);
        blackboard.DORotate(new Vector3(0, 0, -117), 1.3f).SetEase(Ease.InQuad);
        await UniTask.WaitForSeconds(2f);
        bg.DOColor(new Color(1, 1, 1, 0), 1f);
        await UniTask.WaitForSeconds(1f);
        bg.gameObject.SetActive(false);
    }

    private void Update()
    {
        progressbar.fillAmount = Mathf.Lerp(progressbar.fillAmount, progress, Time.deltaTime * 5f);
    }
}
