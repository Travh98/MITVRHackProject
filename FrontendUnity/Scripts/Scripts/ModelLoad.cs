

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.IO.Compression;
using GLTFast;

public class ModelDownloaderAndLoader : MonoBehaviour
{
    public string serverUrl = "http://10.10.24.212:3000/get-model"; // 다운로드 링크를 제공하는 서버 엔드포인트
    private string extractPath;

    void Start()
    {
        extractPath = Path.Combine(Application.persistentDataPath, "ExtractedModel");
        StartCoroutine(GetDownloadLink());
    }

    IEnumerator GetDownloadLink()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(serverUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error fetching download link: " + www.error);
            }
            else
            {
                string downloadLink = www.downloadHandler.text;
                StartCoroutine(DownloadZipFile(downloadLink));
            }
        }
    }

    IEnumerator DownloadZipFile(string downloadLink)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(downloadLink))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error downloading ZIP file: " + www.error);
            }
            else
            {
                string zipPath = Path.Combine(Application.persistentDataPath, "downloaded.zip");
                File.WriteAllBytes(zipPath, www.downloadHandler.data);
                ExtractAndLoadModel(zipPath);
            }
        }
    }

    void ExtractAndLoadModel(string zipPath)
    {
        if (Directory.Exists(extractPath))
        {
            Directory.Delete(extractPath, true);
        }
        Directory.CreateDirectory(extractPath);

        ZipFile.ExtractToDirectory(zipPath, extractPath);

        string gltfPath = Path.Combine(extractPath, "scene.gltf");
        if (File.Exists(gltfPath))
        {
            LoadGLTFModel(gltfPath);
        }
        else
        {
            Debug.LogError("GLTF file not found in the extracted directory");
        }

        // 다운로드한 ZIP 파일 삭제 (선택사항)
        File.Delete(zipPath);
    }

    async void LoadGLTFModel(string gltfPath)
    {
        var gltf = new GltfImport();
        bool success = await gltf.Load(gltfPath);

        if (success)
        {
            var gameObject = new GameObject("LoadedModel");
            success = await gltf.InstantiateMainSceneAsync(gameObject.transform);

            if (success)
            {
                Debug.Log("Model loaded successfully");
                // 필요한 경우 모델의 위치, 회전, 크기 조정
                gameObject.transform.position = Vector3.zero;
                gameObject.transform.rotation = Quaternion.identity;
                gameObject.transform.localScale = Vector3.one;
            }
            else
            {
                Debug.LogError("Failed to instantiate GLTF scene");
            }
        }
        else
        {
            Debug.LogError("Failed to load GLTF");
        }
    }
}
