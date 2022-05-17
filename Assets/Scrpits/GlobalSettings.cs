using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using UnityEngine;
using UnityEngine.Rendering;
using Object = Intel.RealSense.Base.Object;
using Random = System.Random;


// Token: 0x0200009A RID: 154
public static class GlobalSettings
{




    public static Configure Configure = null;

    /// <summary>
    /// 存放序列帧文件夹的路径，不需要的序列帧文件及时删掉，否则占用内存
    /// </summary>
    public static string SequenceFramePath = Application.streamingAssetsPath + "/SequenceFrame";

	// Token: 0x06000631 RID: 1585 RVA: 0x000451F0 File Offset: 0x000433F0
	public static bool FileIsUsed(string fileFullName)
	{
		bool result = false;
		bool flag = !File.Exists(fileFullName);
		if (flag)
		{
			result = false;
		}
		else
		{
			FileStream fileStream = null;
			try
			{
				fileStream = File.Open(fileFullName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
			}
			catch (IOException ioEx)
			{
				result = true;
			}
			catch (Exception ex)
			{
				result = true;
			}
			finally
			{
				bool flag2 = fileStream != null;
				if (flag2)
				{
					fileStream.Close();
				}
			}
		}
		return result;
	}

	
	/// <summary>
	/// 创建8位的UUID
	/// </summary>
	/// <returns></returns>
	public static string CreatUuid()
	{
		string uuid = null;
		Random random = new Random();
		for (int i = 1; i <= 8; i++)
		{
			uuid += GlobalSettings.CharsLetter[random.Next(GlobalSettings.CharsLetter.Length)].ToString();
		}
		return uuid;
	}

	// Token: 0x06000637 RID: 1591 RVA: 0x00045B3D File Offset: 0x00043D3D
	public static IEnumerator WaitTime(float time, Action action)
	{
		yield return new WaitForSeconds(time);
		bool flag = action != null;
		if (flag)
		{
			action();
		}
		yield break;
	}

    /// <summary>
    /// 缩略图片
    /// </summary>
    /// <param name="source"></param>
    /// <param name="newWidth"></param>
    /// <param name="newHeight"></param>
    /// <returns></returns>
    public static Texture2D Resize(Texture2D source, int newWidth, int newHeight)
    {
        source.filterMode = FilterMode.Point;
        RenderTexture rt = new RenderTexture(newWidth, newHeight, 0);
        rt.filterMode = FilterMode.Point;
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        var nTex = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false);//TextureFormat.RGBA32格式，经测试，此格式耗费显存相对较低
        nTex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        nTex.Apply();
        RenderTexture.active = null;
        UnityEngine.Object.Destroy(rt);//及时删掉
        return nTex;
    }


    public static Texture2DArray SetTexToGpu(Texture2D[] texs)
    {
        if (texs == null || texs.Length == 0)
        {

            return null;
        }

        if (SystemInfo.copyTextureSupport == CopyTextureSupport.None ||
            !SystemInfo.supports2DArrayTextures)
        {

            return null;
        }

        Texture2DArray texArr = new Texture2DArray(texs[0].width, texs[0].width, texs.Length, texs[0].format, false, false);

        for (int i = 0; i < texs.Length; i++)
        {
            //拷贝的贴图必须长宽一致,不要求2的幂次方 
            Graphics.CopyTexture(texs[i], 0, 0, texArr, i, 0);
        }

        texArr.wrapMode = TextureWrapMode.Clamp;
        texArr.filterMode = FilterMode.Trilinear;
        for (int i = 0; i < texs.Length; i++)
        {
			UnityEngine.Object.Destroy(texs[i]);


        }
        Resources.UnloadUnusedAssets();

        return texArr;
    }

	//得到最接近 into 且大于into的二次方数
	public static int Get2PowHigh(int into)
    {
        --into;//避免正好输入一个2的次方数
        into |= into >> 1;
        into |= into >> 2;
        into |= into >> 4;
        into |= into >> 8;
        into |= into >> 16;
        return ++into;
    }

	//得到最接近 into 且小于into的二次方数
	public static int Get2PowLow(int into)
    {
        return Get2PowHigh(into) >> 1;
    }

	// Token: 0x06000639 RID: 1593 RVA: 0x00045BC6 File Offset: 0x00043DC6
	public static IEnumerator WaitEndFarme(Action callBack)
	{
		yield return null;
		bool flag = callBack != null;
		if (flag)
		{
			callBack();
		}
		yield break;
	}

	// Token: 0x0600063A RID: 1594 RVA: 0x00045BD8 File Offset: 0x00043DD8
	public static void InitArg()
	{
		string av = PlayerPrefs.GetString("AV");
		string tv = PlayerPrefs.GetString("TV");
		string iso = PlayerPrefs.GetString("ISO");
		bool flag = !string.IsNullOrEmpty(av);
		if (flag)
		{
			GlobalSettings.AV = uint.Parse(av);
		}
		bool flag2 = !string.IsNullOrEmpty(tv);
		if (flag2)
		{
			GlobalSettings.TV = uint.Parse(tv);
		}
		bool flag3 = !string.IsNullOrEmpty(iso);
		if (flag3)
		{
			GlobalSettings.ISO = uint.Parse(iso);
		}
	}

