using System;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using GLTFast;


public class LoadModelFromStreamingAssets : MonoBehaviour
{
    private string zipFilePath;      // ZIP 파일 경로
    private string extractToPath;   // 압축 해제 경로
    private bool isModelLoaded = false;

    void Start()
    {
        // StreamingAssets 경로에서 ZIP 파일 경로 설정
        zipFilePath = Path.Combine(Application.streamingAssetsPath, "smartphone.zip");
        extractToPath = Path.Combine(Application.persistentDataPath, "Extracted");

        // 3초 후 모델 로드 실행
        Invoke(nameof(LoadModelFromZip), 3f); // 3초 후 LoadModelFromZip 실행
    }

    private async void LoadModelFromZip()
    {
        if (isModelLoaded)
        {
            Debug.LogWarning("Model is already loaded!");
            return;
        }

        if (!File.Exists(zipFilePath))
        {
            Debug.LogError($"ZIP file not found at: {zipFilePath}");
            return;
        }

        // 압축 해제
        ExtractZipFile(zipFilePath, extractToPath);

        // GLTF 파일 경로 설정
        string gltfFilePath = Path.Combine(extractToPath, "scene.gltf");

        if (File.Exists(gltfFilePath))
        {
            // glTFast를 사용하여 GLTF 파일 로드
            var gltfImport = new GltfImport();
            bool success = await gltfImport.Load(gltfFilePath);

            if (success)
            {
                // 모델 인스턴스화
                GameObject loadedModel = new GameObject("LoadedModel");
                await gltfImport.InstantiateMainSceneAsync(loadedModel.transform);

                // 모델 위치 설정 (0, 0, 0으로 배치)
                loadedModel.transform.position = new Vector3(0, 0, 0);

                Debug.Log("Model loaded successfully!");
                isModelLoaded = true;
            }
            else
            {
                Debug.LogError("Failed to load GLTF file!");
            }
        }
        else
        {
            Debug.LogError($"GLTF file not found at: {gltfFilePath}");
        }
    }

    private void ExtractZipFile(string zipFilePath, string destinationPath)
    {
        // 기존 폴더 삭제 후 재생성
        if (Directory.Exists(destinationPath))
        {
            Directory.Delete(destinationPath, true);
        }
        Directory.CreateDirectory(destinationPath);

        // 압축 해제
        try
        {
            ZipFile.ExtractToDirectory(zipFilePath, destinationPath);
            Debug.Log($"ZIP file extracted to: {destinationPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to extract ZIP file: {e.Message}");
        }
    }
}