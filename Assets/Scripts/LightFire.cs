using UnityEngine;

[ExecuteInEditMode]
public class LightFire : MonoBehaviour
{
    public float waveSpeed;
    public Vector3 waveAmplitude;
    public Vector3 waveOffset;
    private float shiftX;
    private float shiftY;
    private float shiftZ;

    private void Start()
    {
        shiftX = Random.value * 50;
        shiftY = Random.value * 50;
        shiftZ = Random.value * 50;
    }

    void Update()
    {
        float delta = Time.time * waveSpeed;
        float moveX = Mathf.PerlinNoise(delta, shiftX + delta) - .5f;
        float moveY = Mathf.PerlinNoise(delta, shiftY + delta) - .5f;
        float moveZ = Mathf.PerlinNoise(delta, shiftZ + delta) - .5f;
        transform.localPosition = waveOffset + new Vector3(
            moveX * waveAmplitude.x,
            moveY * waveAmplitude.y,
            moveZ * waveAmplitude.z);
    }
}
