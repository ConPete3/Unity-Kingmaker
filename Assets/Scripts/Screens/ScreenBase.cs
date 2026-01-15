using UnityEngine;
using UnityEngine.Events;

public abstract class ScreenBase : MonoBehaviour
{
    [SerializeField] private GameScreen screenType;
    [SerializeField] private GameObject screenRoot;

    public GameScreen ScreenType => screenType;

    [HideInInspector] public UnityEvent OnScreenShown = new UnityEvent();
    [HideInInspector] public UnityEvent OnScreenHidden = new UnityEvent();

    public virtual void Show(object context = null)
    {
        if (screenRoot != null)
            screenRoot.SetActive(true);
        else
            gameObject.SetActive(true);

        OnShow(context);
        OnScreenShown?.Invoke();
    }

    public virtual void Hide()
    {
        OnHide();

        if (screenRoot != null)
            screenRoot.SetActive(false);
        else
            gameObject.SetActive(false);

        OnScreenHidden?.Invoke();
    }

    protected abstract void OnShow(object context);
    protected abstract void OnHide();
}
