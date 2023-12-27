using UnityEngine;
using UnityEngine.UI;

public class DownButtons : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    
    private void Start()
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            var index = i;
            buttons[i].onClick.AddListener(() => WindowManager.Instance.Index = index);
        }
    }
}