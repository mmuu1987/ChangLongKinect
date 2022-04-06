
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Events;
using UnityEngine.EventSystems;


//规范命名、添加注释、合理封装、限制访问权限、异常处理    
public class SequenceFrame : MonoBehaviour
{



    public enum State
    {
        idle,
        playing,
        pause
    }
    public enum State1
    {
        once,
        loop
    }
    //播放状态(默认、播放中、暂停)
    private State play_state;
    private RawImage _showImage;
    private bool _isSelect;
    private int index;
    private float tim;
    private float waittim;
    private bool isplay;
    private int _selectMax;
    private int _hightMax;
    public Material CurMaterial;
    private int _curMax;
    //正常的序列帧
    private PictureInfo pictureInfo;
    /// <summary>
    /// 高亮的序列帧
    /// </summary>
    private PictureInfo PictureHighLighInfo;

    /// <summary>
    /// 序列帧的名字
    /// </summary>
    public string TexName;
    /// <summary>
    /// 高亮序列帧的名字
    /// </summary>
    public string HighTexName;

    public float FrameNumber = 30;

    public State1 condition = State1.once;


    public bool Play_Awake = false;
    /// <summary>
    /// 点击后是否产生渐变效果
    /// </summary>
    public bool ISGradualChange = true;
    //回调事件  
    public UnityEvent onCompleteEvent;

    public Shader Shader;
    public event Action<SequenceFrame> ClickEvent;

    private bool _isInitEnd = false;

    private void Awake()
    {


    }
    void Start()
    {
        if (SequenceFrameManager.Instance != null)
            SequenceFrameManager.Instance.IsLoadCompleted += InitData;
    }



    private void SetMat(int depth)
    {
        _curMax = depth;
    }
    private void InitData(bool obj)
    {

        if (SequenceFrameManager.Instance == null || !SequenceFrameManager.Instance.IsIntEnd) return;

        if (obj)
        {
            // Debug.Log("初始化完成数据");


            tim = 0;
            index = 0;
            waittim = 1 / FrameNumber;
            play_state = State.idle;
            isplay = false;





            _showImage = this.GetComponent<RawImage>();
            if (_showImage == null) throw new UnityException("没有找到ugui组件");
            CurMaterial = new Material(Shader);

            pictureInfo = SequenceFrameManager.Instance.GeTPictureInfo(TexName);

            if (pictureInfo != null)
                CurMaterial.SetTexture("_TexArr", pictureInfo.Texture2DArray);
            else
            {

                Debug.LogError("没有加载到序列帧 " + TexName);
            }
            _selectMax = pictureInfo.Texture2DArray.depth;


            if (HighTexName != null)
            {
                PictureHighLighInfo = SequenceFrameManager.Instance.GeTPictureInfo(HighTexName);

                if (PictureHighLighInfo == null) Debug.LogWarning("没有加载高亮序列帧");
                else
                {
                    CurMaterial.SetTexture("_HightArr", PictureHighLighInfo.Texture2DArray);
                    _hightMax = PictureHighLighInfo.Texture2DArray.depth;
                }


            }


            _showImage.material = CurMaterial;

            //自动匹配序列帧的大小
            //  _showImage.rectTransform.sizeDelta = pictureInfo.Size;


            SetMat(_selectMax);
            CurMaterial.SetInt("_Index", 0);
            CurMaterial.SetFloat("_Convert", 0);
            _isInitEnd = true;
            if (Play_Awake)
            {
                Play();
            }
        }



    }



    void Update()
    {
        if (_isInitEnd)
        {
            //测试
            if (Input.GetKeyDown(KeyCode.A))
            {
                Play();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Replay();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Stop();
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                Pause();
            }
            UpMove();


            _farmeCount++;
            if (_farmeCount >= 20)
            {
                CurMaterial.SetFloat("_Convert", 0);
            }

        }


    }

    private void OnDestroy()
    {
        Debug.Log("Destroy textureArray");
        Destroy(CurMaterial);

    }

