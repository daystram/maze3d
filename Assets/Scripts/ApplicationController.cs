using UnityEngine;
using SFB;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

public class ApplicationController : MonoBehaviour
{
    const int DIMENSION_LIMIT = 9;
    private Color COLOR_ENABLED = new Color(0, 1, 0, .5f);
    private Color COLOR_DISABLED = new Color(1, 0, 0, .5f);

    private static ApplicationController instance;

    private Texture2D mazeMap;
    private string mapPath;

    public GameController gameController;
    public PlayerController playerController;

    public Volume volume;
    private GlobalIllumination globalIllumination;
    private Fog fog;

    public Canvas menuUI;
    public GameObject graphUI;
    public Canvas finishUI;
    public RawImage map;
    public Text pathText;
    public Text durationText;

    public Image statusMouse;
    public Image statusDayCycle;
    public Image statusGI;
    public Image statusFog;

    public static ApplicationController GetInstance()
    {
        if (instance == null)
        {
            Debug.Log("[ApplicationController] Controller has not been instantiated!");
        }
        return instance;
    }
    void Awake()
    {
        instance = this;
        volume.profile.TryGet(out globalIllumination);
        volume.profile.TryGet(out fog);
        LoadNewMap(true);
        playerController.EnableControl(true);
        menuUI.enabled = true;
        graphUI.SetActive(true);
    }

    public static string GetMazeMapPath()
    {
        string[] paths = new StandaloneFileBrowserWindows()
            .OpenFilePanel("Select a " + DIMENSION_LIMIT + "x" + DIMENSION_LIMIT + " Maze Map", "./", new[] { new ExtensionFilter("", "png") }, false);
        if (paths.Length != 0)
        {
            return paths[0];
        }
        return "";
    }

    public Texture2D ParseTexture(string path)
    {
        Texture2D mazeMap = new Texture2D(9, 9);
        mazeMap.LoadImage(File.ReadAllBytes(mapPath));
        mazeMap.filterMode = FilterMode.Point;
        return mazeMap;
    }

    public static void EndGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    private void Update()
    {
        if (Input.GetButtonDown("Menu"))
        {
            menuUI.enabled = !menuUI.enabled;
            graphUI.SetActive(menuUI.enabled);
        }
        if (Input.GetButtonDown("Daylight"))
        {
            gameController.ToggleDaylightCycle();
            statusDayCycle.color = gameController.doDaylightCycle ? COLOR_ENABLED : COLOR_DISABLED;
        }
        if (Input.GetButtonDown("Cancel"))
        {
            playerController.EnableControl(!playerController.controlEnabled);
            statusMouse.color = playerController.controlEnabled ? COLOR_ENABLED : COLOR_DISABLED;
        }
        if (Input.GetButtonDown("GI") && globalIllumination)
        {
            globalIllumination.active = !globalIllumination.active;
            statusGI.color = globalIllumination.active ? COLOR_ENABLED : COLOR_DISABLED;
        }
        if (Input.GetButtonDown("Fog") && fog)
        {
            fog.active = !fog.active;
            statusFog.color = fog.active ? COLOR_ENABLED : COLOR_DISABLED;
        }
    }

    public void LoadNewMap(bool exitOnFail = false)
    {
        mapPath = GetMazeMapPath();
        if (mapPath.Length == 0)
        {
            Debug.LogError("[ApplicationController] No maze map provided!");
            if (exitOnFail) EndGame();
            else return;
        }
        mazeMap = ParseTexture(mapPath);
        if (mazeMap.width > DIMENSION_LIMIT || mazeMap.height > DIMENSION_LIMIT)
        {
            Debug.LogError("[ApplicationController] Map larger than DIMENSION_LIMIT");
            if (exitOnFail) EndGame();
            else return;
        }
        gameController.LoadMap(mazeMap);
        map.texture = mazeMap;
        pathText.text = Path.GetFileName(mapPath);
        ShowFinishUI(-1);
        playerController.EnableControl(true);
    }

    public void SetPlayerController(PlayerController playerController)
    {
        this.playerController = playerController;
    }

    public void ShowFinishUI(float duration)
    {
        durationText.text = string.Format("{0:0.##} seconds", duration);
        finishUI.enabled = duration >= 0;
        playerController.EnableControl(duration < 0);
        statusMouse.color = playerController.controlEnabled ? COLOR_ENABLED : COLOR_DISABLED;
    }
}
