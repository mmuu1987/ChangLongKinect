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
    /// �Ƿ��ǴӴ���������ʶ������
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
    /// ��ʾ�ֵ�λ�õ�ͼƬ  
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
    /// ��������Ƶ����Ҫ�ȴ�Щ��ʱ�������û������Ƶ�ʱ��
    /// </summary>
    public List<int> WaitIndex= new List<int>();
    /// <summary>
    /// ��Ƶ���ŵĶ�ʱ��
    /// </summary>
    private Coroutine _videoCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            throw new UnityException("�Ѿ������˵���");
        }
        if (Configure == null)
        {
            LoadConfigure();
        }

        Screen.SetResolution(2560,1440,true);
    }

    private void Start()
    {

        #region ��Ƶ·��
        path1 = Application.streamingAssetsPath + "/01����ӰƬ/01��������";

        path2 = Application.streamingAssetsPath + "/02����ѩ/02����ѩ";

        path3_1 = Application.streamingAssetsPath + "/03�ַ���Ͱ��-ιʳ/03-01�����˽���";

        path3_2 = Application.streamingAssetsPath + "/03�ַ���Ͱ��-ιʳ/03-02�����˹۲�״̬+ˮͰ����";

        path3_3 = Application.streamingAssetsPath + "/03�ַ���Ͱ��-ιʳ/03-03�����˹۲�״̬+���㶯��";

        path4_1 = Application.streamingAssetsPath + "/04���Ʒ���/04-01�����ϰ�";

        path4_2 = Application.streamingAssetsPath + "/04���Ʒ���/04-02�������״̬";

        path4_3 = Application.streamingAssetsPath + "/04���Ʒ���/04-03������״̬";

        path5_1 = Application.streamingAssetsPath + "/05���Ʒ���+��Ħ/05-01�����˽��⺣��";

        path5_2 = Application.streamingAssetsPath + "/05���Ʒ���+��Ħ/05-02�������״̬";

        path5_3 = Application.streamingAssetsPath + "/05���Ʒ���+��Ħ/05-03������״̬";

        path5_4 = Application.streamingAssetsPath + "/05���Ʒ���+��Ħ/05-04�������״̬";

        path5_5 = Application.streamingAssetsPath + "/05���Ʒ���+��Ħ/05-05����˫���Ķ�Ƥ";

        path6_1 = Application.streamingAssetsPath + "/06����/06-01�����˽��⺣��";

        path6_2 = Application.streamingAssetsPath + "/06����/06-02�����ϰ�";

        path6_3 = Application.streamingAssetsPath + "/06����/06-03�������";

        path6_4 = Application.streamingAssetsPath + "/06����/06-04��������";

        path7_1 = Application.streamingAssetsPath + "/07�����ζ�/07-01�����˽��⺣��";

        path7_2 = Application.streamingAssetsPath + "/07�����ζ�/07-02�������";

        path7_3 = Application.streamingAssetsPath + "/07�����ζ�/07-03������ˮ��";

        path7_4 = Application.streamingAssetsPath + "/07�����ζ�/07-04����20�뻶��";

        path7_5 = Application.streamingAssetsPath + "/07�����ζ�/07-05����30��ending";


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
       Debug.LogError("�յ����пط�������Ϣ : "+obj);

       switch (obj)
       {
            case "Skip":
                SkipState();
                break;
            case "StandBy":
                StartStandby();
                break;
            default:
                Debug.LogError("���յ�����ȷ��ָ��");
                break;
       }

    }
    /// <summary>
    /// ������״̬��������һ��״̬������Ǵ�����Ƶ�Ļ�
    /// </summary>
    public void SkipState()
    {


        base.StartCoroutine(this.CanceStandby(new Action(this.StartComputeStandby)));
        this._isStandByDiscriminate = false;
        Debug.Log("��������");

        //���ŵڶ����������棬֮���Զ�����

        _curIndex = 2;
        PlayVideo(_pathList[_curIndex]);

      
    }

    /// <summary>
    /// ����ʶ��ɹ����¼�
    /// </summary>
    /// <param name="obj"></param>
    private void MyMovingPoseManager_DetectorEvent(string obj)
    {
       Debug.Log("ʶ�����Ƴɹ� "+obj);
       InfoText.text = "ʶ����롣";
       myMovingPoseManager.StopAnimator();
       StopAndNextPlay();
    }

    private void VideoPlayer_prepareCompleted(VideoPlayer source)
    { 
        
        if (_curIndex == 0)
        {

            source.isLooping = true;
            VideoPlayMask.isLooping = true;

            return;//������Ƶ�������Զ�����

        }
        source.isLooping = false;
        VideoPlayMask.isLooping = false;

        if (_videoCoroutine != null) StopCoroutine(_videoCoroutine);

        Debug.Log(VideoPlayer.length);

        float addTime = 0f;//���û������ж����Ƶ�ʱ��

        if (WaitIndex.Contains(_curIndex))
        {

            Debug.Log("�ȴ���ʱ���� "+ Configure.WaitTime);
             addTime = (float)Configure.WaitTime;

        }

        handTipImage.gameObject.SetActive(false);
        tipImage.gameObject.SetActive(false);

        if (addTime > 0)//������ʱ�䣬˵����Ҫ��Ƶ�������ж��ֵ�λ�û���ʶ���û�����
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

            InfoText.text = "�Զ�����";
            myMovingPoseManager.StopAnimator();//���ۺ��ַ�ʽ���룬��ֹͣ��������
           
            _isHandCheck = false;//��ͣ����λ�ü�⣬��Ϊʱ�䵽��

            _curIndex++;
          if (_curIndex >= _pathList.Count)//����������̲��Ž���
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
    /// ֹͣ��ǰ���ţ���������һ����Ƶ
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

        Debug.Log("�������û�");

        if (isFirstDiscriminate)//�Ӵ����лָ�
        {
            base.StartCoroutine(this.CanceStandby(new Action(this.StartComputeStandby)));
            this._isStandByDiscriminate = false;
            Debug.Log("��������");

            //���ŵڶ����������棬֮���Զ�����

            _curIndex = 1;
            PlayVideo(_pathList[_curIndex]);


        }
        else
        {
          
            Debug.Log("ʶ������");
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
    /// ��ʼ�ֲ�λ�õļ��
    /// </summary>
    public void StartHandPosCheck()
    {

        Debug.Log("��ʼ���Ƽ��");
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

        Debug.Log("������Ļ��Ŀ���Ϊ��"+ pos);

       
        _targetPos = pos;

        

    }

    private void Update()
    {

        EnterTarget();
    }
    /// <summary>
    /// �ֽ���Ŀ�����ж�
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
                Debug.Log("�ֽ����˷�Χ");

                _isHandCheck = false;

                StopAndNextPlay();

                InfoText.text = "ʶ����롣";
            }

           
        }

       
        //if()
    }


    /// <summary>
    /// ȡ������
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
    /// �����������
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
    /// ��������
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
                Debug.Log("����json���ݳɹ�");
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
    /// ��������
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

                Debug.Log("����json���ݳɹ�");
            }
            else
            {
                throw new UnityException("�Ѿ�����json�ļ�");

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
