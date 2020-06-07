using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private Texture2D mazeMap;

    public GameObject sun;
    public float dayLength;

    private GameObject mazeRoot;
    private GameObject entityRoot;
    public GameObject wallGroup;
    public GameObject groundGroup;
    public GameObject grassGroup;

    private Light sunLight;
    private Transform sunTransform;

    public GameObject tilePrefab;
    public GameObject wallPrefab;
    public GameObject playerPrefab;
    public GameObject mirrorPrefab;
    public GameObject lightPrefab;
    public GameObject startTrigger;
    public GameObject finishTrigger;

    public int dayIntensity;
    public int nightIntensity;
    public int dayTemperature;
    public int nightTemperature;

    public float sunriseAngle;
    public float sunsetAngle;
    public float azimuthAngle;

    private float currentTime;
    private float sunXRot;
    private bool day = true;
    private float sunDampeningFactor = .5f;
    public bool doDaylightCycle;

    private float startTime;
    private bool finished = false;

    private static Vector3[] positionSlot = new Vector3[] {
        new Vector3(-.40f, 0f,  .40f),
        new Vector3(   0f, 0f,  .40f),
        new Vector3( .40f, 0f,  .40f),
        new Vector3(-.40f, 0f,    0f),
        new Vector3(   0f, 0f,    0f),
        new Vector3( .40f, 0f,    0f),
        new Vector3(-.40f, 0f, -.40f),
        new Vector3(   0f, 0f, -.40f),
        new Vector3( .40f, 0f, -.40f),
    };
    private static Quaternion[] rotationSlot = new Quaternion[] {
        Quaternion.LookRotation(Vector3.right, Vector3.up),
        Quaternion.LookRotation(Vector3.back, Vector3.up),
        Quaternion.LookRotation(Vector3.left, Vector3.up),
        Quaternion.LookRotation(Vector3.forward, Vector3.up),
        Quaternion.LookRotation(Vector3.right + Vector3.back, Vector3.up),
        Quaternion.LookRotation(Vector3.back + Vector3.left, Vector3.up),
        Quaternion.LookRotation(Vector3.left + Vector3.forward, Vector3.up),
        Quaternion.LookRotation(Vector3.forward + Vector3.right, Vector3.up),
    };

    void Awake()
    {
        sunXRot = sunriseAngle;
        sunLight = sun.GetComponent<Light>();
        sunTransform = sun.GetComponent<Transform>();

        sunTransform.rotation = Quaternion.Euler(sunXRot, azimuthAngle, 0);
        sunLight.intensity = dayIntensity;
        sunLight.colorTemperature = dayTemperature;
    }

    public void LoadMap(Texture2D mazeMap)
    {
        this.mazeMap = mazeMap;
        Destroy(entityRoot);
        ParseMap();
        MergeMesh();
        startTime = Time.time;
        finished = false;
    }

    public void ParseMap()
    {
        mazeRoot = new GameObject("MazeRoot");
        entityRoot = new GameObject("EntityRoot");
        for (var i = 0; i < mazeMap.width; i++)
        {
            for (var j = 0; j < mazeMap.height; j++)
            {
                Color pixel = mazeMap.GetPixel(i, j);
                if (pixel == Color.black) Instantiate(wallPrefab, new Vector3(i, 0, j), Quaternion.identity, mazeRoot.transform);
                else
                {
                    Instantiate(tilePrefab, new Vector3(i, 0, j), Quaternion.identity, mazeRoot.transform);
                    if (pixel == Color.red)
                    {
                        PlaceSubTransform(Instantiate(startTrigger, new Vector3(i, 0, j), Quaternion.identity, entityRoot.transform), i, j);
                        GameObject player = Instantiate(playerPrefab, new Vector3(i, 0.5f, j), Quaternion.identity, entityRoot.transform);
                        ApplicationController.GetInstance().SetPlayerController(player.GetComponent<PlayerController>());
                    }
                    if (pixel == Color.blue)
                    {
                        GameObject finish = Instantiate(finishTrigger, new Vector3(i, 0, j), Quaternion.identity, entityRoot.transform);
                        finish.GetComponentInChildren<FinishTrigger>().Activate(this);
                        PlaceSubTransform(finish, i, j);
                    }
                    if (pixel == Color.green) PlaceSubTransform(Instantiate(mirrorPrefab, new Vector3(i, 0f, j), Quaternion.identity, entityRoot.transform), i, j);
                    if (pixel == Color.magenta) PlaceSubTransform(Instantiate(lightPrefab, new Vector3(i, 0f, j), Quaternion.identity, entityRoot.transform), i, j);
                }
            }
        }
    }

    public void PlaceSubTransform(GameObject gameObject, int i, int j)
    {
        Transform transform = gameObject.transform;
        int type = 0;
        if (j + 1 > mazeMap.height - 1 || mazeMap.GetPixel(i, j + 1) == Color.black) type += 1;
        if (i + 1 > mazeMap.width - 1 || mazeMap.GetPixel(i + 1, j) == Color.black) type += 2;
        if (j - 1 < 0 || mazeMap.GetPixel(i, j - 1) == Color.black) type += 4;
        if (i - 1 < 0 || mazeMap.GetPixel(i - 1, j) == Color.black) type += 8;
        Debug.Log("(" + i + ", " + j + ") === " + type);
        Vector3 position = positionSlot[4];
        Quaternion rotation = rotationSlot[3];
        switch (type)
        {
            case 0:
                position = positionSlot[3];
                rotation = rotationSlot[0];
                break;
            case 1:
                position = positionSlot[1];
                rotation = rotationSlot[1];
                break;
            case 2:
                position = positionSlot[5];
                rotation = rotationSlot[2];
                break;
            case 3:
                position = positionSlot[2];
                rotation = rotationSlot[5];
                break;
            case 4:
                position = positionSlot[7];
                rotation = rotationSlot[3];
                break;
            case 5:
                position = positionSlot[1];
                rotation = rotationSlot[1];
                break;
            case 6:
                position = positionSlot[8];
                rotation = rotationSlot[6];
                break;
            case 7:
                position = positionSlot[5];
                rotation = rotationSlot[2];
                break;
            case 8:
                position = positionSlot[3];
                rotation = rotationSlot[0];
                break;
            case 9:
                position = positionSlot[0];
                rotation = rotationSlot[4];
                break;
            case 10:
                position = positionSlot[3];
                rotation = rotationSlot[0];
                break;
            case 11:
                position = positionSlot[1];
                rotation = rotationSlot[1];
                break;
            case 12:
                position = positionSlot[6];
                rotation = rotationSlot[7];
                break;
            case 13:
                position = positionSlot[3];
                rotation = rotationSlot[0];
                break;
            case 14:
                position = positionSlot[7];
                rotation = rotationSlot[3];
                break;
            case 15:
                position = positionSlot[3];
                rotation = rotationSlot[0];
                break;
        }
        transform.rotation = rotation;
        transform.position = position + new Vector3(i, 0f, j);
    }

    public void MergeMesh()
    {
        MeshFilter[] children = mazeRoot.GetComponentsInChildren<MeshFilter>();
        ArrayList instancesWall = new ArrayList();
        ArrayList instancesGround = new ArrayList();
        ArrayList instancesGrass = new ArrayList();
        for (int i = 0; i < children.Length; i++)
        {
            CombineInstance instance = new CombineInstance();
            string name = children[i].transform.name;
            if (name == "Player") continue;
            instance.subMeshIndex = 0;
            instance.mesh = children[i].sharedMesh;
            instance.transform = children[i].transform.localToWorldMatrix;
            if (name == "Wall")
            {
                instancesWall.Add(instance);
            }
            else if (name == "Ground")
            {
                instancesGround.Add(instance);
            }
            else if (name == "Grass")
            {
                instancesGrass.Add(instance);
            }
        }
        Mesh combinedWall = new Mesh();
        combinedWall.CombineMeshes(
            instancesWall.ToArray(typeof(CombineInstance)) as CombineInstance[]);
        combinedWall.Optimize();
        wallGroup.GetComponent<MeshFilter>().sharedMesh = combinedWall;
        wallGroup.GetComponent<MeshCollider>().sharedMesh = combinedWall;
        Mesh combinedGround = new Mesh();
        combinedGround.CombineMeshes(
            instancesGround.ToArray(typeof(CombineInstance)) as CombineInstance[]);
        combinedGround.Optimize();
        groundGroup.GetComponent<MeshFilter>().sharedMesh = combinedGround;
        groundGroup.GetComponent<MeshCollider>().sharedMesh = combinedGround;
        Mesh combinedGrass = new Mesh();
        combinedGrass.CombineMeshes(
            instancesGrass.ToArray(typeof(CombineInstance)) as CombineInstance[]);
        combinedGrass.Optimize();
        grassGroup.GetComponent<MeshFilter>().sharedMesh = combinedGrass;
        Destroy(mazeRoot);
    }

    public void FinishGame()
    {
        if (!finished)
        {
            finished = true;
            ApplicationController.GetInstance().ShowFinishUI(Time.time - startTime);
        }
    }
    public void ContinueGame()
    {
        
        ApplicationController.GetInstance().ShowFinishUI(-1);
    }

    public void ToggleDaylightCycle()
    {
        doDaylightCycle = !doDaylightCycle;
    }

    void FixedUpdate()
    {
        if (doDaylightCycle) MoveSun();
    }

    void MoveSun()
    {
        float intensity;
        float colorTemperature;
        currentTime += Time.deltaTime;
        if (sunXRot > sunsetAngle)
        {
            day = false;
            currentTime = 0;
        }
        else if (sunXRot < sunriseAngle)
        {
            day = true;
            currentTime = 0;
        }
        if (day)
        {
            sunXRot = Mathf.Lerp(
                sunriseAngle, sunsetAngle + .1f, currentTime / dayLength);
            intensity = Mathf.Lerp(
                nightIntensity, dayIntensity, currentTime / sunDampeningFactor);
            colorTemperature = Mathf.Lerp(
                nightTemperature, dayTemperature, currentTime / sunDampeningFactor);

        }
        else
        {
            sunXRot = Mathf.Lerp(
                sunsetAngle, sunriseAngle - .1f, currentTime / dayLength);
            intensity = Mathf.Lerp(
                dayIntensity, nightIntensity, currentTime / sunDampeningFactor);
            colorTemperature = Mathf.Lerp(
                dayTemperature, nightTemperature, currentTime / sunDampeningFactor);
        }
        sunTransform.rotation = Quaternion.Euler(sunXRot, azimuthAngle, 0);
        sunLight.intensity = intensity;
        sunLight.colorTemperature = colorTemperature;
    }
}