	// Token: 0x0600063B RID: 1595
	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	public static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int Width, int Height, int flags);

	// Token: 0x0600063C RID: 1596
	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	public static extern IntPtr GetForegroundWindow();

	// Token: 0x0600063D RID: 1597
	[DllImport("user32.dll")]
	public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

	// Token: 0x0600063E RID: 1598
	[DllImport("user32.dll")]
	private static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);

	// Token: 0x0600063F RID: 1599 RVA: 0x00045C59 File Offset: 0x00043E59
	public static void OnClickMinimize()
	{
		GlobalSettings.ShowWindow(GlobalSettings.GetForegroundWindow(), 2);
	}

	// Token: 0x06000640 RID: 1600 RVA: 0x00045C68 File Offset: 0x00043E68
	public static void OnClickMaximize()
	{
		GlobalSettings.ShowWindow(GlobalSettings.GetForegroundWindow(), 3);
	}

	// Token: 0x06000641 RID: 1601 RVA: 0x00045C77 File Offset: 0x00043E77
	public static void OnClickRestore()
	{
		GlobalSettings.ShowWindow(GlobalSettings.GetForegroundWindow(), 1);
	}

	// Token: 0x06000642 RID: 1602 RVA: 0x00045C86 File Offset: 0x00043E86
	public static void TopWindows()
	{
		GlobalSettings.SetWindowPos(GlobalSettings.GetForegroundWindow(), -1, 0, 0, 0, 0, 3);
	}

	// Token: 0x06000643 RID: 1603
	[DllImport("User32.dll")]
	public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

	// Token: 0x06000644 RID: 1604
	[DllImport("User32.dll")]
	public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);

	// Token: 0x04000675 RID: 1653
	public const float ZoomMagnification = 1.6f;

	// Token: 0x04000676 RID: 1654
	public static int PictureWidth = 2400;

	// Token: 0x04000677 RID: 1655
	public static int PictureHeight = 1600;

	// Token: 0x04000678 RID: 1656
	public static float WidthScale = 0.5f;

	// Token: 0x04000679 RID: 1657
	public static float HeightScale = 1f;

	// Token: 0x0400067A RID: 1658
	public static uint AV = 40U;

	// Token: 0x0400067B RID: 1659
	public static uint TV = 109U;

	// Token: 0x0400067C RID: 1660
	public static uint ISO = 104U;

	// Token: 0x0400067D RID: 1661
	private static char[] CharsLetter = new char[]
	{
		'1',
		'2',
		'3',
		'4',
		'5',
		'6',
		'7',
		'8',
		'9',
		'0'
	};

	// Token: 0x0400067E RID: 1662
	public static string ServerPictureIp = "http://www.syyj.tglfair.com/Webpage/VoiceOffice/MyScreenshots.aspx";

	public static string ServerMP4Ip = "http://www.syyj.tglfair.com/Webpage/VoiceOffice/ServerGetMp4.aspx";

	public static string GetMp4 = "http://www.syyj.tglfair.com/Webpage/VoiceOffice/ClientGetMp4.aspx";

	// Token: 0x0400067F RID: 1663
	public static bool IsDeletedPicture = true;

	// Token: 0x04000680 RID: 1664
	public static float Brightness = 1f;

	// Token: 0x04000681 RID: 1665
	public static float Saturation = 1f;

	// Token: 0x04000682 RID: 1666
	public static float Contrast = 1f;

	// Token: 0x04000683 RID: 1667
	public static bool IsDebug = true;

	// Token: 0x04000684 RID: 1668
	public static string ServerIp = "";

	

	// Token: 0x04000686 RID: 1670
	public static string Stie = "阿里巴巴大文娱试衣镜";

	/// <summary>
	/// 停留UI触发点击的时间
	/// </summary>
    public static float ClickTime = 2f;

	// Token: 0x04000687 RID: 1671
	public static bool IsOutLog = true;

	// Token: 0x04000688 RID: 1672
	private const uint SWP_SHOWWINDOW = 64U;

	// Token: 0x04000689 RID: 1673
	private const int GWL_STYLE = -16;

	// Token: 0x0400068A RID: 1674
	private const int WS_BORDER = 1;

	// Token: 0x0400068B RID: 1675
	private const int SW_SHOWMINIMIZED = 2;

	// Token: 0x0400068C RID: 1676
	private const int SW_SHOWMAXIMIZED = 3;

	// Token: 0x0400068D RID: 1677
	private const int SW_SHOWRESTORE = 1;

	// Token: 0x0400068E RID: 1678
	public const int REPORT_LEVENL = 2;

	// Token: 0x0400068F RID: 1679
	public static int LOG_LEVENL = 3;
}


public class Configure
{
	/// <summary>
	/// 待机的时间
	/// </summary>
	public double StandBy;

	/// <summary>
	/// 扫描头像的时间
	/// </summary>
	public double StayCamTime;
	/// <summary>
	/// 是否加载完全部资源
	/// </summary>
	public bool IsLoadCompleted;


	/// <summary>
	/// 要求作出动作的的等待时间，过了这个时间则自动进入下一步
	/// </summary>
    public double WaitTime = 45;
	public bool IsShowFPS;
	

	public double UnitySize = 512;

	/// <summary>
	/// 一次检测姿势的时间
	/// </summary>
    public double CheckTime = 1.5;

	/// <summary>
	/// 在CheckTime时间内检测的次数
	/// </summary>
	public double CheckCount = 3;

	/// <summary>
	/// 检测通过的比率
	/// </summary>
    public double CheckPercent = 0.8;

    public JsonVector3 ScreenPosTarget3_02;


   

    public JsonVector3 ScreenPosTarget5_04;


    

	public Configure()
    {
        ScreenPosTarget3_02 = new JsonVector3();

        ScreenPosTarget5_04 = new JsonVector3();

	}
}

public class JsonVector3
{
    public double X;

    public double Y;

    public double Z;

    public JsonVector3()
    {
        X = 0;
        Y = 0;
        Z = 0;
    }

    public Vector3 ConvertVector3()
    {
		return new Vector3((float)X, (float)Y, (float)Z);
    }
}
