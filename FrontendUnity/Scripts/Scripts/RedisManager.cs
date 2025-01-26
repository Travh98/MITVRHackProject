// using UnityEngine;
// using StackExchange.Redis;
// using System.Collections;

// public class RedisManager : MonoBehaviour
// {
//     private static ConnectionMultiplexer redis;
//     private static IDatabase db;
//     public string redisServerAddress = "your_redis_server:6379";
//     public string redisKey = "your_key";
//     public float updateInterval = 5f;

//     void Start()
//     {
//         ConnectToRedis();
//         StartCoroutine(FetchDataRoutine());
//     }

//     private void ConnectToRedis()
//     {
//         try
//         {
//             redis = ConnectionMultiplexer.Connect(redisServerAddress);
//             db = redis.GetDatabase();
//             Debug.Log("Connected to Redis server.");
//         }
//         catch (System.Exception e)
//         {
//             Debug.LogError($"Failed to connect to Redis: {e.Message}");
//         }
//     }

//     public string GetData(string key)
//     {
//         try
//         {
//             return db.StringGet(key);
//         }
//         catch (System.Exception e)
//         {
//             Debug.LogError($"Failed to get data from Redis: {e.Message}");
//             return null;
//         }
//     }

//     private IEnumerator FetchDataRoutine()
//     {
//         while (true)
//         {
//             string data = GetData(redisKey);
//             if (data != null)
//             {
//                 Debug.Log($"Received data: {data}");
//                 // 여기에 데이터 처리 로직을 추가하세요
//             }
//             yield return new WaitForSeconds(updateInterval);
//         }
//     }

//     void OnApplicationPause(bool pauseStatus)
//     {
//         if (!pauseStatus)
//         {
//             if (redis == null || !redis.IsConnected)
//             {
//                 ConnectToRedis();
//             }
//         }
//     }

//     void OnApplicationQuit()
//     {
//         if (redis != null)
//         {
//             redis.Close();
//         }
//     }
// }
