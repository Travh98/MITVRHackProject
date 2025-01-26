using UnityEngine;

public class ScriptScheduler : MonoBehaviour
{
    // 외부 스크립트를 Unity Inspector에서 연결할 수 있도록 설정
    public MonoBehaviour externalScript1; // 첫 작업을 실행할 스크립트
    public MonoBehaviour externalScript2; // 반복 작업을 실행할 스크립트

    public float initialDelay = 3f; // 첫 실행 지연 시간
    public float repeatInterval = 3f; // 반복 실행 간격

    void Start()
    {
        // 3초 후 첫 스크립트 실행
        Invoke("ExecuteFirstScript", initialDelay);

        // 이후 3초마다 반복 스크립트 실행
        InvokeRepeating("ExecuteRepeatingScript", initialDelay + repeatInterval, repeatInterval);
    }

    void ExecuteFirstScript()
    {
        if (externalScript1 != null)
        {
            externalScript1.Invoke("ExecuteFirstTask", 0f); // 외부 스크립트의 "ExecuteFirstTask" 메서드 실행
        }
        else
        {
            Debug.LogError("ExternalScript1 is not assigned!");
        }
    }

    void ExecuteRepeatingScript()
    {
        if (externalScript2 != null)
        {
            externalScript2.Invoke("ExecuteRepeatingTask", 0f); // 외부 스크립트의 "ExecuteRepeatingTask" 메서드 실행
        }
        else
        {
            Debug.LogError("ExternalScript2 is not assigned!");
        }
    }
}