using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Networking;
using Object = System.Object;

namespace Top2.Common.Http
{
    public class HttpTools : MonoBehaviour
    {
        public static HttpTools Instance;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        public static void PrintTest()
        {
            Debug.Log("Top2.Common.Http.HttpTools, version 0.0.2");
        }

        public Coroutine Post(string url, string header, string body, string contentType, Action<string, string> callback)
        {
            return StartCoroutine(CoroutinePost(url, header, body, contentType, callback));
        }

        IEnumerator CoroutinePost(string url, string header, string body, string contentType,
            Action<string, string> callback)
        {
            // 暂时不判定传参对错
            var unityWebRequest = UnityWebRequest.Post(url, String.Empty);
            if (!string.IsNullOrEmpty(header))
            {
                var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(header);
                foreach (var pair in dictionary)
                {
                    unityWebRequest.SetRequestHeader(pair.Key, pair.Value);
                    Debug.Log(pair.Key + "=" + pair.Value);
                }
            }

            if (!string.IsNullOrEmpty(body) && "null" != body)
            {
                if ("application/json" == contentType)
                {
                    unityWebRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
                    unityWebRequest.uploadHandler.contentType = contentType; //"application/json";
                }
                else if ("multipart/form-data" == contentType)
                {
                    WWWForm form = new WWWForm();
                    var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
                    foreach (var pair in dictionary)
                    {
                        form.AddField(pair.Key, pair.Value);
                        Debug.Log(pair.Key + "=" + pair.Value);
                    }
                    unityWebRequest.uploadHandler = new UploadHandlerRaw(form.data);
                    unityWebRequest.uploadHandler.contentType = contentType; //"application/json";
                }

            }


            var operation = unityWebRequest.SendWebRequest();
            while (!operation.isDone)
            {
                yield return null;
            }

            var log =
                $"<color=#66FF00>==> url : {url}\n</color>" +
                $"<color=#66FF00>==> header : {header}\n</color>" +
                $"<color=#66FF00>==> body : {body}\n</color>" +
                $"<color=#66FF00>==> code : {operation.webRequest.responseCode.ToString()}\n</color>" +
                $"<color=#66FF00>==> text : {operation.webRequest.downloadHandler.text}\n</color>";
            Debug.Log(log);
            callback?.Invoke(operation.webRequest.responseCode.ToString(), operation.webRequest.downloadHandler.text);
            unityWebRequest.Dispose();
            yield return null;
        }
    }
}
