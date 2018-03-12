using System.Collections.Generic;
using UnityEngine;

public class GenerateMeshes : MonoBehaviour
{
    public float height = 4f;
    public float radius = 1f;
    public int edgesCount = 8;
    public int arcsCount = 8;
    public float loudnessHeight = 20f;
    public float loudnessRadius = 8f;
    public int spectrumDivider = 64;
    public float updateStep = 20f;
    public int sampleDataLength = 8192;
    public int sampleDataLengthDisplayed;
    public float lerpRatio = 30f;
    public float loudnessMini = 1f;

    [HideInInspector]
    public bool isPlaying = true;

    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;
    int trianglesIndex = 0;
    AudioSource audioSource;
    float currentUpdateTime = 0f;
    float[] prevSamples;
    float[] samples;

    private void Awake()
    {
#if UNITY_EDITOR
        UnityEditor.EditorWindow.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
#endif
        mesh = GetComponent<MeshFilter>().mesh;
        audioSource = GetComponent<AudioSource>();
        samples = new float[sampleDataLength];
        prevSamples = new float[sampleDataLength];
    }

    void Update()
    {
        if (isPlaying)
        {
            currentUpdateTime += Time.deltaTime;
            vertices = new List<Vector3>();
            triangles = new List<int>();
            trianglesIndex = 0;

            if (currentUpdateTime >= updateStep)
            {
                currentUpdateTime = 0f;
                float[] spectrum = new float[sampleDataLength];

                audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

                for (int i = 1; i < spectrum.Length / spectrumDivider - 1; i++)
                {
                    prevSamples[i] = samples[i];
                    samples[i] = spectrum[i] * loudnessHeight;
                }
            }

            for (int i = 1; i < samples.Length / spectrumDivider - 1; i++)
            {
                float loudness = Mathf.Lerp(prevSamples[i], samples[i], currentUpdateTime * lerpRatio);
                if (loudness >= loudnessMini)
                {
                    for (int j = 0; j < arcsCount; j++)
                    {
                        GenerateMesh(Vector3.forward * radius * 2 * i * Mathf.Cos(2f * Mathf.PI / arcsCount * j) + Vector3.left * radius * 2 * i * Mathf.Sin(2f * Mathf.PI / arcsCount * j), loudness);
                    }
                }
            }

            mesh.Clear();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
        }
    }

    void GenerateMesh(Vector3 startPosition, float heightRatio)
    {
        int trianglesIndexOffset = trianglesIndex++;
        float radiusRatio = Mathf.Max(radius, radius * heightRatio / loudnessRadius);

        vertices.Add(startPosition + new Vector3(0f, height * heightRatio, 0f));
        vertices.Add(startPosition + new Vector3(0f, 0f, radiusRatio));

        for (int i = 0; i < edgesCount; i++)
        {
            float xRadius = radiusRatio * Mathf.Sin(2f * Mathf.PI / edgesCount * (trianglesIndex - trianglesIndexOffset));
            float zRadius = radiusRatio * Mathf.Cos(2f * Mathf.PI / edgesCount * (trianglesIndex - trianglesIndexOffset));            
            vertices.Add(startPosition + new Vector3(xRadius, 0f, zRadius));

            triangles.Add(trianglesIndex);
            triangles.Add(++trianglesIndex);
            triangles.Add(trianglesIndexOffset);
        }
        trianglesIndex++;
    }
}
