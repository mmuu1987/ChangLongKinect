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
    /// �Ƿ��ǵ�һ��ʶ��
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
            throw new UnityException("�Ѿ������˵���");
        }
        if (Configure == null)
        {
            LoadConfigure();
        }
    }

    private void Start()
    {

        path1 = Application.streamingAssetsPath + "/01����ӰƬ/01��������";

        path2 = Application.streamingAssetsPath + "/02����ѩ/02����ѩ";

        path3_1 = Application.streamingAssetsPath + "/03�ַ���Ͱ��-ιʳ/03-01�����˽���";

        path3_2 = Application.streamingAssetsPath + "/03�ַ���Ͱ��-ιʳ/03-02�����˹۲�״̬+ˮͰ����";

        path3_3 = Application.streamingAssetsPath + "/03�ַ���Ͱ��-ιʳ/03-03�����˹۲�״̬+���㶯��";

        path4_1 = Application.streamingAssetsPath + "/04���Ʒ���/04-01�����ϰ�";

        path4_1 = Application.streamingAssetsPath + "/04���Ʒ���/04-02�������״̬";

        path4_1 = Application.streamingAssetsPath + "/04���Ʒ���/04-03������״̬";

        path5_1 = Application.streamingAssetsPath + "/05���Ʒ���+��Ħ/05-01�����˽��⺣��";

        path5_1 = Application.streamingAssetsPath + "/05���Ʒ���+��Ħ/05-02�������״̬";

        path5_1 = Application.streamingAssetsPath + "/05���Ʒ���+��Ħ/05-03������״̬";

        path5_1 = Application.streamingAssetsPath + "/05���Ʒ���+��Ħ/05-04�������״̬";

        path5_1 = Application.streamingAssetsPath + "/05���Ʒ���+��Ħ/05-05����˫���Ķ�Ƥ";

        path6_1 = Application.streamingAssetsPath + "/06����/06-01�����˽��⺣��";

        path6_2 = Application.streamingAssetsPath + "/06����/06-02�����ϰ�";

        path6_3 = Application.streamingAssetsPath + "/06����/06-03�������";

        path6_4 = Application.streamingAssetsPath + "/06����/06-04��������";

        path7_1 = Application.streamingAssetsPath + "/07�����ζ�/07-01�����˽��⺣��";

        path7_2 = Application.streamingAssetsPath + "/07�����ζ�/07-02�������";

        path7_3 = Application.streamingAssetsPath + "/07�����ζ�/07-03������ˮ��";

        path7_4 = Application.streamingAssetsPath + "/07�����ζ�/07-04����20�뻶��";

        path7_5 = Application.streamingAssetsPath + "/07�����ζ�/07-05����30��ending";

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
            Debug.Log("����");
        }
        else
        {
          
            //Debug.Log("ʶ������");
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
    /// ȡ������
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
    /// �����������
    /// </summary>
    private void StartStandby()
    {
      
        KinectManager.Instance.playerCalibrationPose = GestureType.None;
        KinectManager.Instance.ClearKinectUsers();
        this._isFirstDiscriminate = true;
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

            Configure = JsonUtility.FromJson<Configure>(str);

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
        SaveConfiugre(path, true);
    }
    /// <summary>
    /// ��������
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
            PlayVideo(path1);
        }
    }
#endif


}
