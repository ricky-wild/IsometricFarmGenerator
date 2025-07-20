using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using wildlogicgames;

namespace IsometricFarmGenerator
{
    public enum LogoStateID
    {
        Logo_Black_Text = 0,
        Logo_White_Text = 1,
    };

    public enum LogoScreenPosID
    {
        Logo_Pos_Centre = 0,
        Logo_Pos_BotRight = 1,

    };

    [RequireComponent(typeof(SpriteRenderer))]
    public class LogoObj : MonoBehaviour
    {

        [Header("Rotation Speed")]
        [Range(50.0f, 100.0f)]
        public float _rotSpeed;

        [Header("Logo Variants")]
        public Sprite[] _logoSprites;

        //[Header("Logo UI Rect Pos")]
        //public RectTransform[] _logoUIPositions;

        [Header("Logo UI RAW Image")]
        public RawImage[] _rawImage;
        //private RectTransform _rawImgRectTrans;

        private Transform _transform;
        private SpriteRenderer _spriteRenderer;
        private bool _process;

        private LogoStateID _logoDisplayState;
        private LogoScreenPosID _logoPosState;


        private void Awake() => Setup();
        private void Setup()
        {
            if (_transform == null) _transform = this.transform;
            if (_spriteRenderer == null) _spriteRenderer = this.GetComponent<SpriteRenderer>();
            //if (_rawImage != null) _rawImgRectTrans = _rawImage.GetComponent<RectTransform>();

            _logoDisplayState = LogoStateID.Logo_Black_Text;
            _logoPosState = LogoScreenPosID.Logo_Pos_Centre;

            _rawImage[(int)LogoScreenPosID.Logo_Pos_Centre].enabled = true;
            _rawImage[(int)LogoScreenPosID.Logo_Pos_BotRight].enabled = false;

            _process = true;
        }
        private void Update()
        {
            if (!_process) return;

            _transform.Rotate(0f, _rotSpeed * Time.deltaTime, 0f);
        }

        public void SetLogo(LogoStateID logoStateID)
        {        
            _spriteRenderer.sprite = _logoSprites[(int)logoStateID];
        }

        public void SetLogoPos(LogoScreenPosID logoScreenPosID)
        {
            if (_logoPosState == logoScreenPosID) return;

            _logoPosState = logoScreenPosID;

            _rawImage[(int)LogoScreenPosID.Logo_Pos_Centre].enabled = false;
            _rawImage[(int)LogoScreenPosID.Logo_Pos_BotRight].enabled = false;

            _rawImage[(int)_logoPosState].enabled = true;
        }

    }
}
