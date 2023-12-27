using System;
using System.IO;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.Linq;
using Cysharp.Threading.Tasks;

public enum DaySeason { Dawn, Morning, Day, Eve, Night }

[Serializable]
public struct Launch
{
    public string date;
    public string dish;
    public string material;
    public string calorie;
}

[Serializable]
public struct Weather
{
    //기온
    public float T1H;
    //풍속
    public float WSD;
    //습도
    public float REH;
}

[Serializable]
public struct Forecast
{
    //일 최저기온
    public float TMN;
    //일 최고기온
    public float TMX;
}

public class MainWindow : MonoBehaviour
{
    [SerializeField] private RectTransform[] layoutGroups;
    [Header("Weather")]
    [SerializeField] private Sprite[] weatherDaySeasonBackgroundSprites;
    [SerializeField] private Image daySeasonImage;
    [SerializeField] private TextMeshProUGUI temperatureText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI informationText;
    [Header("Launch")]
    [SerializeField] private TextMeshProUGUI todayLaunchText;

    private int lastUpdateHour;
    private int lastUpdateDay;

    private void Start()
    {
        InitializeData().Forget();
    }

    private async UniTaskVoid InitializeData()
    {
        var json = await RequestJsonNEIS(DateTime.Now);

        var now = DateTime.Now;
        if (lastUpdateHour != now.Hour)
        {
            lastUpdateHour = now.Hour;
            PlayerPrefs.SetString("weather", await GetCurrentWeather(now));
        }

        if (lastUpdateDay != now.Day)
        {
            lastUpdateDay = now.Day;
            PlayerPrefs.SetString("forecast", await GetForecast(now));
            PlayerPrefs.SetString("launch", await GetLaunch(now));
        }

        PlayerPrefs.Save();
        var weather = SaveLoad.Load<Weather>("weather");
        var forecast = SaveLoad.Load<Forecast>("forecast");
        var launch = SaveLoad.Load<Launch>("launch");

        temperatureText.text = $"{weather.T1H} ℃";
        dateText.text = $"{now:yyyy-MM-dd}\n{DayOfWeekToKorean(now.DayOfWeek)}";
        informationText.text = $"{forecast.TMN}℃/{forecast.TMX}℃ \n {weather.REH}%\n{weather.WSD}㎧";
        todayLaunchText.text = $"{launch.date}\n\n{launch.dish}\n\n{launch.material}\n\n{launch.calorie}";

        foreach (var rect in layoutGroups)
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    private async UniTask<string> GetForecast(DateTime now)
    {
        var jobj = JObject.Parse(await RequestJsonKMA(true, now));
        var todayInfo =
            jobj["response"]["body"]["items"]["item"].
            Children().
            Where(token => token["fcstDate"].ToString() == DateTime.Now.ToString("yyyyMMdd")).
            Where(token => token["category"].ToString() is "TMX" or "TMN").
            ToArray();

        var forecast = new Forecast();
        foreach (var token in todayInfo)
        {
            switch (token["category"].ToString())
            {
                case "TMX": forecast.TMX = (float)token["fcstValue"]; break;
                case "TMN": forecast.TMN = (float)token["fcstValue"]; break;
            }
        }

        return JsonUtility.ToJson(forecast);
    }

    private async UniTask<string> GetCurrentWeather(DateTime now)
    {
        var jobj = JObject.Parse(await RequestJsonKMA(false, now));
        var todayInfo =
            jobj["response"]["body"]["items"]["item"].
            Children().
            Where(token => token["category"].ToString() is "T1H" or "WSD" or "REH").
            ToArray();

        var weather = new Weather();
        foreach (var token in todayInfo)
        {
            switch (token["category"].ToString())
            {
                case "T1H": weather.T1H = (float)token["obsrValue"]; break;
                case "WSD": weather.WSD = (float)token["obsrValue"]; break;
                case "REH": weather.REH = (float)token["obsrValue"]; break;
            }
        }

        return JsonUtility.ToJson(weather);
    }

    private async UniTask<string> RequestJsonKMA(bool isForecast, DateTime now)
    {
        Debug.Log(now);
        Debug.Log($"isForecast : {isForecast}");

        string url =
            isForecast ?
            "http://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getVilageFcst" :
            "http://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getUltraSrtNcst";

        url += "?ServiceKey=tdLzrK9pFzayKxbqDcpLWFDMeBhXrBS56d%2BskBrrFLC%2FWeQqFEHI2mdOS8MBtQPbiw4my%2FWyczrsg2O8VxXCNg%3D%3D"; // Service Key
        url += "&pageNo=1";
        url += "&numOfRows=1000";
        url += "&dataType=JSON";

        var date = "&base_date=" +
            (isForecast ?
            $"{now:yyyyMM}{now.AddDays(-1).Day:00}" :
            now.ToString("yyyyMMdd"));
        Debug.Log(date);
        url += date;

        string basetime = $"&base_time=";
        if (isForecast)
        {
            basetime += "2300";
        }
        else
        {
            if (now.Minute > 40)
                basetime += $"{now.Hour:00}00";
            else
                basetime += $"{now.AddHours(-1).Hour:00}00";
        }
        Debug.Log(basetime);
        url += basetime;

        url += "&nx=60";
        url += "&ny=127";

        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";

        string results = string.Empty;
        HttpWebResponse response;
        using (response = await request.GetResponseAsync() as HttpWebResponse)
        {
            StreamReader reader = new StreamReader(response.GetResponseStream());
            results = reader.ReadToEnd();
        }

        File.WriteAllText(Application.dataPath + (isForecast ? "/forecast-all.json" : "/weather-all.json"), results);

        return results;
    }

    private async UniTask<string> GetLaunch(DateTime now)
    {
        var jobj = JObject.Parse(await RequestJsonNEIS(now));
        var data = jobj["mealServiceDietInfo"][1]["row"][0];
        var launch = new Launch();
        launch.date = $"{now:yyyy-MM-dd} {DayOfWeekToKorean(now.DayOfWeek)}";
        launch.dish = data["DDISH_NM"].ToString().Replace("<br/>", "\n");
        launch.material = data["ORPLC_INFO"].ToString().Replace("<br/>", "\n");
        launch.calorie = data["CAL_INFO"].ToString();

        return JsonUtility.ToJson(launch);
    }

    private string DayOfWeekToKorean(DayOfWeek dayOfWeek)
    {
        switch (dayOfWeek)
        {
            case DayOfWeek.Sunday: return "일요일";
            case DayOfWeek.Monday: return "월요일";
            case DayOfWeek.Tuesday: return "화요일";
            case DayOfWeek.Wednesday: return "수요일";
            case DayOfWeek.Thursday: return "목요일";
            case DayOfWeek.Friday: return "금요일";
            case DayOfWeek.Saturday: return "토요일";
            default: return "";
        }
    }

    private async UniTask<string> RequestJsonNEIS(DateTime now)
    {
        string url = "https://open.neis.go.kr/hub/mealServiceDietInfo";
        url += "?KEY=290e9aa090394e8abfb1e4551badd282";
        url += "&Type=json";
        url += "&pIndex=1";
        url += "&pSize=30";
        url += "&ATPT_OFCDC_SC_CODE=B10";
        url += "&SD_SCHUL_CODE=7010572";
        url += "&MLSV_YMD=" + now.ToString("yyyyMMdd");

        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";

        string results = string.Empty;
        HttpWebResponse response;
        using (response = await request.GetResponseAsync() as HttpWebResponse)
        {
            StreamReader reader = new StreamReader(response.GetResponseStream());
            results = reader.ReadToEnd();
        }

        File.WriteAllText(Application.dataPath + "/neis-all.json", results);

        return results;
    }
}