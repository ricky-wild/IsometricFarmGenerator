using System;
using UnityEngine;
using wildlogicgames;
using UnityEngine.UI;
using TMPro;

namespace IsometricFarmGenerator
{
    public class RewardController : MonoBehaviour
    {
        [Header("UI RewardVFXWindow RawImage Ref")]
        public RawImage _UIRawImg;
        private Color _UIRawImgColor;
        private float _colourAlpha;
        private const float _alphaFreq = 0.05f;//0.025f;

        [Header("Confetti Particle Sys Obj")]
        public GameObject _particleSysObj;
        private ParticleSystem _particleSysCached;

        [Header("Badge Animator Ref")]
        public Animator _animator;
        private int _badgeAnimTimeIndex;

        [Header("Quad Light Shafts Obj")]
        public GameObject _lightingQuadObj;

        [Header("Badge Title Text Obj")]
        public GameObject _badgeTitleObj;
        private TMP_Text _badgeTitleStrCached;


        private const int _timerCount = 2;
        private GameObject[] _timerObjs = new GameObject[2];
        private WTimer[] _timers = new WTimer[2];
        private float[] _waitTimes = new float[2];

        private int _hedgreRowCorrectIncrement = 0;
        private const string _hedgerowBadgeA = "Biodiversity\nI";
        private const string _hedgerowBadgeB = "Biodiversity\nII";
        private const string _hedgerowBadgeC = "Biodiversity\nIII";

        private bool _process, _fadeOutFXEnabled, _intialized, _hideBadgeTextFlag;

        private void Awake() => Setup();
        private void Setup()
        {
            //_animator = this.GetComponent<Animator>();

            _process = false;
            _fadeOutFXEnabled = false;
            _intialized = false;
            _hideBadgeTextFlag = true;

            //_animator.Play("Default_Intro");
            //_animator.Play("Default_Idle");

            _UIRawImgColor = new Color();
            if (_UIRawImg != null)
            {
                _UIRawImgColor = _UIRawImg.color;
                _colourAlpha = 1.0f;
            }

            if (_particleSysObj != null)
            {
                _particleSysCached = _particleSysObj.GetComponent<ParticleSystem>();
            }

            if (_badgeTitleObj != null)
            {
                _badgeTitleStrCached = _badgeTitleObj.GetComponent<TMP_Text>();
                _badgeTitleObj.SetActive(false);
            }
            if (_lightingQuadObj != null)
            {
                _lightingQuadObj.SetActive(false);
            }

            for (int i = 0; i < _timerCount; i++)
            {
                _timerObjs[i] = new GameObject();
                _timerObjs[i].AddComponent<WTimer>();
                _timers[i] = _timerObjs[i].GetComponent<WTimer>();
            }
            _waitTimes[0] = 1.33f;// 2 secs
            _waitTimes[1] = 3.75f;

        }
        private void BeginRewardEvent()
        {
            if (_animator != null) PlayIntroBadgeAnimation();

            _badgeAnimTimeIndex = 0;
            _timers[_badgeAnimTimeIndex].StartTimer(_waitTimes[_badgeAnimTimeIndex]);


        }
        private void BeginEventFadeOut()
        {
            _fadeOutFXEnabled = true;

        }
        private void OnEnable()
        {
            _intialized = true;
            //Init();



            //_process = true;

            //_colourAlpha = 1.0f;
            //_UIRawImgColor.a = _colourAlpha;
            //_UIRawImg.color = _UIRawImgColor;

            //_particleSysObj.SetActive(true);
            //_particleSysCached.Play();



            //BeginRewardEvent();

        }
        private void Init()
        {
            _process = true;
            _hideBadgeTextFlag = true;

            _colourAlpha = 1.0f;
            _UIRawImgColor.a = _colourAlpha;
            _UIRawImg.color = _UIRawImgColor;

            _particleSysObj.SetActive(true);
            _particleSysCached.Play();

            EventManager.SetEventFlag(EventFlagID.EventFlag_BadgeReward_Process);
            //AudioManager.PlaySFX(AudioID.Audio_Cheering01_SFX);

            BeginRewardEvent();
        }
        private void OnDisable()
        {
            _intialized = false;
            _process = false;

        }
        private void Update()
        {
            if (!_process) return;

            BadgeAnimationUpdate();
            FadeOutFXUpdate();
        }
        private void FadeOutFXUpdate()
        {
            if (!_fadeOutFXEnabled) return;

            if (_UIRawImgColor.a >= 0f)
            {
                _colourAlpha -= _alphaFreq;
                _UIRawImgColor.a = _colourAlpha;
                _UIRawImg.color = _UIRawImgColor;

                if (!_hideBadgeTextFlag)
                {
                    if (_colourAlpha <= 0.5f) //Half Way Faded. The hide the badge text here.
                    {
                        if (_badgeTitleObj.activeSelf)
                        {
                            _hideBadgeTextFlag = true;
                            _badgeTitleObj.SetActive(false);
                            _lightingQuadObj.SetActive(false);
                        }
                    }
                }

            }
            else
            {
                _fadeOutFXEnabled = false;
                _particleSysCached.Stop();
                _particleSysObj.SetActive(false);
                _badgeTitleObj.SetActive(false);
                _process = false;
                EventManager.SetEventFlag(EventFlagID.EventFlag_Idle);
                this.gameObject.SetActive(false);

            }
        }
        private void BadgeAnimationUpdate()
        {
            if (_badgeAnimTimeIndex == -1) return;

            if (_timers[_badgeAnimTimeIndex].HasTimerFinished(true))
            {
                if (_badgeAnimTimeIndex == 0)
                {
                    if (_animator != null) PlayDefaultBadgeAnimation();
                    _badgeAnimTimeIndex++;
                    _timers[_badgeAnimTimeIndex].StartTimer(_waitTimes[_badgeAnimTimeIndex]);
                    _hideBadgeTextFlag = false;
                    _badgeTitleObj.SetActive(true);
                    _lightingQuadObj.SetActive(true);
                    AudioManager.PlaySFX(AudioID.Audio_Cheering01_SFX);
                    return;
                }
                if (_badgeAnimTimeIndex == 1)
                {
                    //_badgeTitleObj.SetActive(true);
                    BeginEventFadeOut();
                    _badgeAnimTimeIndex = -1;
                    return;
                }
            }
        }
        private void PlayIntroBadgeAnimation() => _animator.Play("Default_Intro");

        private void PlayDefaultBadgeAnimation() => _animator.Play("Default_Idle");

        public void UpdateHedgrowRewardCount()// => _hedgreRowCorrectIncrement++;
        {
            _hedgreRowCorrectIncrement++;

            if (_intialized)
            {
                if (_hedgreRowCorrectIncrement == 1)
                {
                    _badgeTitleStrCached.text = _hedgerowBadgeA;
                    Init();
                    return;
                }
                if (_hedgreRowCorrectIncrement == 2)
                {
                    _badgeTitleStrCached.text = _hedgerowBadgeB;
                    Init();
                    return;
                }
                if (_hedgreRowCorrectIncrement == 3)
                {
                    _badgeTitleStrCached.text = _hedgerowBadgeC;
                    Init();
                    return;
                }

                _intialized = false;
                _process = false;
            }
        }
    }
}
