using MyBox;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum UIAction { PAUSE, RESUME, TOGGLE_PAUSE, FADE_TO_BLACK, FADE_FROM_BLACK};
public enum UIResult { };

public class UIManager : MonoBehaviour
{
    public static UIManager i;

    [HideInInspector] public UnityEvent<UIAction, object> OnUpdateUI;
    [HideInInspector] public UnityEvent<UIResult> OnResult;

    public void TriggerOnResult(UIResult parameter) => OnResult?.Invoke(parameter);
    public void Do(UIAction type) => OnUpdateUI?.Invoke(type, null);

    private void Awake()
    {
        if (i != null) Destroy(i.gameObject);
        i = this;

        var controllers = GetComponentsInChildren<UIController>(true);
        foreach (var controller in controllers) controller.Initialize();
    }

    private void Start()
    {
        Do(UIAction.FADE_FROM_BLACK);
    }

    private void Update()
    {
        if (InputController.GetDown(Control.PAUSE)) Do(UIAction.TOGGLE_PAUSE);
    }

    public void Do<T>(UIAction type, T parameter = default)
    {
        CheckParameter(parameter);
        OnUpdateUI?.Invoke(type, parameter);
    }

    private void CheckParameter<T>(T parameter)
    {
        if (parameter == null || parameter is T) return;
        Debug.LogError("Incorrect parameter passed. expected type: " + typeof(T) + ", got: " + parameter.GetType());
    }
}
