using System;
using System.Collections;
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
    public float cameraRadius = 75f;
    public float cameraAngle = 3f;

    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;
    int trianglesIndex = 0;

    AudioSource audioSource;
    public float updateStep = 20f;
    public int sampleDataLength = 8192;

    private float currentUpdateTime = 0f;

    //private float clipLoudness;
    //private float[] clipSampleData;
    private float[] prevSamples;
    private float[] samples;
    public int sampleDataLengthDisplayed;
    public float lerpRatio = 30f;
    public float loudnessMini = 1f;

    public Transform mainCameraTransform;

    private void Awake()
    {
#if UNITY_EDITOR
        UnityEditor.EditorWindow.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
#endif
        mesh = GetComponent<MeshFilter>().mesh;
        audioSource = GetComponent<AudioSource>();
        //clipSampleData = new float[sampleDataLength];
        samples = new float[sampleDataLength];
        prevSamples = new float[sampleDataLength];

        ///position
        ///x=-69.3
        ///y=6.6
        ///z=28.7
        ///rotation
        ///x=5.0
        ///y=112.5
        ///z=0
        mainCameraTransform.position = new Vector3(
            cameraRadius * Mathf.Cos(2f * Mathf.PI / arcsCount * cameraAngle + 2f * Mathf.PI / arcsCount / 2f),
            mainCameraTransform.position.y,
            cameraRadius * Mathf.Sin(2f * Mathf.PI / arcsCount * cameraAngle + 2f * Mathf.PI / arcsCount / 2f)
        );
        mainCameraTransform.LookAt(Vector3.zero);
    }

    void Update()
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
                //Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
                prevSamples[i] = samples[i];
                //clipLoudness = Mathf.Abs(Mathf.Log(spectrum[i]));
                samples[i] = spectrum[i] * loudnessHeight;
                //loudnessRatio = Mathf.Min(height / clipLoudness, loudnessRatio);
            }
        }

        for (int i = 1; i < samples.Length / spectrumDivider - 1; i++)
        {
            float loudness = Mathf.Lerp(prevSamples[i], samples[i], currentUpdateTime * lerpRatio);
            if (loudness >= loudnessMini)
            {
                //GenerateMesh(Vector3.left * radius * 2 * i, samples[i]);
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

    //void GetAudioAmplitude()
    //{
    //    currentUpdateTime += Time.deltaTime;
    //    if (currentUpdateTime >= updateStep)
    //    {
    //        currentUpdateTime = 0f;
    //        audioSource.clip.GetData(clipSampleData, audioSource.timeSamples); //I read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.
    //        clipLoudness = 0f;
    //        foreach (var sample in clipSampleData)
    //        {
    //            clipLoudness += Mathf.Abs(sample);
    //        }
    //        clipLoudness /= sampleDataLength; //clipLoudness is what you are looking for
    //        loudnessRatio = (height / clipLoudness <= loudnessRatio) ? height / clipLoudness : loudnessRatio;
    //        clipLoudness *= loudnessRatio;
    //        //Debug.Log("Loudness: " + clipLoudness);
    //    }
    //}

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
