using System;
using StackExchange.Redis;
using UnityEngine;
using TMPro;

public class RedisConnector : MonoBehaviour
{
    public TextMeshPro textDisplay;
    private ConnectionMultiplexer redis;
    private IDatabase db;

    // Redis 서버 주소 (예: "127.0.0.1:6379")
    public string redisServerAddress = "192.168.186.156";
    public int redisPort = 6379;

    void Start()
    {
        ConnectToRedis();
        RetrieveData("Detection::yolonas::0"); // "your-key"는 Redis에서 가져올 키입니다.
    }

    void ConnectToRedis()
    {
        try
        {
            string configuration = $"{redisServerAddress}:{redisPort}";
            redis = ConnectionMultiplexer.Connect(configuration);
            db = redis.GetDatabase();
            textDisplay.text = "Redis 연결 성공!";
        }
        catch (Exception ex)
        {
            textDisplay.text = ex.Message;
        }
    }

    void RetrieveData(string key)
    {
        try
        {
            // Redis에서 데이터 가져오기
            string value = db.StringGet(key);
            Debug.Log($"키: {key}, 값: {value}");
            textDisplay.text = value;
        }
        catch (Exception ex)
        {
            textDisplay.text = ex.Message;
        }
    }

    private void OnApplicationQuit()
    {
        // Redis 연결 종료
        if (redis != null)
        {
            redis.Close();
            textDisplay.text = "Redis 연결 종료";
        }
    }
}