using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EyesOpenessGetter : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(GetEar());
    }

    IEnumerator GetEar()
    {
        while (true)
        {

            UnityWebRequest www = UnityWebRequest.Get("http://localhost:13967/face_params");
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                print(www.error);
            }
            else
            {
                // Show results as text
                print(www.downloadHandler.text);

                // Or retrieve results as binary data
                byte[] results = www.downloadHandler.data;
            }

            // Wait for a short period before the next request.
            yield return new WaitForSeconds(0.1f);
        }
    }
}
