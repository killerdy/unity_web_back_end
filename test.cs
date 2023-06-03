using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public enum NetWorkState
{
    NETWORK_WARNING,//警告
    NETWORK_ERROE,//连接错误
    NETWORK_SUCCESS//连接成功
}
public class WebRequest: MonoBehaviour
{
    // Start is called before the first frame update
    //---------------------------------------------------------------------------------------------//3-1编号代码
    public static WebRequest _ins;

    readonly string form_data = "后端会给你这个路径";//如果没有表单，暂时可以不写
    readonly string raw = "后端会给你这个路径";//用来找到后端的数据的
    public readonly string url_login = "后端会给你这个路径";//登录按钮

    private void Awake()
    {
        _ins = this;
        DontDestroyOnLoad(this);
    }

    public void GetInfo_Login(string bindPhone, string password, Action<NetWorkState, string> callBack)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>//把账号密码设置成字典
        {
            { "bindPhone",bindPhone},
            { "password",password}
        };
        Dictionary<string, string> headers = SetHeader(raw);//加《写入请求头》的方法，用来查找你要找的数据，下面方法直接看
        //检查账号密码
        StartCoroutine(PostUrl(url_login, headers, dic, callBack));//去看3-2
    }
    private Dictionary<string, string> SetHeader(string ContentType)//这一步，按照官方的指示来
    {
        Dictionary<string, string> headers = new Dictionary<string, string>()
           {
          { "Content-Type", ContentType },//新版舍弃了www，改用UnityWebRequest，看下图。
          };
        return headers;
    }

    //---------------------------------------------------------------------------------------------//3-2编号代码   
    /// <summary>
    /// POST请求
    /// </summary>
    /// <param name="url">登录按钮路径（服务器给的路径）</param>
    /// <param name="dic">请求头（类似和服务器的交接暗号）</param>
    /// <param name="postDataDic">装有账号密码的字典 </param>
    /// <param name="callBack">成功时的回调函数</param>
    /// <returns></returns>
    public IEnumerator PostUrl(string url, Dictionary<string, string> dic, Dictionary<string, object> postDataDic, Action<NetWorkState02, string> callBack)
    {
        string type = dic["Content-Type"];
        string postData = "";
        if (form_data.Equals(type)) postData = GetStringFromDict(postDataDic);//把字典转换成表单，传给服务器
        //如果没有表单，那就把字典序列化，然后要传给服务器
        else if (raw.Equals(type)) postData = JsonConvert.SerializeObject(postDataDic);

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            if (!String.IsNullOrEmpty(postData))
            {
                byte[] postBytes = Encoding.UTF8.GetBytes(postData);
                www.uploadHandler = (UploadHandler)new UploadHandlerRaw(postBytes);
            }
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.useHttpContinue = false;
            www.timeout = 30;
            foreach (var item in dic)
            {
                www.SetRequestHeader(item.Key, item.Value);
            }
            yield return www.SendWebRequest();
            long mark = 200;
            if (www.isNetworkError)//发起请求失败 -- 弹出异常
            {
                Debug.Log("www.isNetworkError:" + www.error);
                //("请求失败", "请求失败!\n在请求时发生了一个错误,\n请重试\n错误信息:" + www.error);
                callBack(NetWorkState.NETWORK_ERROE, www.error);
            }
            else if (!mark.Equals(www.responseCode))//发起请求成功,但未能正确到达服务器
            {
                Debug.Log("warning:" + www.responseCode.ToString());
                //("请求失败", "请求异常!\n请重试.\n错误代码:" + www.responseCode);
                callBack(NetWorkState.NETWORK_WARNING, www.responseCode.ToString());//warning
            }
            else if (mark.Equals(www.responseCode))//本次请求成功 - 服务器有返回数据
            {
                Debug.Log("success:" + www.downloadHandler.text.ToString());
                string json = www.downloadHandler.text;

                callBack(NetWorkState.NETWORK_SUCCESS, json);//这个json就是返回的数据callBack，
                //去看2-2编码
            }
        }
    }
    /// <summary>
    /// 转换字典数据到表单
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private string GetStringFromDict(Dictionary<string, object> data)
    {
        StringBuilder builder = new StringBuilder();
        int i = 0;
        foreach (var item in data)
        {
            if (i > 0)
                builder.Append("&");
            builder.AppendFormat("{0}={1}", item.Key, item.Value);
            i++;
        }
        return builder.ToString();
    }

}
