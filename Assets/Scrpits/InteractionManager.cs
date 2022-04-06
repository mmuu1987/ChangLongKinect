using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using com.rfilkov.components;
using com.rfilkov.kinect;
using UnityEngine;
using UnityEngine.Video;

public class InteractionManager : MonoBehaviour
{

    public static InteractionManager Instance;


    public Configure Configure;


    /// <summary>
    /// 是否是第一次识别
    /// </summary>
    private bool _isFirstDiscriminate = false;


    public bool IsUseStandby = false;

    private Coroutine _coroutine;


    public VideoPlayer VideoPlayer;

    public VideoPlayer VideoPlayMask;

    

    private ulong _curId;

    private string path1;

    private string path2;

    private string path3_1;

    private string path3_2;

    private string path3_3;

    private string path4_1;

    private string path4_2;

    private string path4_3;

    private string path5_1;

    private string path5_2;

    private string path5_3;

    private string path5_4;

    private string path5_5;

    private string path6_1;

    private string path6_2;

    private string path6_3;

    private string path6_4;

    private string path7_1;

    private string path7_2;

    private string path7_3;

    private string path7_4;

    private string path7_5;




    public float StandbyTime = 7f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            throw new UnityException("已经设置了单例");
        }
        if (Configure == null)
        {
            LoadConfigure();
        }
    }

    private void Start()
    {

        path1 = Application.streamingAssetsPath + "/01屏保影片/01海象屏保";

        path2 = Application.streamingAssetsPath + "/02暴风雪/02暴风雪";

        path3_1 = Application.streamingAssetsPath + "/03手放入桶中-喂食/03-01机器人讲解";

        path3_2 = Application.streamingAssetsPath + "/03手放入桶中-喂食/03-02机器人观察状态+水桶出现";

        path3_3 = Application.streamingAssetsPath + "/03手放入桶中-喂食/03-03机器人观察状态+吃鱼动作";

        path4_1 = Application.streamingAssetsPath + "/04拍掌翻身/04-01海象上岸";

        path4_1 = Application.streamingAssetsPath + "/04拍掌翻身/04-02海象待命状态";

        path4_1 = Application.streamingAssetsPath + "/04拍掌翻身/04-03海象翻身状态";

        path5_1 = Application.streamingAssetsPath + "/05拍掌翻身+抚摩/05-01机器人讲解海象";

        path5_1 = Application.streamingAssetsPath + "/05拍掌翻身+抚摩/05-02海象待命状态";

        path5_1 = Application.streamingAssetsPath + "/05拍掌翻身+抚摩/05-03海象翻身状态";

        path5_1 = Application.streamingAssetsPath + "/05拍掌翻身+抚摩/05-04海象待命状态";

        path5_1 = Application.streamingAssetsPath + "/05拍掌翻身+抚摩/05-05海象双手拍肚皮";

        path6_1 = Application.streamingAssetsPath + "/06张嘴/06-01机器人讲解海象";

        path6_2 = Application.streamingAssetsPath + "/06张嘴/06-02海象上岸";

        path6_3 = Application.streamingAssetsPath + "/06张嘴/06-03海象待命";

        path6_4 = Application.streamingAssetsPath + "/06张嘴/06-04海象张嘴";

        path7_1 = Application.streamingAssetsPath + "/07来回游动/07-01机器人讲解海象";

        path7_2 = Application.streamingAssetsPath + "/07来回游动/07-02海象待命";

        path7_3 = Application.streamingAssetsPath + "/07来回游动/07-03海象划入水中";

        path7_4 = Application.streamingAssetsPath + "/07来回游动/07-04海象20秒欢呼";

        path7_5 = Application.streamingAssetsPath + "/07来回游动/07-05海象30秒ending";

        KinectManager.Instance.userManager.OnUserAdded.AddListener(AddingUserEvent); 
        KinectManager.Instance.userManager.OnUserRemoved.AddListener(RemoveUserEvent);
    }

    public void PlayVideo(string path)
    {
        VideoPlayer.url = path+".mp4";

        VideoPlayMask.url = path + "TD.mp4";

        VideoPlayMask.Play();
        VideoPlayer.Play();
    }

    private void AddingUserEvent(ulong obj,int id)
    {
        bool isFirstDiscriminate = this._isFirstDiscriminate;
        if (isFirstDiscriminate)
        {
            base.StartCoroutine(this.CanceStandby(new Action(this.StartComputeStandby)));
            this._isFirstDiscriminate = false;
            Debug.Log("待机");
        }
        else
        {
          
            //Debug.Log("识别到人物");
           // this.userBodyBlender.ChangeBackGround(-1);
            this.StopComputeStandby();
            //this.CheckBodyHeight();
           
          

            this._curId = obj;

        }
    }


    private void RemoveUserEvent(ulong obj, int id)
    {
       
     
        this.StartComputeStandby();

        
       
        bool flag = this._curId == obj;
        if (flag)
        {
            this._curId = 0;
        }
    }


    /// <summary>
    /// 取消待机
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    private IEnumerator CanceStandby(Action action)
    {
        yield return new WaitForEndOfFrame();
       
        KinectManager.Instance.ClearKinectUsers();
        this.StopComputeStandby();
        KinectManager.Instance.playerCalibrationPose = GestureType.ObliqueHand;//ObliqueHand

        bool flag = action != null;
        if (flag)
        {
            action();
        }
        yield break;
    }




    private void StartComputeStandby()
    {
        bool flag = !this.IsUseStandby;
        if (!flag)
        {
            bool flag2 = this._coroutine != null;
            if (flag2)
            {
                base.StopCoroutine(this._coroutine);
            }
            this._coroutine = base.StartCoroutine(GlobalSettings.WaitTime(this.StandbyTime, new Action(this.StartStandby)));
        }
    }

    private void StopComputeStandby()
    {
        bool flag = this._coroutine != null;
        if (flag)
        {
            base.StopCoroutine(this._coroutine);
        }
    }

    /// <summary>
    /// 立即进入待机
    /// </summary>
    private void StartStandby()
    {
      
        KinectManager.Instance.playerCalibrationPose = GestureType.None;
        KinectManager.Instance.ClearKinectUsers();
        this._isFirstDiscriminate = true;
    }

    /// <summary>
    /// 加载配置
    /// </summary>
    private void LoadConfigure()
    {
        string path = Application.streamingAssetsPath + "/Configure.json";

        if (!File.Exists(path))
        {
            CreatConfigure(path);
        }
        else
        {
            byte[] bytes = File.ReadAllBytes(path);

            string str = Encoding.Default.GetString(bytes);

            Configure = JsonUtility.FromJson<Configure>(str);

            if (Configure != null)
            {
                Debug.Log("加载json数据成功");
            }
        }
    }

    private void CreatConfigure(string path)
    {
        Configure = new Configure();
        Configure.IsLoadCompleted = false;
        Configure.StandBy = 300f;
        Configure.StayCamTime = 3f;
        Configure.UnitySize = 512;
        Configure.IsShowFPS = false;
        SaveConfiugre(path, true);
    }
    /// <summary>
    /// 保存配置
    /// </summary>
    private void SaveConfiugre(string path, bool isFirst)
    {


        string str = JsonUtility.ToJson(Configure);

        byte[] bytes = Encoding.Default.GetBytes(str);

        if (isFirst)
        {
            if (!File.Exists(path))
            {
                File.WriteAllBytes(path, bytes);

                Debug.Log("创建json数据成功");
            }
            else
            {
                throw new UnityException("已经有了json文件");

            }
        }
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0f, 0f, 100f, 100f), "test"))
        {
            PlayVideo(path1);
        }
    }
#endif


}
