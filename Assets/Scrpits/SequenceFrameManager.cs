using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// ����֡����������
/// </summary>
public class SequenceFrameManager : MonoBehaviour
{

    public static SequenceFrameManager Instance;

    public List<PictureInfo> PictureInfos = new List<PictureInfo>();

   


    /// <summary>
    /// ȫ������֡������ͼƬ�ı�����ռ���Դ����
    /// </summary>
    public int TargetScale = 1024;

    public event Action<bool> IsLoadCompleted;

    public bool IsIntEnd = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null) throw new UnityException("�Ѿ������˵���");

        Instance = this;

       //StartCoroutine(LoadSequenceFrame());
    }

    private void Start()
    {
        StartCoroutine(LoadSequenceFrame());
    }
    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// ���ض�������֡
    /// </summary>
    private IEnumerator LoadSequenceFrame()
    {
        string[] directories = Directory.GetDirectories(GlobalSettings.SequenceFramePath);

        if (directories.Length <= 0) throw new UnityException("�� " + GlobalSettings.SequenceFramePath + " �ļ�����û���ҵ�����֡�ļ���");
        int count = 0;
        foreach (string directory in directories)
        {

            Debug.Log("����֡��·���� "+directory);

            string[] files = Directory.GetFiles(directory);

            List<Texture2D> texs = new List<Texture2D>();
            PictureInfo pictureInfo = new PictureInfo();
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);

            string[] names = directoryInfo.Name.Split(new[] {'-'}, StringSplitOptions.None);
            float maxSize = 0;
            try
            {
                pictureInfo.Name = names[0];

                maxSize = int.Parse(names[1]);
            }
            catch (Exception e)
            {
               Debug.LogError("����֡������ʽ����ȷ   "+ directoryInfo.Name  +"     "+   e.ToString());
            }
           

           
            Vector2 size = Vector2.zero;
            int scaleSize = 0;  //ͳһ�ĳߴ�
            foreach (string file in files)
            {
                if (file.Contains(".meta")) continue;

                byte[] bytes = File.ReadAllBytes(file);

                Texture2D tex = new Texture2D(4, 4);

                tex.LoadImage(bytes);

                tex.Apply();//Ӧ�ú󣬵õ�ͼƬ�������ߴ�

                if (scaleSize == 0)
                {
                    size = new Vector2(tex.width, tex.height);

                    scaleSize = (int)Mathf.Sqrt(Mathf.ClosestPowerOfTwo((int)(size.x * size.y)));


                    scaleSize = GlobalSettings.Get2PowLow(scaleSize);

                    // TargetScale = (int) DataManager.Instance.Configure.UnitySize;




                    Debug.Log("����֡���ŵĳߴ�Ϊ��" + scaleSize + "  ͼƬ�ߴ�Ŀ��Ϊ��" + maxSize);

                    if (scaleSize >= maxSize) scaleSize = (int)maxSize;//ͼƬ���������1024
                    pictureInfo.Size = size;

                }
                tex = GlobalSettings.Resize(tex, scaleSize, scaleSize);

                texs.Add(tex);
                yield return null;
                Resources.UnloadUnusedAssets();
               

            }

            Texture2DArray texture2DArray = GlobalSettings.SetTexToGpu(texs.ToArray());

            pictureInfo.Texture2DArray = texture2DArray;

            PictureInfos.Add(pictureInfo);

           //break;
        }

        Debug.Log("����֡��Դ�������");

        //PictureInfos.Clear();
        IsIntEnd = true;
        if (IsLoadCompleted != null) IsLoadCompleted(true);

      
    }

    public Texture2DArray GeTexture2DArray(string texArrayName)
    {
        foreach (PictureInfo info in PictureInfos)
        {
            if (info.Name == texArrayName)
            {
                return info.Texture2DArray;
            }
        }

        return null;
    }

    public PictureInfo GeTPictureInfo(string texArrayName)
    {
        foreach (PictureInfo info in PictureInfos)
        {
            if (info.Name == texArrayName)
            {
                return info;
            }
        }

        return null;
    }

    public Texture2DArray LoadImage(string key)
    {
        foreach (PictureInfo info in PictureInfos)
        {
            if (info.Name == key)
            {
                return info.Texture2DArray;
            }
        }

        return null;
    }

   

}
/// <summary>
/// һ������֡���ϵ�������Ϣ
/// </summary>
public class PictureInfo
{

    public PictureInfo()
    {

    }
    public PictureInfo(string name, Vector2 size)
    {
        Name = name;

        Size = size;
    }
    /// <summary>
    /// ����֡�����֣��Լ��ص�����֡�ļ�������Ϊ׼
    /// </summary>
    public string Name;

    public Texture2DArray Texture2DArray;
    /// <summary>
    /// ����֡ԭ���ĳߴ磬һ������£�ÿ������֡�ĳߴ綼��һ�µ�
    /// </summary>
    public Vector2 Size;

    public override string ToString()
    {
        string str = "\r\n";
        str += "PictureName is " + Name + "\r\n";
        str += "Size is " + Size + "\r\n";
        return str;
    }
}