    private void UpMove()
    {
        //单播
        if (condition == State1.once)
        {
            if (play_state == State.idle && isplay)
            {
                play_state = State.playing;
                index = 0;
                tim = 0;
            }
            if (play_state == State.pause && isplay)
            {
                play_state = State.playing;
                tim = 0;
            }
            if (play_state == State.playing && isplay)
            {
                tim += Time.deltaTime;
                if (tim >= waittim)
                {
                    tim = 0;
                    index++;
                    if (index >= _curMax)
                    {
                        index = 0;
                        //ShowImage.sprite = _curSelectList[index];
                        CurMaterial.SetInt("_Index", index);
                        isplay = false;
                        play_state = State.idle;
                        //此处可添加结束回调函数
                        if (onCompleteEvent != null)
                        {
                            onCompleteEvent.Invoke();
                            return;
                        }
                    }
                    // ShowImage.sprite = _curSelectList[index];
                    CurMaterial.SetInt("_Index", index);
                }
            }
        }
        //循环播放
        if (condition == State1.loop)
        {
            if (play_state == State.idle && isplay)
            {
                play_state = State.playing;
                index = 0;
                tim = 0;
            }
            if (play_state == State.pause && isplay)
            {
                play_state = State.playing;
                tim = 0;
            }
            if (play_state == State.playing && isplay)
            {
                tim += Time.deltaTime;
                if (tim >= waittim)
                {
                    tim = 0;
                    index++;
                    if (index >= _curMax)
                    {
                        index = 0;
                        //此处可添加结束回调函数
                    }
                    CurMaterial.SetInt("_Index", index);
                }
            }
        }
    }
    /// <summary>
    /// 播放
    /// </summary>
    public void Play()
    {
        isplay = true;
    }
    /// <summary>
    /// 暂停
    /// </summary>
    public void Pause()
    {
        isplay = false;
        play_state = State.pause;
    }
    /// <summary>
    /// 停止
    /// </summary>
    public void Stop()
    {
        isplay = false;
        play_state = State.idle;
        index = 0;
        tim = 0;
        if (CurMaterial == null)
        {
            Debug.LogWarning("Image为空，请赋值");
            return;
        }
        CurMaterial.SetInt("_Index", 0);
    }
    /// <summary>
    /// 重播
    /// </summary>
    public void Replay()
    {
        isplay = true;
        play_state = State.playing;
        index = 0;
        tim = 0;
    }

    /// <summary>
    /// 点击后改变图片效果
    /// </summary>
    /// <param name="isShow"></param>
    public void ChangeSperite(bool isShow)
    {
        if (ISGradualChange) return;

        if (isShow)
        {
            if (_hightMax > 0)
            {
                SetMat(_hightMax);

            }
            _isSelect = true;
        }
        else
        {

            SetMat(_selectMax);
            _isSelect = false;

        }
    }


    private int _farmeCount = 0;
    private void ClickEffect()
    {

        if (PictureHighLighInfo != null)
        {
            if (_hightMax > 0)
            {
                CurMaterial.SetFloat("_Convert", 1);
            }
            _farmeCount = 0;
        }



    }

    /// <summary>
    /// 是否过了点击的间隔时间，防止密密麻麻点击造成的崩溃
    /// </summary>
    private bool _isInterval = true;

    private Coroutine _coroutineInterval;


    private void OnEnable()
    {
        _isInterval = true;

        if (!_isInitEnd)
        {
            InitData(true);
        }
    }
    /// <summary>
    /// 间隔点允许下一次点击的时间
    /// </summary>
    public float IntervalTime = 0.5f;
    public void OnPointerClick(PointerEventData eventData)
    {


        if (_isInterval)
        {
            _isInterval = false;
            if (this.gameObject.activeInHierarchy)
                _coroutineInterval = StartCoroutine(GlobalSettings.WaitTime(IntervalTime, (() =>
               {
                   _isInterval = true;
                // Debug.LogError("恢复点击");

            })));
        }
        else
        {
            // Debug.LogError("不允许点击");

            return;
        }


        if (ISGradualChange)
        {
            ClickEffect();
        }
        else
        {
            GradualChange();
        }



        if (ClickEvent != null)
            ClickEvent(this);

    }

    private void OnDisable()
    {
        SetMat(_selectMax);
    }
    private void GradualChange()
    {
        //ShowImage.DOColor(new Color(168 / 255f, 183 / 255f, 255f / 255f, 0.35f), 0.25f).OnComplete((() =>
        //{
        //    ShowImage.DOColor(new Color(255f / 255f, 255f / 255f, 255f / 255f, 1f), 0.25f).SetDelay(0.15f);
        //}));


    }
}