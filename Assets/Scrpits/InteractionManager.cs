using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using com.rfilkov.components;
using com.rfilkov.kinect;
using UnityEngine;
using UnityEngine.Video;
using LitJson;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{

    public static InteractionManager Instance;


    public Configure Configure;


    /// <summary>
    /// 是否是从待机画面中识别人物
    /// </summary>
    private bool _isStandByDiscriminate = true;


    public bool IsUseStandby = false;

    private Coroutine _coroutine;


    public VideoPlayer VideoPlayer;

    public VideoPlayer VideoPlayMask;


    public MyMovingPoseManager myMovingPoseManager;

    public Image handImage;

    public Text InfoText;

    public RawImage tipImage;

    /// <summary>
    /// 提示手的位置的图片  
    /// </summary>
    public Image handTipImage;

    private KinectManager kinectManager = null;

    [Tooltip("Camera that will be used to overlay the 3D-objects over the background.")]
    public Camera foregroundCamera;

    private Rect backgroundRect = Rect.zero;

    [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
    public int playerIndex = 0;

    [Tooltip("Depth sensor index used for color camera overlay - 0 is the 1st one, 1 - the 2nd one, etc.")]
    public int sensorIndex = 0;

    [Tooltip("Scene object that will be used to represent the sensor's position and rotation in the scene.")]
    public Transform sensorTransform;

    private Vector3 _screenPos;


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


    private List<string> _pathList = new List<string>();

    public float StandbyTime = 7f;


    /// <summary>
    /// 播放完视频后，需要等待些许时间来让用户做姿势的时间
    /// </summary>
    public List<int> WaitIndex= new List<int>();
    /// <summary>
    /// 视频播放的定时器
    /// </summary>
    private Coroutine _videoCoroutine;

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

        Screen.SetResolution(2560,1440,true);
    }

    private void Start()
    {

        #region 视频路径
        path1 = Application.streamingAssetsPath + "/01屏保影片/01海象屏保";

        path2 = Application.streamingAssetsPath + "/02暴风雪/02暴风雪";

        path3_1 = Application.streamingAssetsPath + "/03手放入桶中-喂食/03-01机器人讲解";

        path3_2 = Application.streamingAssetsPath + "/03手放入桶中-喂食/03-02机器人观察状态+水桶出现";

        path3_3 = Application.streamingAssetsPath + "/03手放入桶中-喂食/03-03机器人观察状态+吃鱼动作";

        path4_1 = Application.streamingAssetsPath + "/04拍掌翻身/04-01海象上岸";

        path4_2 = Application.streamingAssetsPath + "/04拍掌翻身/04-02海象待命状态";

        path4_3 = Application.streamingAssetsPath + "/04拍掌翻身/04-03海象翻身状态";

        path5_1 = Application.streamingAssetsPath + "/05拍掌翻身+抚摩/05-01机器人讲解海象";

        path5_2 = Application.streamingAssetsPath + "/05拍掌翻身+抚摩/05-02海象待命状态";

        path5_3 = Application.streamingAssetsPath + "/05拍掌翻身+抚摩/05-03海象翻身状态";

        path5_4 = Application.streamingAssetsPath + "/05拍掌翻身+抚摩/05-04海象待命状态";

        path5_5 = Application.streamingAssetsPath + "/05拍掌翻身+抚摩/05-05海象双手拍肚皮";

        path6_1 = Application.streamingAssetsPath + "/06张嘴/06-01机器人讲解海象";

        path6_2 = Application.streamingAssetsPath + "/06张嘴/06-02海象上岸";

        path6_3 = Application.streamingAssetsPath + "/06张嘴/06-03海象待命";

        path6_4 = Application.streamingAssetsPath + "/06张嘴/06-04海象张嘴";

        path7_1 = Application.streamingAssetsPath + "/07来回游动/07-01机器人讲解海象";

        path7_2 = Application.streamingAssetsPath + "/07来回游动/07-02海象待命";

        path7_3 = Application.streamingAssetsPath + "/07来回游动/07-03海象划入水中";

        path7_4 = Application.streamingAssetsPath + "/07来回游动/07-04海象20秒欢呼";

        path7_5 = Application.streamingAssetsPath + "/07来回游动/07-05海象30秒ending";


        _pathList.Add(path1);
        _pathList.Add(path2);
        _pathList.Add(path3_1);
        _pathList.Add(path3_2);
        _pathList.Add(path3_3);
        _pathList.Add(path4_1);
        _pathList.Add(path4_2);
        _pathList.Add(path4_3);
        _pathList.Add(path5_1);
        _pathList.Add(path5_2);
        _pathList.Add(path5_3);
        _pathList.Add(path5_4);
        _pathList.Add(path5_5);
        _pathList.Add(path6_1);
        _pathList.Add(path6_2);
        _pathList.Add(path6_3);
        _pathList.Add(path6_4);
        _pathList.Add(path7_1);
        _pathList.Add(path7_2);
        _pathList.Add(path7_3);
        _pathList.Add(path7_4);
        _pathList.Add(path7_5);
     

        #endregion



        KinectManager.Instance.userManager.OnUserAdded.AddListener(AddingUserEvent); 
        KinectManager.Instance.userManager.OnUserRemoved.AddListener(RemoveUserEvent);

        myMovingPoseManager.DetectorEvent += MyMovingPoseManager_DetectorEvent;

        VideoPlayer.prepareCompleted += VideoPlayer_prepareCompleted;

        TCPUDPSocket.Instance.RecevieDataEvent += Instance_RecevieDataEvent;

        _curIndex = 0;
        PlayVideo(_pathList[_curIndex]);

        StandbyTime = (float)Configure.StandBy;

        myMovingPoseManager.Init((float)Configure.CheckTime,(int)Configure.CheckCount, (float)Configure.CheckPercent);

        kinectManager = KinectManager.Instance;

        tipImage.gameObject.SetActive(false);
        handTipImage.gameObject.SetActive(false);
    }

    private void Instance_RecevieDataEvent(string obj)
    {
       Debug.LogError("收到了中控发来的消息 : "+obj);

       switch (obj)
       {
            case "Skip":
                SkipState();
                break;
            case "StandBy":
                StartStandby();
                break;
            default:
                Debug.LogError("接收到不正确的指令");
                break;
       }

    }
    /// <summary>
    /// 跳过该状态，进入下一个状态，如果是待机视频的话
    /// </summary>
    public void SkipState()
    {


        base.StartCoroutine(this.CanceStandby(new Action(this.StartComputeStandby)));
        this._isStandByDiscriminate = false;
        Debug.Log("待机结束");

        //播放第二个待机界面，之后自动播放

        _curIndex = 2;
        PlayVideo(_pathList[_curIndex]);

      
    }

    /// <summary>
    /// 姿势识别成功的事件
    /// </summary>
    /// <param name="obj"></param>
    private void MyMovingPoseManager_DetectorEvent(string obj)
    {
       Debug.Log("识别姿势成功 "+obj);
       InfoText.text = "识别进入。";
       myMovingPoseManager.StopAnimator();
       StopAndNextPlay();
    }

    private void VideoPlayer_prepareCompleted(VideoPlayer source)
    { 
        
        if (_curIndex == 0)
        {

            source.isLooping = true;
            VideoPlayMask.isLooping = true;

            return;//待机视频不参与自动播放

        }
        source.isLooping = false;
        VideoPlayMask.isLooping = false;

        if (_videoCoroutine != null) StopCoroutine(_videoCoroutine);

        Debug.Log(VideoPlayer.length);

        float addTime = 0f;//给用户增加判断姿势的时间

        if (WaitIndex.Contains(_curIndex))
        {

            Debug.Log("等待的时间是 "+ Configure.WaitTime);
             addTime = (float)Configure.WaitTime;

        }

        handTipImage.gameObject.SetActive(false);
        tipImage.gameObject.SetActive(false);

        if (addTime > 0)//有增加时间，说明需要视频结束后判断手的位置或者识别用户姿势
        {

         
            if (_curIndex ==3)
            {

                StartHandPosCheck();
                handTipImage.gameObject.SetActive(true);
            }
          
            else if (_curIndex == 6)
            {
                myMovingPoseManager.PlayAnimator("paizhang");
                tipImage.gameObject.SetActive(true);
            }
            else if (_curIndex == 9)
            {
                myMovingPoseManager.PlayAnimator("paizhang");
                tipImage.gameObject.SetActive(true);
            }
            else if (_curIndex == 11)
            {
                StartHandPosCheck();
                handTipImage.gameObject.SetActive(true);
            }
            else if (_curIndex == 15)
            {
                myMovingPoseManager.PlayAnimator("taishou");
                tipImage.gameObject.SetActive(true);
            }

            else if (_curIndex == 18)
            {
                myMovingPoseManager.PlayAnimator("yaobai");
                tipImage.gameObject.SetActive(true);
            }
        }

        _videoCoroutine = StartCoroutine(GlobalSettings.WaitTime((float)VideoPlayer.length+ addTime, (() =>
        {

            InfoText.text = "自动进入";
            myMovingPoseManager.StopAnimator();//无论何种方式进入，都停止动画播放
           
            _isHandCheck = false;//暂停手势位置检测，因为时间到了

            _curIndex++;
          if (_curIndex >= _pathList.Count)//整个体验过程播放结束
          {
                StartStandby();
          }
          else
          {
              PlayVideo(_pathList[_curIndex]);
          }
         

        })));
    }

    /// <summary>
    /// 停止当前播放，并进入下一个视频
    /// 
    /// </summary>
    public void StopAndNextPlay()
    {   
        StopVideo();
        if (_videoCoroutine != null) StopCoroutine(_videoCoroutine);
        _curIndex++;
        PlayVideo(_pathList[_curIndex]);
    }

    public void PlayVideo(string path)
    {
        VideoPlayer.url = path+".mp4";

        VideoPlayMask.url = path + "TD.mp4";

        VideoPlayMask.Play();
        VideoPlayer.Play();
    }

    public void StopVideo()
    {
        VideoPlayMask.Stop();
        VideoPlayer.Stop();
    }

    private int _curIndex;
   
    private void AddingUserEvent(ulong obj,int id)
    {
        if (Configure.IsAwaysStandBy) return;
        bool isFirstDiscriminate = this._isStandByDiscriminate;

        Debug.Log("增加了用户");

        if (isFirstDiscriminate)//从待机中恢复
        {
            base.StartCoroutine(this.CanceStandby(new Action(this.StartComputeStandby)));
            this._isStandByDiscriminate = false;
            Debug.Log("待机结束");

            //播放第二个待机界面，之后自动播放

            _curIndex = 1;
            PlayVideo(_pathList[_curIndex]);


        }
        else
        {
          
            Debug.Log("识别到人物");
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




    private Vector2 GetPos(KinectInterop.JointType jointType)
    {
        Vector2 screenPos = Vector3.one * 1000;


        if (kinectManager && kinectManager.IsInitialized())
        {
            if (foregroundCamera)
            {
                // get the background rectangle (use the portrait background, if available)
                backgroundRect = foregroundCamera.pixelRect;
                PortraitBackground portraitBack = PortraitBackground.Instance;

                if (portraitBack && portraitBack.enabled)
                {
                    backgroundRect = portraitBack.GetBackgroundRect();
                }
            }

            // overlay the joint
            ulong userId = kinectManager.GetUserIdByIndex(playerIndex);

            int iJointIndex = (int)jointType;
            if (kinectManager.IsJointTracked(userId, iJointIndex))
            {
                Vector3 posJoint = foregroundCamera ?
                    kinectManager.GetJointPosColorOverlay(userId, iJointIndex, sensorIndex, foregroundCamera, backgroundRect) :
                    sensorTransform ? kinectManager.GetJointKinectPosition(userId, iJointIndex, true) :
                    kinectManager.GetJointPosition(userId, iJointIndex);

                if (sensorTransform)
                {
                    posJoint = sensorTransform.TransformPoint(posJoint);
                }

                if (posJoint != Vector3.zero )
                {

                    screenPos = this.foregroundCamera.WorldToScreenPoint(posJoint);

                 

                    

                }
            }
           

        }

        return screenPos;
    }

    private bool _isHandCheck = false;

    private Vector3 _targetPos;
    /// <summary>
    /// 开始手部位置的检测
    /// </summary>
    public void StartHandPosCheck()
    {

        Debug.Log("开始手势检测");
        _isHandCheck = true;

        Vector3 pos;

        if (_curIndex == 3)
        {
            pos = Configure.ScreenPosTarget3_02.ConvertVector3();
        }
        else
        {
            pos = Configure.ScreenPosTarget5_04.ConvertVector3();
        }

        Debug.Log("触摸屏幕的目标点为："+ pos);

       
        _targetPos = pos;

        

    }

    private void Update()
    {

        EnterTarget();
    }
    /// <summary>
    /// 手进入目标点的判断
    /// </summary>
    private void EnterTarget()
    {

        if (_isHandCheck)
        {

            Vector2 screenPos = GetPos(KinectInterop.JointType.HandLeft);

            handImage.rectTransform.anchoredPosition = screenPos;


            float d = Vector3.Distance(screenPos, _targetPos);

            if (d <= 100)
            {
                Debug.Log("手进入了范围");

                _isHandCheck = false;

                StopAndNextPlay();

                InfoText.text = "识别进入。";
            }

           
        }

       
        //if()
    }


    /// <summary>
    /// 取消待机
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    private IEnumerator CanceStandby(Action action)
    {
        yield return new WaitForEndOfFrame();
       
        //KinectManager.Instance.ClearKinectUsers();
        this.StopComputeStandby();
        //KinectManager.Instance.playerCalibrationPose = GestureType.ObliqueHand;//ObliqueHand

        bool flag = action != null;
        if (flag)
        {
            action();
        }
        yield break;
    }




    private void StartComputeStandby()
    {
      
        
            bool flag2 = this._coroutine != null;
            if (flag2)
            {
                base.StopCoroutine(this._coroutine);
            }
            this._coroutine = base.StartCoroutine(GlobalSettings.WaitTime(this.StandbyTime, new Action(this.StartStandby)));
        
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
        if(_videoCoroutine!=null)StopCoroutine(_videoCoroutine);
    
        PlayVideo(_pathList[0]);
        this._isStandByDiscriminate = true;
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

            Configure = JsonMapper.ToObject<Configure>(str);

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
        Configure.UDPReceivePort = 6000;
        SaveConfiugre(path, true);
    }
    /// <summary>
    /// 保存配置
    /// </summary>
    private void SaveConfiugre(string path, bool isFirst)
    {


        string str = JsonMapper.ToJson(Configure);

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
            SkipState();
        }

        if (GUI.Button(new Rect(100f, 0f, 100f, 100f), "test1"))
        {
            StartStandby();
        }

        if (GUI.Button(new Rect(200f, 0f, 100f, 100f), "test1"))
        {
            StopAndNextPlay();
        }
    }
#endif


}
