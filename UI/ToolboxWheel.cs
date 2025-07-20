
using System.Collections.Generic;
using UnityEngine;
using wildlogicgames;
using System;
using UnityEngine.UI;
using TMPro;

namespace IsometricFarmGenerator
{
    public class ToolboxWheel : MonoBehaviour
    {
        [Header("Field Generator Ref")]
        public FieldGenerator _fieldGenerator;

        [Header("Spade Sprites")]
        public Sprite[] _spadeSprites;

        [Header("Tool Sprites")]
        public SpriteRenderer[] _toolSprites;
        private Color _cachedOriginalColour, _cachedTransparentColour;

        [Header("Tool Text")]
        public TMP_Text _curToolText;
        public TMP_Text _selectedToolText;
        private string _spadePToolStr = "Spade";
        private const string _fenceToolStr = "Fence";
        private const string _treeToolStr = "Tree";
        private const string _wildflowerToolStr = "WildFlower";
        private const string _hedgerowToolStr = "Hedgerow";
        private const string _waterPoolToolStr = "Water";
        private const string _empty1ToolStr = "Empty 1";
        private const string _empty2ToolStr = "Empty 2";
        private const int _toolSelectionCount = 8;//4;
        private int _toolSelectionId = 0;

        private Transform _transform;
        private float rotationSpeed = 90f;//180f; 

        private Quaternion _targRotation;
        private bool _process = false;

        private void Awake() => Setup();
        private void Setup()
        {
            _transform = this.transform;
            _targRotation = _transform.rotation;
            _cachedOriginalColour = new Color();
            _cachedTransparentColour = new Color();

            _cachedOriginalColour = _toolSprites[0].color;
            _cachedTransparentColour = _toolSprites[0].color;
            _cachedTransparentColour.a = 0.5f;

            ApplySoilSelection(_fieldGenerator.GetFieldType());
            SetTextToolStr(0);
        }
        public void ApplySoilSelection(FieldGridCellID fieldGridID)
        {
            

            if (fieldGridID == FieldGridCellID.FieldGridCell_Soil_Peat)
            {
                _toolSprites[0].sprite = _spadeSprites[0];
                _spadePToolStr = "Spade P";
                _selectedToolText.text = "Selected " + _spadePToolStr + " Tool";
                return;
            }
            if (fieldGridID == FieldGridCellID.FieldGridCell_Soil_Light)
            {
                _toolSprites[0].sprite = _spadeSprites[1];
                _spadePToolStr = "Spade L";
                _selectedToolText.text = "Selected " + _spadePToolStr + " Tool";
                return;
            }
            if (fieldGridID == FieldGridCellID.FieldGridCell_Soil_Heavy)
            {
                _toolSprites[0].sprite = _spadeSprites[2];
                _spadePToolStr = "Spade H";
                _selectedToolText.text = "Selected " + _spadePToolStr + " Tool";
                return;
            }
        }
        private void SetTextToolStr(int id)
        {
            if (_curToolText == null) return;


            //SetSpriteTransparency(Mathf.Abs(id));

            if (id == -7) _curToolText.text = _waterPoolToolStr;
            if (id == -6) _curToolText.text = _hedgerowToolStr;
            if (id == -5) _curToolText.text = _wildflowerToolStr;
            if (id == -4) _curToolText.text = _treeToolStr;
            if (id == -3) _curToolText.text = _empty1ToolStr;
            if (id == -2) _curToolText.text = _fenceToolStr;
            if (id == -1) _curToolText.text = _empty2ToolStr;

            if (id == 0) _curToolText.text = _spadePToolStr;

            if (id == 1) _curToolText.text = _waterPoolToolStr;
            if (id == 2) _curToolText.text = _hedgerowToolStr;
            if (id == 3) _curToolText.text = _wildflowerToolStr;
            if (id == 4) _curToolText.text = _treeToolStr;
            if (id == 5) _curToolText.text = _empty1ToolStr;
            if (id == 6) _curToolText.text = _fenceToolStr;
            if (id == 7) _curToolText.text = _empty2ToolStr;


            string temp = _curToolText.text;

            _selectedToolText.text = "Selected " + temp + " Tool";
        }
        private void SetSpriteTransparency(int id)
        {
            for (int i = 0; i < _toolSprites.Length; i++)
            {
                _toolSprites[i].color = _cachedTransparentColour;
            }
            _toolSprites[id].color = _cachedOriginalColour;
        }
        private void Update()
        {
            if (!_process) return;

            _transform.rotation = Quaternion.RotateTowards(_transform.rotation, _targRotation, rotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, _targRotation) < 0.1f)
            {
                _transform.rotation = _targRotation;
                _process = false;
            }
        }
        public void RotateLeft()
        {
            if (!_process)
            {
                float stepAngle = 360f / _toolSelectionCount;

                StartRotation(Vector3.up * stepAngle);// 90f); // +90 deg around Y

                int val = -(_toolSelectionCount - 1);

                if (_toolSelectionId > val)
                {
                    _toolSelectionId--;
                }
                else
                {
                    _toolSelectionId = 0;
                }
                SetTextToolStr(_toolSelectionId);
            }
        }

        public void RotateRight()
        {
            if (!_process)
            {
                float stepAngle = 360f / _toolSelectionCount;

                StartRotation(Vector3.down * stepAngle);//90f); // -90 deg around Y

                int val = _toolSelectionCount - 1;

                if (_toolSelectionId < val)
                {
                    _toolSelectionId++;
                }
                else
                {
                    _toolSelectionId = 0;
                }
                SetTextToolStr(_toolSelectionId);
            }
        }

        private void StartRotation(Vector3 eulerRotation)
        {
            _targRotation = _transform.rotation * Quaternion.Euler(eulerRotation);
            _process = true;
        }
    }
}
