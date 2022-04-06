using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// 序列帧动画管理器
/// </summary>
public class SequenceFrameManager : MonoBehaviour
{

    public static SequenceFrameManager Instance;

    public List<PictureInfo> PictureInfos = new List<PictureInfo>();

   


    /// <summary>
    /// 全部序列帧的缩放图片的倍数，占用显存过大
    /// </summary>
    public int TargetScale = 1024;

    public event Action<bool> IsLoadCompleted;

    public bool IsIntEnd = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null) throw new UnityException("已经设置了单例");

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
    /// 加载动画序列帧
    /// </summary>
    private IEnumerator LoadSequenceFrame()
    {
        string[] directories = Directory.GetDirectories(GlobalSettings.SequenceFramePath);

        if (directories.Length <= 0) throw new UnityException("在 " + GlobalSettings.SequenceFramePath + " 文件夹下没有找到序列帧文件夹");
        int count = 0;
        foreach (string directory in directories)
        {

            Debug.Log("序列帧的路径： "+directory);

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
               Debug.LogError("序列帧命名格式不正确   "+ directoryInfo.Name  +"     "+   e.ToString());
            }
           

           
            Vector2 size = Vector2.zero;
            int scaleSize = 0;  //统一的尺寸
            foreach (string file in files)
            {
                if (file.Contains(".meta")) continue;

                byte[] bytes = File.ReadAllBytes(file);

                Texture2D tex = new Texture2D(4, 4);

                tex.LoadImage(bytes);

                tex.Apply();//应用后，得到图片的真正尺寸

                if (scaleSize == 0)
                {
                    size = new Vector2(tex.width, tex.height);

                    scaleSize = (int)Mathf.Sqrt(Mathf.ClosestPowerOfTwo((int)(size.x * size.y)));


                    scaleSize = GlobalSettings.Get2PowLow(scaleSize);

                    // TargetScale = (int) DataManager.Instance.Configure.UnitySize;




                    Debug.Log("序列帧缩放的尺寸为：" + scaleSize + "  图片尺寸目标为：" + maxSize);

                    if (scaleSize >= maxSize) scaleSize = (int)maxSize;//图片尽量别大于1024
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

        Debug.Log("序列帧资源加载完成");

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
/// 一个序列帧集合的整体信息
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
    /// 序列帧的名字，以加载的序列帧文件夹名字为准
    /// </summary>
    public string Name;

    public Texture2DArray Texture2DArray;
    /// <summary>
    /// 序列帧原本的尺寸，一版情况下，每张序列帧的尺寸都是一致的
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

