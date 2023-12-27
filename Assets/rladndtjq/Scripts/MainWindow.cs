using System;
using System.Device.Location;
using System.IO;
using System.Net;
using System.Net.Http;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public enum WeatherType { Sun, Rain, Snow, Cloud }
public enum DaySeason { Dawn, Morning, Day, Eve, Night }

public class MainWindow : MonoBehaviour
{
    [Header("Weather")]
    [SerializeField] private Sprite[] weatherTypeIconSprites;
    [SerializeField] private Sprite[] weatherDaySeasonBackgroundSprites;
    [SerializeField] private Image daySeasonImage;
    [SerializeField] private TextMeshProUGUI temperatureText;
    [SerializeField] private Image weatherTypeImage;
    [SerializeField] private TextMeshProUGUI informationText;
    [Header("Launch")]
    [SerializeField] private TextMeshProUGUI todayLaunchText;
    [SerializeField] private Image launchImage;

    private void Start()
    {
        File.WriteAllText(Application.dataPath + "/Data2.json", GetCurrentWeather());

        // var result = File.ReadAllText(Application.dataPath + "/Data.json");
        // var obj = JObject.Parse(result);
        // Debug.Log(obj["response"]["body"]["items"]["item"][0]["category"].ToString());
    }

    private string GetCurrentWeather()
    {
        string url = "http://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getVilageFcst"; // URL
        url += "?ServiceKey=" + "tdLzrK9pFzayKxbqDcpLWFDMeBhXrBS56d%2BskBrrFLC%2FWeQqFEHI2mdOS8MBtQPbiw4my%2FWyczrsg2O8VxXCNg%3D%3D"; // Service Key
        url += "&pageNo=1";
        url += "&numOfRows=1000";
        url += "&dataType=JSON";
        url += "&base_date=20231226";
        url += "&base_time=1200";
        url += "&nx=37";
        url += "&ny=127";

        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";

        string results = string.Empty;
        HttpWebResponse response;
        using (response = request.GetResponse() as HttpWebResponse)
        {
            StreamReader reader = new StreamReader(response.GetResponseStream());
            results = reader.ReadToEnd();
        }

        return results;
    }
}