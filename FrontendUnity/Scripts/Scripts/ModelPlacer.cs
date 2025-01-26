using UnityEngine;
using System.IO;
using System.IO.Compression;
using System.Collections;
using GLTFast;

public class ModelPlacer : MonoBehaviour
{
    private string zipPath = "/Users/jang/Library/Application Support/DefaultCompany/My project/smartphone.zip";
    private string extractPath = "/Users/jang/Library/Application Support/DefaultCompany/My project/extracted_smartphone";
    private string gltfPath;
    private Camera xrCamera;

    void Start()
    {
        xrCamera = Camera.main;
        if (xrCamera == null)
        {
            Debug.LogError("XR Camera not found!");
            return;
        }

        ExtractZipFile();
        gltfPath = Path.Combine(extractPath, "scene.gltf");

        if (File.Exists(gltfPath))
        {
            StartCoroutine(DisplayModelRoutine());
        }
        else
        {
            Debug.LogError("scene.gltf not found in the extracted files!");
        }
    }

    void ExtractZipFile()
    {
        if (File.Exists(zipPath))
        {
            ZipFile.ExtractToDirectory(zipPath, extractPath, true);
            Debug.Log("Zip file extracted successfully.");
        }
        else
        {
            Debug.LogError("Zip file not found at the specified path!");
        }
    }

    IEnumerator DisplayModelRoutine()
    {
        while (true)
        {
            yield return DisplayModel();
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator DisplayModel()
    {
        var gltf = new GltfImport();
        yield return gltf.Load(gltfPath);

        GameObject modelInstance = new GameObject("Model");
        gltf.InstantiateMainScene(modelInstance.transform);

        modelInstance.transform.position = xrCamera.transform.position + xrCamera.transform.forward * 1f;
        modelInstance.transform.rotation = xrCamera.transform.rotation;
        modelInstance.transform.localScale = Vector3.one * 0.1f;

        yield return new WaitForSeconds(2f);
        Destroy(modelInstance);
    }
}
