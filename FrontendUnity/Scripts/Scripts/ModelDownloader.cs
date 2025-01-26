using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


public class ModelDownloader : MonoBehaviour
{
    // Google Sheets 설정
    static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
    static readonly string ApplicationName = "Unity Google Sheets Reader";
    static readonly string SpreadsheetId = "1bQpc-37znZBbo-Igy8Gy_2L79mOCif8n0W9ranl7i44";
    static readonly string Range = "A1";

    private string lastFetchedKeyword = "";
    public string currentKeyword = ""; // 현재 검색할 키워드
    public bool modelDownloaded = false; // 모델 다운로드 여부
    public string downloadedModelPath = ""; // 다운로드된 모델 경로

    // public XRController rightController;
    // public XRController leftController;

    void Start()
    {
        // DownloadAndExtractModelFlow 코루틴 실행
        StartCoroutine(DownloadAndExtractModelFlow());
    }
    void Update()
    {
        // if (leftController.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool leftPrimaryButtonPressed) && leftPrimaryButtonPressed)
        // {
        //     Debug.Log("A 버튼 (왼쪽 컨트롤러) 눌림");
        // }
    }

    private IEnumerator DownloadAndExtractModelFlow()
    {
        // Google Sheets에서 데이터 가져오기
        yield return StartCoroutine(FetchDataFromSheet((keyword) =>
        {
            currentKeyword = keyword;
            Debug.Log($"Fetched keyword: {currentKeyword}");
        }));

        // 새로운 키워드일 경우에만 다운로드 실행
        if (!string.IsNullOrEmpty(currentKeyword) && currentKeyword != lastFetchedKeyword)
        {
            lastFetchedKeyword = currentKeyword;
            modelDownloaded = false; // 이전 상태 초기화
            yield return StartCoroutine(PerformSketchfabSearch());
        }
        else
        {
            Debug.Log("No new keyword or keyword is empty.");
        }
    }

    private IEnumerator FetchDataFromSheet(System.Action<string> callback)
    {
        GoogleCredential credential;
        using (var stream = new FileStream(Path.Combine(Application.streamingAssetsPath, "k1t-448916-22f91d69871b.json"), FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
        }

        var service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });

        var request = service.Spreadsheets.Values.Get(SpreadsheetId, Range);
        var response = request.Execute();
        var values = response.Values;

        if (values != null && values.Count > 0 && values[0].Count > 0)
        {
            callback(values[0][0].ToString()); // A1 셀 데이터를 반환
        }
        else
        {
            Debug.Log("No data found in Google Sheets.");
            callback(null);
        }

        yield return null;
    }

    private IEnumerator PerformSketchfabSearch()
    {
        if (string.IsNullOrEmpty(currentKeyword))
        {
            Debug.Log("Keyword is empty!");
            yield break;
        }

        // Sketchfab API 검색 URL
        string url = $"https://api.sketchfab.com/v3/search?type=models&q={Uri.EscapeUriString(currentKeyword)}&animated=false&downloadable=true&archives_flavours=true";

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
                // Sketchfab 검색 결과 처리
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

            // 다운로드된 파일 저장
            downloadedModelPath = Path.Combine(Application.persistentDataPath, "model.zip");
            File.WriteAllBytes(downloadedModelPath, www.downloadHandler.data);
            Debug.Log($"Model downloaded and saved to: {downloadedModelPath}");

            // 압축 해제
            string extractPath = Path.Combine(Application.persistentDataPath, "ExtractedModel");
            System.IO.Compression.ZipFile.ExtractToDirectory(downloadedModelPath, extractPath);
            Debug.Log($"Model extracted to: {extractPath}");

            modelDownloaded = true;
        }
    }

    [Serializable]
    private class SketchfabSearchResults
    {
        public SketchfabResult[] results;
    }

    [Serializable]
    private class SketchfabResult
    {
        public string uid;
    }

    [Serializable]
    private class SketchfabDownloadData
    {
        public Gltf gltf;
    }

    [Serializable]
    private class Gltf
    {
        public string url;
    }
}