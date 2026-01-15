using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }

    [SerializeField] private List<ScreenBase> screens;
    [SerializeField] private GameScreen initialScreen = GameScreen.GlobalMap;

    private Dictionary<GameScreen, ScreenBase> screenLookup = new Dictionary<GameScreen, ScreenBase>();
    private ScreenBase currentScreen;
    private GameScreen currentScreenType;

    public GameScreen CurrentScreenType => currentScreenType;
    public ScreenBase CurrentScreen => currentScreen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (var screen in screens)
        {
            if (screen != null)
            {
                screenLookup[screen.ScreenType] = screen;
                screen.Hide();
            }
        }
    }

    private void Start()
    {
        ShowScreen(initialScreen);
    }

    public void ShowScreen(GameScreen screenType, object context = null)
    {
        if (currentScreen != null)
        {
            currentScreen.Hide();
        }

        if (screenLookup.TryGetValue(screenType, out ScreenBase screen))
        {
            currentScreen = screen;
            currentScreenType = screenType;
            screen.Show(context);
            Debug.Log($"Switched to screen: {screenType}");
        }
        else
        {
            Debug.LogError($"Screen not found: {screenType}");
        }
    }

    public void GoBack()
    {
        if (currentScreenType != GameScreen.GlobalMap)
        {
            ShowScreen(GameScreen.GlobalMap);
        }
    }
}
