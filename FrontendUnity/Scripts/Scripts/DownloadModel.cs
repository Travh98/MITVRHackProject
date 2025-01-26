using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System;

public class SphereFeedback : MonoBehaviour
{
    // Public으로 선언한 Sphere 오브젝트
    public GameObject feedbackSphere;

    private string currentKeyword = "smartphone"; // 고정 텍스트 값
    private bool modelDownloaded = false;

    void Start()
    {
        // 앱 실행 시 다운로드 및 상태 피드백 흐름 시작
        StartCoroutine(DownloadAndPlaceModel());
    }

    private IEnumerator DownloadAndPlaceModel()
    {
        // 진행 중: Sphere 색상을 노란색으로 설정
        SetSphereColor(Color.yellow);

        // Sketchfab API 검색 실행
        yield return StartCoroutine(PerformSketchfabSearch());

        // 다운로드 성공 여부에 따라 색상 변경
        if (modelDownloaded)
        {
            // 성공: Sphere 색상을 초록색으로 설정
            SetSphereColor(Color.green);
        }
        else
        {
            // 실패: Sphere 색상을 빨간색으로 설정
            Debug.LogError("Model download failed or an error occurred.");
            SetSphereColor(Color.red); // 문제 발생
        }
    }

    private IEnumerator PerformSketchfabSearch()
    {
        if (string.IsNullOrEmpty(currentKeyword))
        {
            Debug.LogError("Keyword is empty!");
            yield break;
        }

        string url = $"https://api.sketchfab.com/v3/search?type=models&q={Uri.EscapeUriString(currentKeyword)}&animated=false&downloadable=true";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", "Token 2136b597e7e449eab7996eaa922a5b62");
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {www.error}");
            }
            else
            {
                string jsonResult = www.downloadHandler.text;
                SketchfabSearchResults searchResults = JsonUtility.FromJson<SketchfabSearchResults>(jsonResult);

                if (searchResults.results != null && searchResults.results.Length > 0)
                {
                    string modelUid = searchResults.results[0].uid; // 첫 번째 검색 결과의 UID
                    yield return StartCoroutine(GetModelDetails(modelUid));
                }
                else
                {
                    Debug.Log("No downloadable models found.");
                }
            }
        }
    }

    private IEnumerator GetModelDetails(string UID)
    {
        string downloadUrl = $"https://api.sketchfab.com/v3/models/{UID}/download";

        using (UnityWebRequest www = UnityWebRequest.Get(downloadUrl))
        {
            www.SetRequestHeader("Authorization", "Token 2136b597e7e449eab7996eaa922a5b62");
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error fetching details: {www.error}");
            }
            else
            {
                string jsonResult = www.downloadHandler.text;
                SketchfabDownloadData downloadData = JsonUtility.FromJson<SketchfabDownloadData>(jsonResult);

                if (downloadData.gltf != null && !string.IsNullOrEmpty(downloadData.gltf.url))
                {
                    yield return StartCoroutine(DownloadModelFile(downloadData.gltf.url));
                }
                else
                {
                    Debug.Log("No downloadable file URL found.");
                }
            }
        }
    }

    private IEnumerator DownloadModelFile(string url)
    {
        Debug.Log($"Downloading model from: {url}");

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"File Download Error: {www.error}");
                yield break;
            }

            string downloadedModelPath = Path.Combine(Application.persistentDataPath, "model.zip");
            File.WriteAllBytes(downloadedModelPath, www.downloadHandler.data);
            Debug.Log($"Model downloaded and saved to: {downloadedModelPath}");

            modelDownloaded = true; // 다운로드 성공
        }
    }

    private void SetSphereColor(Color color)
    {
        if (feedbackSphere != null)
        {
            Renderer sphereRenderer = feedbackSphere.GetComponent<Renderer>();
            if (sphereRenderer != null)
            {
                sphereRenderer.material.color = color;
            }
        }
        else
        {
            Debug.LogError("Feedback Sphere is not assigned!");
        }
    }

    [System.Serializable]
    private class SketchfabSearchResults
    {
        public SketchfabResult[] results;
    }

    [System.Serializable]
    private class SketchfabResult
    {
        public string uid;
    }

    [System.Serializable]
    private class SketchfabDownloadData
    {
        public Gltf gltf;
    }

    [System.Serializable]
    private class Gltf
    {
        public string url;
    }
}