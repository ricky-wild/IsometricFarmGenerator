
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using wildlogicgames;
using System;

namespace IsometricFarmGenerator
{
    public enum UIWindowID //Must corrispond to references in _UIWindowObjs inspector.
    {
        Window_Start = 0,
        Window_Control = 1,

        Window_Field_Start = 2,
        Window_Field_1 = 3,
        Window_Field_2 = 4,
        Window_Field_3 = 5,
        Window_Field_4 = 6,

        Window_Crops_1 = 7,
        Window_Crops_2 = 8,
        Window_Crops_3 = 9,
        Window_Crops_4 = 10,

        Window_Tillage_1 = 11,
        Window_Tillage_2 = 12,
        Window_Tillage_3 = 13,
        Window_Tillage_4 = 14,

        Window_Choice_Setup = 15,

        Window_Energy_Cost = 16,
        Window_Time_Line = 17,
        Window_Biodiversity_Rating = 18,
        Window_Yield_Rating = 19,
        Window_SoilHealth_Rating = 20,
        Window_FuelFibreYield_Rating = 21,
        Window_CarbonSequestration_Rating = 22,
        Window_AsetheticRecreation_Rating = 23,


        Window_Toolbox_Enable = 24,
        Window_RootNode_Activation = 25,
        Window_Toolbox_Selection = 26,
        Window_GoBack_FieldViews = 27,

        Window_Side_Panel = 28,
        Window_CurField_Panel = 29,
        Window_Cur_Crop = 30,
        Window_Cur_Soil = 31,
        Window_Cur_Size = 32,
        Window_Cur_Cost = 33,

        Window_Dialogue_Panel = 34,

        Window_Character_Face = 35,

        Window_Badge_Reward = 36,

        Window_TotalSustainability_Rating = 37,

    };
    public enum UIDropdownID //Must corrispond to references in _UIDropdownObjs inspector.
    {
        Dropdown_Field_Count = 0,
        Dropdown_Soil_Selection = 1,

        Dropdown_Field_Size_1 = 2,
        Dropdown_Field_Size_2 = 3,
        Dropdown_Field_Size_3 = 4,
        Dropdown_Field_Size_4 = 5,

        Dropdown_Crop_1 = 6,
        Dropdown_Crop_2 = 7,
        Dropdown_Crop_3 = 8,
        Dropdown_Crop_4 = 9,

        Dropdown_Tillage_1 = 10,
        Dropdown_Tillage_2 = 11,
        Dropdown_Tillage_3 = 12,
        Dropdown_Tillage_4 = 13,

        Dropdowm_Optimization = 14,

    };
    public enum UIButtonID //Must corrispond to references in _UIButtonObjs inspector.
    {
        Button_None_Error = -1,

        Button_Run = 0,
        Button_Exit = 1,

        Button_Add_Fields = 2,
        Button_Add_Crops = 3,
        Button_Add_Tillage = 4,

        Button_Confirm_Field_Count = 5,
        Button_Confirm_Field_1 = 6,
        Button_Confirm_Field_2 = 7,
        Button_Confirm_Field_3 = 8,
        Button_Confirm_Field_4 = 9,

        Button_Confirm_Crops_1 = 10,
        Button_Confirm_Crops_2 = 11,
        Button_Confirm_Crops_3 = 12,
        Button_Confirm_Crops_4 = 13,

        Button_Confirm_Tillage_1 = 14,
        Button_Confirm_Tillage_2 = 15,
        Button_Confirm_Tillage_3 = 16,
        Button_Confirm_Tillage_4 = 17,

        Button_Generate = 18,

        Button_Field_Output_1 = 19,
        Button_Field_Output_2 = 20,
        Button_Field_Output_3 = 21,
        Button_Field_Output_4 = 22,

        //Button_Tool_Hedgerow = 23,
        //Button_Tool_SpadeDig = 24,
        Button_Toolbox_Enable = 23,
    };
    public enum UICursorID
    {
        Cursor_Plat_Hedgerow = 0,
        Cursor_Plat_SpadeDig = 1,
    };
   
    
    public enum UITimerID //Identifies array usage of timers and all associated variables
	{
        Timer_Init_Logo_Pos_Change = 0,
        Timer_Field_Select_But_Update = 1,
        Timer_Startup_Logo_Only_Display = 2,
        Timer_RootNode_Win_Display = 3,
        Timer_Initial_Character_Welcome = 4,
        Timer_Dialogue = 5,
        Timer_Attrribute_Text_Display = 6
	};
    public enum UIFieldTextDetailID //Must corrispond to references in _UIFieldTextDetails inspector.
    {
        FieldText_Count = 0,
        FieldText_Size = 1,
        FieldText_Soil = 2,
        FieldText_Crop = 3
    };
    public enum UIAttrributeTextID  //Must corrispond to references in _attrributeText inspector.
    {
        Text_Biodiversity = 0,
        Text_Yield = 1,
        Text_SoilHealth = 2,
        Text_FuelFibreYield = 3,
        Text_CarbonSequestration = 4,
        Text_AestheticRecreation = 5,
        Text_TotalSustainability = 6,
    };


    public class UIManager : MonoBehaviour
    {

        [Header("Field Generator Ref")]
        public FieldGenerator _fieldGenerator;
        private FieldGridSize[] _fieldGridTooAdd = new FieldGridSize[4];
        private int _curSelectedFieldId;
        private CellObj _cachedClickedCell;
        private Vector3 _relativeClickedCellPos, _originGridCellPos;
        private Plane _cachedColliderReplacement;
        private Ray _cachedRay;
        private float _cachedCellWidth;
        private int _cachedClickedX;
        private int _cachedClickedY;

        [Header("Toolbox Wheel Ref")]
        public ToolboxWheel _toolboxWheel;

        [Header("Camera Manager Ref")]
        public CameraManager _cameraManager;

        [Header("Reward CameraUI Controller Ref")]
        public GameObject _rewardControllerObj;
        private RewardController _rewardControllerCached;



        [Header("UI Windows")]
        public GameObject[] _UIWindowObjs;

        [Header("UI Dropdowns")]
        public GameObject[] _UIDropdownObjs;
        private TMP_Dropdown[] _UIDropdownsCached = new TMP_Dropdown[17];
        private List<string> _fieldCountOptions;
        private string _selectedFieldCount;
        private int _fieldCount;
        private List<string> _fieldGridSizeOptions;
        private string _selectedFieldGridSize;
        private List<string> _soilTypeOptions;
        private string _selectedFieldSoilType;
        private List<string> _cropTypeOptions;
        private string _selectedFieldCropType;
        private List<string> _tillageTypeOptions;
        private string _selectedFieldTillageType;
        private List<string> _optimizeOptions;
        private string _selectedOptimizsationOption;

        [Header("UI Buttons")]
        public GameObject[] _UIButtonObjs;
        private Button[] _UIButtonsCached = new Button[24];
        private Dictionary<UIButtonID, bool> _confirmButtonDic;
        private bool _fieldToWorkUponDecided;
        private int _fieldToWorkUponSelectedId;
        private bool _usingDefaultSetup;


        [Header("Field Output Button Rect Transforms")]
        public RectTransform[] _fieldOutputButRects;

        [Header("Field Output Cur Sel Text Obj")]
        public TMP_Text _curSelFieldText;
        public TMP_Text _curSelCropText;
        public TMP_Text _curSelSizeText;
        public TMP_Text _curSelSoilText;
        public TMP_Text _curSelCostText;

        [Header("Attrribute Text Obj")]
        public TMP_Text[] _attrributeText;//Must corrispond to references in enum UIAttrributeTextID
        private const string _cachedAttrributeStrA = "Biodiversity";
        private const string _cachedAttrributeStrB = "Yield";
        private const string _cachedAttrributeStrC = "Soil Health";
        private const string _cachedAttrributeStrD = "Fuel/Fibre Yield";
        private const string _cachedAttrributeStrE = "Carbon Sequestration";
        private const string _cachedAttrributeStrF = "Asethetic/Recreation";
        private const string _cachedAttrributeStrG = "Sustainability";

        [Header("Dialogue Text Obj")]
        public TMP_Text _characterDialogueText;
        private string[] _characterDialogueStr = new string[13];

        [Header("Logo Obj")]
        public LogoObj _logoObj;
        [Header("Logo UI RawImg")]
        public RawImage _logoRawImgUI;

        [Header("Cursor Objs")]
        public GameObject _cursorObj; //In order of Cursor_Plat_Hedgerow
        private CursorController _cursorController;

        [Header("Root-Node Activation Detail Text Obj")]
        public TMP_Text _ruleScopeText;

        [Header("Coins Cost Text Obj")]
        public TMP_Text _coinsText;



        private const int _timerCount = 7;
        private GameObject[] _timerObjs = new GameObject[7];
        private WTimer[] _timers = new WTimer[7];
        private float[] _waitTimes = new float[7];// = 10.0f;

        [Header("FPS Obj")]
        public GameObject _fpsObj;



        //EnableRootNodeScoringMsgWindow()
        private Action[] _actionListener = new Action[11];

        private void OnDisable()
        {
            EventManager.Unsubscribe("Report_Placement_OnBorder_Hedgerow", _actionListener[0]);
            EventManager.Unsubscribe("Report_OnClick_Hedgerow", _actionListener[1]);
            EventManager.Unsubscribe("Report_OnClick_Fence", _actionListener[2]);
            EventManager.Unsubscribe("Report_Left_Hedgerow_Award", _actionListener[3]);
            EventManager.Unsubscribe("Report_Right_Hedgerow_Award", _actionListener[4]);
            EventManager.Unsubscribe("Report_Top_Hedgerow_Award", _actionListener[5]);
            EventManager.Unsubscribe("Report_Bottom_Hedgerow_Award", _actionListener[6]);
            EventManager.Unsubscribe("Report_AllSides_Hedgerow_Award", _actionListener[7]);
            EventManager.Unsubscribe("Report_OnClick_Spade_Removal", _actionListener[8]);
            EventManager.Unsubscribe("Report_OnClick_Tree", _actionListener[9]);
            EventManager.Unsubscribe("Report_OnClick_Waterpool", _actionListener[10]);
        }
        private void EnableRootNodeScoringMsgWindow()
        {
            //if (_timers[(int)UITimerID.Timer_RootNode_Win_Display].FinishTimeRecord())

            //SetRewardUIWindow(true);
            //SetWindow(UIWindowID.Window_Badge_Reward, true);
            //_rewardController
            //_badgeRewardController.PlayIntroBadgeAnimation();

            StartTimer(UITimerID.Timer_RootNode_Win_Display);
            SetWindow(UIWindowID.Window_RootNode_Activation, true);
            //SetWindow(UIWindowID.Window_Character_Face, true);

            _ruleScopeText.text = "";
            string str = RulescopeManager._instance.GetRulescope().GetOutputStr();//_ruleScope.GetOutputStr();
            _ruleScopeText.text = str;

            SetWindow(UIWindowID.Window_TotalSustainability_Rating, true);
            SetAttrributeText(UIAttrributeTextID.Text_TotalSustainability);

            ProcessCoinUIUpdate();

            //SetRewardUIWindow(true, 1);
        }
        private void ProcessCoinUIUpdate()
        {
            int coins = RulescopeManager._instance.GetRulescope().GetCoinCount();//_ruleScope.GetCoinCount();
            if (_coinsText != null) _coinsText.text = coins.ToString();
            //SetAttrributeText(UIAttrributeTextID.Text_TotalSustainability);
        }
        private void ProcessDigRemoval()
        {
            SetAttrributeText(UIAttrributeTextID.Text_Yield);
        }
        private void ProcessHedgeAwardUIUpdate(int hedgeAwardId)
        {
            //hedgeAwardId
            // 0 = left,
            // 1 = right,
            // 2 = top,
            // 3 = bot,
            // 4 = all sides.

            int curField = RulescopeManager._instance.GetRulescope().GetSelectedField();
            curField = curField + 1;
            string fieldIdStr = "\nField #" + curField.ToString();

            if (hedgeAwardId == 0) //LEFT HEDGE AWARD
            {
                if (_ruleScopeText != null) _ruleScopeText.text = fieldIdStr + "\n\nEntire LEFT Hedgerow Award detected!\nExecute reward event chain.";


                return;
            }
            if (hedgeAwardId == 1) //RIGHT HEDGE AWARD
            {
                if (_ruleScopeText != null) _ruleScopeText.text = fieldIdStr + "\n\nEntire RIGHT Hedgerow Award detected!\nExecute reward event chain.";


                return;
            }
            if (hedgeAwardId == 2) //TOP HEDGE AWARD
            {
                if (_ruleScopeText != null) _ruleScopeText.text = fieldIdStr + "\n\nEntire TOP Hedgerow Award detected!\nExecute reward event chain.";


                return;
            }
            if (hedgeAwardId == 3) //BOTTOM HEDGE AWARD
            {
                if (_ruleScopeText != null) _ruleScopeText.text = fieldIdStr + "\n\nEntire BOTTOM Hedgerow Award detected!\nExecute reward event chain.";


                return;
            }
            if (hedgeAwardId == 4) //ALL HEDGE AWARD
            {
                if (_ruleScopeText != null) _ruleScopeText.text = "\nALL SIDES Hedgerow Award detected! Execute reward event chain.";


                return;
            }


            //int correctHedgerowsMade = RulescopeManager._instance.GetRulescope().GetCorrectHedgerowPlacementCount();

            ////SetWindow(UIWindowID.Window_Badge_Reward, true);
            //SetRewardUIWindow(true, correctHedgerowsMade);
            ////_badgeRewardController.PlayIntroBadgeAnimation();


        }
        private void ProcessHedgerow() => ProcessCoinUIUpdate();
        private void ProcessFence() => ProcessCoinUIUpdate();
        private void ProcessTree() => ProcessCoinUIUpdate();
        private void ProcessWaterpool() => ProcessCoinUIUpdate();
        private void ProcessHedgeLeftAward() => ProcessHedgeAwardUIUpdate(0);//{ }
        private void ProcessHedgeRightAward() => ProcessHedgeAwardUIUpdate(1);//{ }
        private void ProcessHedgeTopAward() => ProcessHedgeAwardUIUpdate(2);//{ }
        private void ProcessHedgeBotAward() => ProcessHedgeAwardUIUpdate(3);//{ }
        private void ProcessHedgeAllAward() => ProcessHedgeAwardUIUpdate(4);//{ }
        private void Awake() => Setup();
        private void Setup()
        {
            if (_curSelFieldText != null) _curSelFieldText.text = "Empty";
            if (_curSelCropText != null) _curSelCropText.text = "Undefined";
            if (_curSelSizeText != null) _curSelSizeText.text = "Undefined";
            if (_curSelSoilText != null) _curSelSoilText.text = "Undefined";
            if (_curSelCostText != null) _curSelCostText.text = "0";

            if (_characterDialogueText != null) _characterDialogueText.text = "...";

            _characterDialogueStr[0] = "Hello! Welcome to the Agroecology Calculator.";
            _characterDialogueStr[1] = "This is a placeholder tutorial. A simple check in with input control.";
            _characterDialogueStr[2] = "Select your field to work on and then confirm. Now you can access your tools.";
            _characterDialogueStr[3] = "To do this, middle click mouse wheel. This opens up your tool menu.";
            _characterDialogueStr[4] = "You can cycle through and select the one you'd like to work with.";
            _characterDialogueStr[5] = "It is at this point, you're now able to make changes and control camera movement.";
            _characterDialogueStr[6] = "You can pan the camera by holding down left mouse and moving.";
            _characterDialogueStr[7] = "Additionally you rotate by holding down right mouse button.";
            _characterDialogueStr[8] = "Finally you're able to zoom in/out by scrolling the mouse wheel.";
            _characterDialogueStr[9] = "";
            _characterDialogueStr[10] = "";

            _characterDialogueStr[11] = "Well done!";
            _characterDialogueStr[12] = "This hedgerow placement will help.";

            _selectedFieldCount = "1";
            _selectedFieldGridSize = "Extra Small";
            _selectedFieldSoilType = "Peat Soil";
            _selectedFieldCropType = "None (Saps)";
            _selectedFieldTillageType = "Standard Tillage";

            _usingDefaultSetup = false;

            InitCursors();
            InitDropdowns();
            InitButtons();
            InitTimers();

            if (_rewardControllerObj != null)
            {
                _rewardControllerCached = _rewardControllerObj.GetComponent<RewardController>();
            }

            SetButton(UIButtonID.Button_Generate, false);
            DisableAllUI();
            //SetWindow(UIWindowID.Window_Start, true);

            //_toolboxWheel.ApplySoilSelection(_fieldGenerator.GetFieldType());

            _actionListener[0] = new Action(EnableRootNodeScoringMsgWindow);//DelegateMethod()
            _actionListener[1] = new Action(ProcessHedgerow);//DelegateMethod()
            _actionListener[2] = new Action(ProcessFence);//DelegateMethod()

            _actionListener[3] = new Action(ProcessHedgeLeftAward);//DelegateMethod()
            _actionListener[4] = new Action(ProcessHedgeRightAward);//DelegateMethod()
            _actionListener[5] = new Action(ProcessHedgeTopAward);//DelegateMethod()
            _actionListener[6] = new Action(ProcessHedgeBotAward);//DelegateMethod()
            _actionListener[7] = new Action(ProcessHedgeAllAward);//DelegateMethod()

            _actionListener[8] = new Action(ProcessDigRemoval);//DelegateMethod()
            _actionListener[9] = new Action(ProcessTree);//DelegateMethod()
            _actionListener[10] = new Action(ProcessWaterpool);//DelegateMethod()


            EventManager.Subscribe("Report_Placement_OnBorder_Hedgerow", _actionListener[0]);
            EventManager.Subscribe("Report_OnClick_Hedgerow", _actionListener[1]);
            EventManager.Subscribe("Report_OnClick_Fence", _actionListener[2]);

            EventManager.Subscribe("Report_Left_Hedgerow_Award", _actionListener[3]);
            EventManager.Subscribe("Report_Right_Hedgerow_Award", _actionListener[4]);
            EventManager.Subscribe("Report_Top_Hedgerow_Award", _actionListener[5]);
            EventManager.Subscribe("Report_Bottom_Hedgerow_Award", _actionListener[6]);
            EventManager.Subscribe("Report_AllSides_Hedgerow_Award", _actionListener[7]);

            EventManager.Subscribe("Report_OnClick_Spade_Removal", _actionListener[8]);
            EventManager.Subscribe("Report_OnClick_Tree", _actionListener[9]);
            EventManager.Subscribe("Report_OnClick_Waterpool", _actionListener[10]);

            //SetupNodes();
        }

        private void SetMouseCursor(bool enabled, MouseClickStateID stateID)
        {
            if (enabled) Cursor.visible = false;
            if (!enabled) Cursor.visible = true;

            _cursorObj.SetActive(enabled);
            _cursorController.Process(enabled, stateID);
            //_cursorActiveDic[(int)UICursorID.Cursor_Plat_Hedgerow].Process(enabled);// = true;
        }
       

        public void GoBackFieldViewButtonPress()
        {
            AudioManager.PlaySFX(AudioID.Audio_But_Click01_SFX);

            DisableAllUI();
            FPSDisplay(true);
            _fieldToWorkUponDecided = false; //Guard clause in button screen position update.
            SetFieldOutputButtons(true);

            _timers[(int)UITimerID.Timer_Field_Select_But_Update].Reset();
            _timers[(int)UITimerID.Timer_Field_Select_But_Update].StartTimer(0.25f);

            _fieldGenerator._cameraManager.EnableStartFieldViewsPoint();

            if (IsSelectedForPerformance())
                _fieldGenerator.RenableAllFieldCrops();
        }
        public void ToolboxCycleButtonPress(int dir)
        {
            if (dir == -1)//Left
            {
                AudioManager.PlaySFX(AudioID.Audio_But_Click02_SFX);
                _toolboxWheel.RotateLeft();
                return;
            }
            if (dir == 1)//Right
            {
                AudioManager.PlaySFX(AudioID.Audio_But_Click03_SFX);
                _toolboxWheel.RotateRight();
                return;
            }
        }
        public void ToolboxEnableButtonPress()
        {
            bool checkFlag = false;

            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Soil_Peat) checkFlag = true;
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Soil_Light) checkFlag = true;
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Soil_Heavy) checkFlag = true;
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Hedgerow) checkFlag = true;
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Fence) checkFlag = true;
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Tree) checkFlag = true;
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Wildflower) checkFlag = true;
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Waterpool) checkFlag = true;
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Nothing) checkFlag = true;


            if (checkFlag)
            {
                AudioManager.PlaySFX(AudioID.Audio_But_Click01_SFX);
                SetMouseCursor(false, MouseClickStateID.OnClick_Nothing);
                SetWindow(UIWindowID.Window_Toolbox_Selection, true);
                SetButton(UIButtonID.Button_Toolbox_Enable, true, "ON");
                _cameraManager.EnableToolboxVisuals(true);
                EventManager.SetMouseClickStateID(MouseClickStateID.OnClick_Tool_Selection);
                _toolboxWheel.ApplySoilSelection(_fieldGenerator.GetFieldType());
                return;
            }
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Tool_Selection)
            {
                AudioManager.PlaySFX(AudioID.Audio_But_Click01_SFX);
                SetWindow(UIWindowID.Window_Toolbox_Selection, false);
                SetButton(UIButtonID.Button_Toolbox_Enable, true, "OFF");
                _cameraManager.EnableToolboxVisuals(false);
                EventManager.SetMouseClickStateID(MouseClickStateID.OnClick_Nothing);

                if (_toolboxWheel._curToolText.text == "Fence")
                {
                    FenceToolButtonPress();
                }
                if (_toolboxWheel._curToolText.text == "Hedgerow")
                {
                    HedgerowToolButtonPress();
                }
                if (_toolboxWheel._curToolText.text == "Tree")
                {
                    TreeToolButtonPress();
                }
                if (_toolboxWheel._curToolText.text == "WildFlower")
                {
                    WildflowerToolButtonPress();
                }
                if (_toolboxWheel._curToolText.text == "Water")
                {
                    WaterpoolToolButtonPress();
                }
                if (_toolboxWheel._curToolText.text == "Spade P" ||
                    _toolboxWheel._curToolText.text == "Spade L" ||
                    _toolboxWheel._curToolText.text == "Spade H")
                {
                    SpadeToolButtonPress();
                }

                return;
            }
        }
        public void FenceToolButtonPress()
        {
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Nothing)
            {
                EventManager.SetMouseClickStateID(MouseClickStateID.OnClick_Paint_Cell_Fence);
                SetMouseCursor(true, MouseClickStateID.OnClick_Paint_Cell_Fence);
                return;
            }
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Fence)
            {
                EventManager.SetMouseClickStateID(MouseClickStateID.OnClick_Nothing);
                SetMouseCursor(false, MouseClickStateID.OnClick_Nothing);
                return;
            }
        }
        public void HedgerowToolButtonPress()
        {
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Nothing)
            {
                EventManager.SetMouseClickStateID(MouseClickStateID.OnClick_Paint_Cell_Hedgerow);
                //SetButton(UIButtonID.Button_Tool_Hedgerow, true, "ON");
                SetMouseCursor(true, MouseClickStateID.OnClick_Paint_Cell_Hedgerow);
                return;
            }
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Hedgerow)
            {
                EventManager.SetMouseClickStateID(MouseClickStateID.OnClick_Nothing);
                //SetButton(UIButtonID.Button_Tool_Hedgerow, true, "OFF");
                SetMouseCursor(false, MouseClickStateID.OnClick_Nothing);
                return;
            }
        }
        public void TreeToolButtonPress()
        {
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Nothing)
            {
                EventManager.SetMouseClickStateID(MouseClickStateID.OnClick_Paint_Cell_Tree);
                //SetButton(UIButtonID.Button_Tool_Hedgerow, true, "ON");
                SetMouseCursor(true, MouseClickStateID.OnClick_Paint_Cell_Tree);
                return;
            }
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Tree)
            {
                EventManager.SetMouseClickStateID(MouseClickStateID.OnClick_Nothing);
                //SetButton(UIButtonID.Button_Tool_Hedgerow, true, "OFF");
                SetMouseCursor(false, MouseClickStateID.OnClick_Nothing);
                return;
            }
        }
        public void WildflowerToolButtonPress()
        {
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Nothing)
            {
                EventManager.SetMouseClickStateID(MouseClickStateID.OnClick_Paint_Cell_Wildflower);
                //SetButton(UIButtonID.Button_Tool_Hedgerow, true, "ON");
                SetMouseCursor(true, MouseClickStateID.OnClick_Paint_Cell_Wildflower);
                return;
            }
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Wildflower)
            {
                EventManager.SetMouseClickStateID(MouseClickStateID.OnClick_Nothing);
                //SetButton(UIButtonID.Button_Tool_Hedgerow, true, "OFF");
                SetMouseCursor(false, MouseClickStateID.OnClick_Nothing);
                return;
            }
        }
        private void WaterpoolToolButtonPress()
        {
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Nothing)
            {
                EventManager.SetMouseClickStateID(MouseClickStateID.OnClick_Paint_Cell_Waterpool);
                //SetButton(UIButtonID.Button_Tool_Hedgerow, true, "ON");
                SetMouseCursor(true, MouseClickStateID.OnClick_Paint_Cell_Waterpool);
                return;
            }
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Waterpool)
            {
                EventManager.SetMouseClickStateID(MouseClickStateID.OnClick_Nothing);
                //SetButton(UIButtonID.Button_Tool_Hedgerow, true, "OFF");
                SetMouseCursor(false, MouseClickStateID.OnClick_Nothing);
                return;
            }
        }
        public void SpadeToolButtonPress()
        {

            MouseClickStateID mouseStateToSet = MouseClickStateID.OnClick_Nothing;

            if (_fieldGridTooAdd[0].SOIL_TYPE == FieldGridCellID.FieldGridCell_Soil_Peat) mouseStateToSet = MouseClickStateID.OnClick_Paint_Cell_Soil_Peat;
            if (_fieldGridTooAdd[0].SOIL_TYPE == FieldGridCellID.FieldGridCell_Soil_Light) mouseStateToSet = MouseClickStateID.OnClick_Paint_Cell_Soil_Light;
            if (_fieldGridTooAdd[0].SOIL_TYPE == FieldGridCellID.FieldGridCell_Soil_Heavy) mouseStateToSet = MouseClickStateID.OnClick_Paint_Cell_Soil_Heavy;

            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Nothing)
            {
                EventManager.SetMouseClickStateID(mouseStateToSet);
                //SetButton(UIButtonID.Button_Tool_SpadeDig, true, "ON");
                SetMouseCursor(true, mouseStateToSet);
                return;
            }

            bool checkFlag = false;

            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Soil_Peat) checkFlag = true;
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Soil_Light) checkFlag = true;
            if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Paint_Cell_Soil_Heavy) checkFlag = true;

            if (checkFlag)
            {
                EventManager.SetMouseClickStateID(MouseClickStateID.OnClick_Nothing);
                //SetButton(UIButtonID.Button_Tool_SpadeDig, true, "OFF");
                SetMouseCursor(false, MouseClickStateID.OnClick_Nothing);
                return;
            }

        }
        public void FieldButtonPress(int fieldId)
        {
            AudioManager.PlaySFX(AudioID.Audio_But_Click03_SFX);
            RulescopeManager._instance.GetRulescope().SetSelectedField(fieldId-1);
            _curSelectedFieldId = fieldId;
            SetFieldOutputButtons(true);
            //EventManager.TriggerEvent("Process_Default_Auto_Fence_Placements");

            if (_curSelFieldText != null)
            {
                string crop = "...";
                string size = "...";
                string soil = "...";
                string cost = "0";
                int expenditure = 0;

                if (_fieldGridTooAdd[fieldId - 1].SURFACE_TYPE == SurfaceGridCellID.Surface_Type_SugarBeat) crop = "Sugar Beat";
                if (_fieldGridTooAdd[fieldId - 1].SURFACE_TYPE == SurfaceGridCellID.Surface_Type_OilSeedRape) crop = "Oil Seed Rape";
                if (_fieldGridTooAdd[fieldId - 1].SURFACE_TYPE == SurfaceGridCellID.Surface_Type_Beans) crop = "Beans";
                if (_fieldGridTooAdd[fieldId - 1].SURFACE_TYPE == SurfaceGridCellID.Surface_Type_Wheat) crop = "Wheat";
                if (_fieldGridTooAdd[fieldId - 1].SURFACE_TYPE == SurfaceGridCellID.Surface_Type_None) crop = "None (saps)";

                if (_fieldGridTooAdd[fieldId - 1].X == 5) size = "Extra Small";
                if (_fieldGridTooAdd[fieldId - 1].X == 10) size = "Small";
                if (_fieldGridTooAdd[fieldId - 1].X == 15) size = "Medium";
                if (_fieldGridTooAdd[fieldId - 1].X == 20) size = "Large";
                if (_fieldGridTooAdd[fieldId - 1].X == 22) size = "Default L";

                if (_fieldGridTooAdd[fieldId - 1].SOIL_TYPE == FieldGridCellID.FieldGridCell_Soil_Peat) soil = "Peat";
                if (_fieldGridTooAdd[fieldId - 1].SOIL_TYPE == FieldGridCellID.FieldGridCell_Soil_Light) soil = "Light";
                if (_fieldGridTooAdd[fieldId - 1].SOIL_TYPE == FieldGridCellID.FieldGridCell_Soil_Heavy) soil = "Heavy";

                expenditure = RulescopeManager._instance.GetRulescope().GetFieldExpenditure(fieldId - 1);
                cost = expenditure.ToString();

                if (fieldId == 1)
                {
                    _curSelFieldText.text = "Field #1";
                }
                if (fieldId == 2)
                {
                    _curSelFieldText.text = "Field #2";
                }
                if (fieldId == 3)
                {
                    _curSelFieldText.text = "Field #3";
                }
                if (fieldId == 4)
                {
                    _curSelFieldText.text = "Field #4";
                }
                if (_curSelCropText != null) _curSelCropText.text = crop;
                if (_curSelSizeText != null) _curSelSizeText.text = size;
                if (_curSelSoilText != null) _curSelSoilText.text = soil;
                if (_curSelCostText != null) _curSelCostText.text = cost;
            }




            SetWindow(UIWindowID.Window_Side_Panel, true);
            SetWindow(UIWindowID.Window_CurField_Panel, true);
            SetWindow(UIWindowID.Window_Cur_Crop, true);
            SetWindow(UIWindowID.Window_Cur_Soil, true);
            SetWindow(UIWindowID.Window_Cur_Size, true);
            SetWindow(UIWindowID.Window_Cur_Cost, true);



            SetWindow(UIWindowID.Window_Energy_Cost, false);
            SetWindow(UIWindowID.Window_Time_Line, false);
            SetWindow(UIWindowID.Window_GoBack_FieldViews, true);
            SetWindow(UIWindowID.Window_Toolbox_Enable, false);
            SetWindow(UIWindowID.Window_Character_Face, false);



            if (_fieldToWorkUponSelectedId == -1)
            {
                _fieldToWorkUponSelectedId = fieldId;
                _fieldToWorkUponDecided = false;
                if (fieldId == 1) SetButton(UIButtonID.Button_Field_Output_1, true, "Confirm #1");
                if (fieldId == 2) SetButton(UIButtonID.Button_Field_Output_2, true, "Confirm #2");
                if (fieldId == 3) SetButton(UIButtonID.Button_Field_Output_3, true, "Confirm #3");
                if (fieldId == 4) SetButton(UIButtonID.Button_Field_Output_4, true, "Confirm #4");

                _fieldGenerator._cameraManager.ChangePivotPoint(fieldId - 1);
                return;
            }
            if (_fieldToWorkUponSelectedId == fieldId) //Selected the saem field to confirm working on it.
            {
                //Reset the button desc ready for for next round, as we now disable these buttons.
                SetButton(UIButtonID.Button_Field_Output_1, true, "Field #1");
                SetButton(UIButtonID.Button_Field_Output_2, true, "Field #2");
                SetButton(UIButtonID.Button_Field_Output_3, true, "Field #3");
                SetButton(UIButtonID.Button_Field_Output_4, true, "Field #4");

                //if (_usingDefaultSetup) SetButton(UIButtonID.Button_Field_Output_1, true, "View Status");

                SetButton(UIButtonID.Button_Field_Output_1, false);
                SetButton(UIButtonID.Button_Field_Output_2, false);
                SetButton(UIButtonID.Button_Field_Output_3, false);
                SetButton(UIButtonID.Button_Field_Output_4, false);

                //SetWindow(UIWindowID.Window_Tool_Hedgerow, true);
                //SetWindow(UIWindowID.Window_Tool_SpadeDig, true);
                SetWindow(UIWindowID.Window_Toolbox_Enable, true);
                SetWindow(UIWindowID.Window_Energy_Cost, true);
                SetWindow(UIWindowID.Window_CurField_Panel, true);
                SetWindow(UIWindowID.Window_Side_Panel, true);


                SetWindow(UIWindowID.Window_Cur_Crop, false);
                SetWindow(UIWindowID.Window_Cur_Soil, false);
                SetWindow(UIWindowID.Window_Cur_Size, false);
                SetWindow(UIWindowID.Window_Cur_Cost, false);

                SetWindow(UIWindowID.Window_Time_Line, true);
                SetWindow(UIWindowID.Window_Biodiversity_Rating, true);
                SetWindow(UIWindowID.Window_Yield_Rating, true);
                SetWindow(UIWindowID.Window_SoilHealth_Rating, true);
                SetWindow(UIWindowID.Window_FuelFibreYield_Rating, true);
                SetWindow(UIWindowID.Window_CarbonSequestration_Rating, true);
                SetWindow(UIWindowID.Window_AsetheticRecreation_Rating, true);

                SetWindow(UIWindowID.Window_TotalSustainability_Rating, true);
                ResetAttrributeText();

                SetWindow(UIWindowID.Window_GoBack_FieldViews, true);

                int width = _fieldGridTooAdd[fieldId - 1].X;

                _cameraManager.EnableFieldStartPivotPoint(fieldId - 1, width, _usingDefaultSetup);

                if (!EventManager.HasInitCharacterDialogueComplete())
                {
                    CharacterPrefabID randomCharacter = CharacterPrefabID.Character_Human_Farmer_A;
                    int decision = 0;

                    decision = wildlogicgames.Utilities.GetRandomNumberInt(0, 100);

                    if (decision < 50)
                        randomCharacter = CharacterPrefabID.Character_Human_Farmer_A;
                    if (decision >= 50)
                        randomCharacter = CharacterPrefabID.Character_Human_Farmer_B;

                    CharacterManager.SpawnCharacterPosition(randomCharacter, fieldId - 1);
                    float waitTime = _waitTimes[(int)UITimerID.Timer_Initial_Character_Welcome];
                    _timers[(int)UITimerID.Timer_Initial_Character_Welcome].StartTimer(waitTime);

                    //SetWindow(UIWindowID.Window_Dialogue_Panel, true);

                    SetWindow(UIWindowID.Window_CurField_Panel, false);
                    SetWindow(UIWindowID.Window_Energy_Cost, false);
                    SetWindow(UIWindowID.Window_Time_Line, false);

                    SetWindow(UIWindowID.Window_Biodiversity_Rating, false);
                    SetWindow(UIWindowID.Window_Yield_Rating, false);
                    SetWindow(UIWindowID.Window_SoilHealth_Rating, false);
                    SetWindow(UIWindowID.Window_FuelFibreYield_Rating, false);
                    SetWindow(UIWindowID.Window_CarbonSequestration_Rating, false);
                    SetWindow(UIWindowID.Window_AsetheticRecreation_Rating, false);
                    SetWindow(UIWindowID.Window_TotalSustainability_Rating, false);

                    SetWindow(UIWindowID.Window_GoBack_FieldViews, false);
                    SetWindow(UIWindowID.Window_Toolbox_Enable, false);
                }

                _fieldToWorkUponDecided = true; //Guard clause in button screen position update.

                if (IsSelectedForPerformance())
                {
                    RulescopeManager._instance.GetRulescope().SetSelectedField(fieldId - 1);
                    _fieldGenerator.DisableUnselectedFieldCrops(fieldId - 1);
                }
                
                return;
            }
            if (_fieldToWorkUponSelectedId != -1 && _fieldToWorkUponSelectedId != fieldId)
            {
                //Not initial selection, nor confirming second click on chosen field. 
                //Ie a different selection occured after initally selecting another.

                SetButton(UIButtonID.Button_Field_Output_1, true, "Field #1");
                SetButton(UIButtonID.Button_Field_Output_2, true, "Field #2");
                SetButton(UIButtonID.Button_Field_Output_3, true, "Field #3");
                SetButton(UIButtonID.Button_Field_Output_4, true, "Field #4");

                _fieldToWorkUponSelectedId = fieldId;
                _fieldToWorkUponDecided = false;
                if (fieldId == 1) SetButton(UIButtonID.Button_Field_Output_1, true, "Confirm #1");
                if (fieldId == 2) SetButton(UIButtonID.Button_Field_Output_2, true, "Confirm #2");
                if (fieldId == 3) SetButton(UIButtonID.Button_Field_Output_3, true, "Confirm #3");
                if (fieldId == 4) SetButton(UIButtonID.Button_Field_Output_4, true, "Confirm #4");

                //if (_usingDefaultSetup) SetButton(UIButtonID.Button_Field_Output_1, true, "Start Building");

                _fieldGenerator._cameraManager.ChangePivotPoint(fieldId - 1);
                return;
            }
        }
        public void RunButtonPress()
		{
            AudioManager.PlaySFX(AudioID.Audio_But_Click01_SFX);
            SetWindow(UIWindowID.Window_Start, false);
            SetWindow(UIWindowID.Window_Choice_Setup, true);
            //SetWindow(UIWindowID.Window_Control, true);
            SetButton(UIButtonID.Button_Add_Crops, false, null);
            SetButton(UIButtonID.Button_Add_Tillage, false, null);
            FPSDisplay(true);

            _logoObj.gameObject.SetActive(false);
            _logoRawImgUI.gameObject.SetActive(false);
        }
        public void ExitButtonPress()
		{
            AudioManager.PlaySFX(AudioID.Audio_But_Click02_SFX);
            Application.Quit();
		}
        public void ManualButtonPress()
        {
            AudioManager.PlaySFX(AudioID.Audio_But_Click03_SFX);
            SetWindow(UIWindowID.Window_Choice_Setup, false);
            SetWindow(UIWindowID.Window_Control, true);
            _logoObj.SetLogoPos(LogoScreenPosID.Logo_Pos_BotRight);
        }
        public void RandomButtonPress()
        {
            //...ect
        }
        public void DefaultButtonPress()
        {
            //...ect
        }
        public void AddFieldsButtonPress()
        {
            AudioManager.PlaySFX(AudioID.Audio_But_Click02_SFX);
            SetWindow(UIWindowID.Window_Field_Start, true);
            SetButton(UIButtonID.Button_Add_Fields, false, null);
            SetWindow(UIWindowID.Window_Control, false);
            //SetButton(UIButtonID.Button_Confirm_Field_Count, false, null);
        }
        public void AddCropsButtonPress()
        {
            AudioManager.PlaySFX(AudioID.Audio_But_Click02_SFX);
            SetButton(UIButtonID.Button_Add_Crops, false, null);
            SetButton(UIButtonID.Button_Add_Tillage, true, null);

            SetWindow(UIWindowID.Window_Crops_1, true);
            SetWindow(UIWindowID.Window_Control, false);

        }
        public void AddTillageButtonPress()
        {
            AudioManager.PlaySFX(AudioID.Audio_But_Click02_SFX);
            SetButton(UIButtonID.Button_Add_Tillage, false, null);

            SetWindow(UIWindowID.Window_Tillage_1, true);
            SetWindow(UIWindowID.Window_Control, false);


        }
        public void ConfirmFieldTotalButtonPress()
        {
            AudioManager.PlaySFX(AudioID.Audio_But_Click03_SFX);
            SetButton(UIButtonID.Button_Confirm_Field_Count, false, "locked");
            SetDropdownState(UIDropdownID.Dropdown_Field_Count, false);
            SetDropdownState(UIDropdownID.Dropdown_Soil_Selection, false);
            SetButton(UIButtonID.Button_Add_Crops, true, null);

            
            SetButton(UIButtonID.Button_Confirm_Field_1, true, null); //Prevent confirmation without selecting dropdown option. We set true there.


            int count = int.Parse(_selectedFieldCount);
            _fieldCount = count;

            for (int i = 0; i < _fieldCount; i++)
            {
                _fieldGridTooAdd[i].X = 5;
                _fieldGridTooAdd[i].Y = 5;
                _fieldGridTooAdd[i].CELL_SIZE = 0.5f;
                _fieldGridTooAdd[i].SURFACE_TYPE = SurfaceGridCellID.Surface_Type_None;
                _fieldGridTooAdd[i].TILLAGE_TYPE = TillageGridCellID.Tillage_Type_Standard;
            }

            _confirmButtonDic = new Dictionary<UIButtonID, bool>();
            _confirmButtonDic.Add(UIButtonID.Button_Confirm_Field_1, false);
            _confirmButtonDic.Add(UIButtonID.Button_Confirm_Field_2, false);
            _confirmButtonDic.Add(UIButtonID.Button_Confirm_Field_3, false);
            _confirmButtonDic.Add(UIButtonID.Button_Confirm_Field_4, false);
            _confirmButtonDic.Add(UIButtonID.Button_Confirm_Crops_1, false);
            _confirmButtonDic.Add(UIButtonID.Button_Confirm_Crops_2, false);
            _confirmButtonDic.Add(UIButtonID.Button_Confirm_Crops_3, false);
            _confirmButtonDic.Add(UIButtonID.Button_Confirm_Crops_4, false);
            _confirmButtonDic.Add(UIButtonID.Button_Confirm_Tillage_1, false);
            _confirmButtonDic.Add(UIButtonID.Button_Confirm_Tillage_2, false);
            _confirmButtonDic.Add(UIButtonID.Button_Confirm_Tillage_3, false);
            _confirmButtonDic.Add(UIButtonID.Button_Confirm_Tillage_4, false);


            SetWindow(UIWindowID.Window_Field_Start, false);
            SetWindow(UIWindowID.Window_Field_1, true);


        }
        public void ConfirmFieldButtonPress(int fieldWinId)
        {
            AudioManager.PlaySFX(AudioID.Audio_But_Click03_SFX);
            SetWindow(UIWindowID.Window_Field_Start, false);
            SetWindow(UIWindowID.Window_Field_1, false);
            SetWindow(UIWindowID.Window_Field_2, false);
            SetWindow(UIWindowID.Window_Field_3, false);
            SetWindow(UIWindowID.Window_Field_4, false);

            switch (fieldWinId)
            {
                case 1:
                    SetButton(UIButtonID.Button_Confirm_Field_1, false, "locked");
                    _confirmButtonDic[UIButtonID.Button_Confirm_Field_1] = true;
                    SetDropdownState(UIDropdownID.Dropdown_Field_Size_1, false);
                    if (_fieldCount > 1) SetWindow(UIWindowID.Window_Field_2, true);
                    else
                    {
                        SetWindow(UIWindowID.Window_Control, true);
                        SetButton(UIButtonID.Button_Add_Fields, false, "- Field");
                        SetButton(UIButtonID.Button_Add_Crops, true, "+ Crop");
                    }
                    break;
                case 2:
                    SetButton(UIButtonID.Button_Confirm_Field_2, false, "locked");
                    _confirmButtonDic[UIButtonID.Button_Confirm_Field_2] = true;
                    SetDropdownState(UIDropdownID.Dropdown_Field_Size_2, false);
                    if (_fieldCount > 2) SetWindow(UIWindowID.Window_Field_3, true);
                    else
                    {
                        SetWindow(UIWindowID.Window_Control, true);
                        SetButton(UIButtonID.Button_Add_Fields, false, "- Field");
                        SetButton(UIButtonID.Button_Add_Crops, true, "+ Crop");
                    }
                    break;
                case 3:
                    SetButton(UIButtonID.Button_Confirm_Field_3, false, "locked");
                    _confirmButtonDic[UIButtonID.Button_Confirm_Field_3] = true;
                    SetDropdownState(UIDropdownID.Dropdown_Field_Size_3, false);
                    if (_fieldCount > 3) SetWindow(UIWindowID.Window_Field_4, true);
                    else
                    {
                        SetWindow(UIWindowID.Window_Control, true);
                        SetButton(UIButtonID.Button_Add_Fields, false, "- Field");
                        SetButton(UIButtonID.Button_Add_Crops, true, "+ Crop");
                    }
                    break;
                case 4:
                    SetButton(UIButtonID.Button_Confirm_Field_4, false, "locked");
                    _confirmButtonDic[UIButtonID.Button_Confirm_Field_4] = true;
                    SetDropdownState(UIDropdownID.Dropdown_Field_Size_4, false);
                    SetWindow(UIWindowID.Window_Control, true);
                    SetButton(UIButtonID.Button_Add_Fields, false, "- Field");
                    SetButton(UIButtonID.Button_Add_Crops, true, "+ Crop");
                    break;
            }

            if (AreUIButtonsConfirmed()) SetButton(UIButtonID.Button_Generate, true);
        }
        public void ConfirmCropsButtonPress(int cropWinId)
        {
            AudioManager.PlaySFX(AudioID.Audio_But_Click03_SFX);
            //SetButton(UIButtonID.Button_ConfirmCrops, false, "locked");
            //SetButton(UIButtonID.Button_AddCrops, false, null);
            //SetButton(UIButtonID.Button_Generate, true);

            SetWindow(UIWindowID.Window_Field_Start, false);
            SetWindow(UIWindowID.Window_Crops_1, false);
            SetWindow(UIWindowID.Window_Crops_2, false);
            SetWindow(UIWindowID.Window_Crops_3, false);
            SetWindow(UIWindowID.Window_Crops_4, false);
            

            switch (cropWinId)
            {
                case 1:
                    SetButton(UIButtonID.Button_Confirm_Crops_1, false, "locked");
                    _confirmButtonDic[UIButtonID.Button_Confirm_Crops_1] = true;
                    SetDropdownState(UIDropdownID.Dropdown_Crop_1, false);
                    if (_fieldCount > 1) SetWindow(UIWindowID.Window_Crops_2, true);
                    else
                    {
                        SetWindow(UIWindowID.Window_Control, true);
                        SetButton(UIButtonID.Button_Add_Crops, false, "- Crop");
                        SetButton(UIButtonID.Button_Add_Tillage, true, "+ Tillage");
                    }

                    break;
                case 2:
                    SetButton(UIButtonID.Button_Confirm_Crops_2, false, "locked");
                    _confirmButtonDic[UIButtonID.Button_Confirm_Crops_2] = true;
                    SetDropdownState(UIDropdownID.Dropdown_Crop_2, false);
                    if (_fieldCount > 2) SetWindow(UIWindowID.Window_Crops_3, true);
                    else
                    {
                        SetWindow(UIWindowID.Window_Control, true);
                        SetButton(UIButtonID.Button_Add_Crops, false, "- Crop");
                        SetButton(UIButtonID.Button_Add_Tillage, true, "+ Tillage");
                    }
                    break;
                case 3:
                    SetButton(UIButtonID.Button_Confirm_Crops_3, false, "locked");
                    _confirmButtonDic[UIButtonID.Button_Confirm_Crops_3] = true;
                    SetDropdownState(UIDropdownID.Dropdown_Crop_3, false);
                    if (_fieldCount > 3) SetWindow(UIWindowID.Window_Crops_4, true);
                    else
                    {
                        SetWindow(UIWindowID.Window_Control, true);
                        SetButton(UIButtonID.Button_Add_Crops, false, "- Crop");
                        SetButton(UIButtonID.Button_Add_Tillage, true, "+ Tillage");
                    }
                    break;
                case 4:
                    SetButton(UIButtonID.Button_Confirm_Crops_4, false, "locked");
                    _confirmButtonDic[UIButtonID.Button_Confirm_Crops_4] = true;
                    SetDropdownState(UIDropdownID.Dropdown_Crop_4, false);
                    SetWindow(UIWindowID.Window_Control, true);
                    SetButton(UIButtonID.Button_Add_Crops, false, "- Crop");
                    SetButton(UIButtonID.Button_Add_Tillage, true, "+ Tillage");
                    break;
            }

            if (AreUIButtonsConfirmed()) SetButton(UIButtonID.Button_Generate, true);
        }
        public void ConfirmTillagesButtonPress(int cropWinId)
        {
            AudioManager.PlaySFX(AudioID.Audio_But_Click03_SFX);
            //SetButton(UIButtonID.Button_ConfirmCrops, false, "locked");
            //SetButton(UIButtonID.Button_AddCrops, false, null);
            //SetButton(UIButtonID.Button_Generate, true);
            //SetButton(UIButtonID.Button_Generate, true);

            SetWindow(UIWindowID.Window_Field_Start, false);
            SetWindow(UIWindowID.Window_Tillage_1, false);
            SetWindow(UIWindowID.Window_Tillage_2, false);
            SetWindow(UIWindowID.Window_Tillage_3, false);
            SetWindow(UIWindowID.Window_Tillage_4, false);

            switch (cropWinId)
            {
                case 1:
                    SetButton(UIButtonID.Button_Confirm_Tillage_1, false, "locked");
                    _confirmButtonDic[UIButtonID.Button_Confirm_Tillage_1] = true;
                    SetDropdownState(UIDropdownID.Dropdown_Tillage_1, false);
                    if (_fieldCount > 1) SetWindow(UIWindowID.Window_Tillage_2, true);
                    else
                    {
                        SetWindow(UIWindowID.Window_Control, true);
                        SetButton(UIButtonID.Button_Add_Tillage, false, "- Tillage");
                    }
                    break;
                case 2:
                    SetButton(UIButtonID.Button_Confirm_Tillage_2, false, "locked");
                    _confirmButtonDic[UIButtonID.Button_Confirm_Tillage_2] = true;
                    SetDropdownState(UIDropdownID.Dropdown_Tillage_2, false);
                    if (_fieldCount > 2) SetWindow(UIWindowID.Window_Tillage_3, true);
                    else
                    {
                        SetWindow(UIWindowID.Window_Control, true);
                        SetButton(UIButtonID.Button_Add_Tillage, false, "- Tillage");
                    }
                    break;
                case 3:
                    SetButton(UIButtonID.Button_Confirm_Tillage_3, false, "locked");
                    _confirmButtonDic[UIButtonID.Button_Confirm_Tillage_3] = true;
                    SetDropdownState(UIDropdownID.Dropdown_Tillage_3, false);
                    if (_fieldCount > 3) SetWindow(UIWindowID.Window_Tillage_4, true);
                    else
                    {
                        SetWindow(UIWindowID.Window_Control, true);
                        SetButton(UIButtonID.Button_Add_Tillage, false, "- Tillage");
                    }
                    break;
                case 4:
                    SetButton(UIButtonID.Button_Confirm_Tillage_4, false, "locked");
                    _confirmButtonDic[UIButtonID.Button_Confirm_Tillage_4] = true;
                    SetDropdownState(UIDropdownID.Dropdown_Tillage_4, false);
                    SetWindow(UIWindowID.Window_Control, true);
                    SetButton(UIButtonID.Button_Add_Tillage, false, "- Tillage");
                    break;
            }

            if (AreUIButtonsConfirmed())
            {
                SetButton(UIButtonID.Button_Generate, true);
                SetWindow(UIWindowID.Window_Control, false);
            }

            //SetButton(UIButtonID.Button_Generate, true);
        }
        private bool AreUIButtonsConfirmed()
        {
            for (int i = 1; i <= _fieldCount; i++)
            {
                UIButtonID fieldButton = GetFieldButtonID(i);
                UIButtonID cropsButton = GetCropsButtonID(i);
                UIButtonID tillageButton = GetTillageButtonID(i);

                if (!_confirmButtonDic.TryGetValue(fieldButton, out bool isFieldConfirmed) || !isFieldConfirmed)
                    return false;

                if (!_confirmButtonDic.TryGetValue(cropsButton, out bool isCropsConfirmed) || !isCropsConfirmed)
                    return false;

                if (!_confirmButtonDic.TryGetValue(tillageButton, out bool isTillageConfirmed) || !isTillageConfirmed)
                    return false;
            }

            //All buttons confrimed.
            return true;
        }

        private UIButtonID GetFieldButtonID(int index)
        {

            int baseValue = (int)UIButtonID.Button_Confirm_Field_1; 

            if (index >= 1 && index <= 4)
                return (UIButtonID)(baseValue + (index - 1));

            return UIButtonID.Button_None_Error;
        }
        private UIButtonID GetCropsButtonID(int index)
        {
            int baseValue = (int)UIButtonID.Button_Confirm_Crops_1; // 10

            if (index >= 1 && index <= 4)
                return (UIButtonID)(baseValue + (index - 1));

            return UIButtonID.Button_None_Error;
        }

        private UIButtonID GetTillageButtonID(int index)
        {
            int baseValue = (int)UIButtonID.Button_Confirm_Tillage_1; // 14

            if (index >= 1 && index <= 4)
                return (UIButtonID)(baseValue + (index - 1));

            return UIButtonID.Button_None_Error;
        }
        private void AddFieldGridSize(int id, int rows, int cols)
        {
            _fieldGridTooAdd[id].X = rows;
            _fieldGridTooAdd[id].Y = cols;
            _fieldGridTooAdd[id].CELL_SIZE = 0.5f;
            //_fieldGenerator.AddFieldGrid(id, _fieldGridTooAdd[id]);
        }
        private void AddFieldGridSoil(int id, FieldGridCellID soilType)
        {
            _fieldGridTooAdd[id].SOIL_TYPE = soilType;
        }
        private void AddFieldGridCrop(int id, SurfaceGridCellID cropType)
        {
            _fieldGridTooAdd[id].SURFACE_TYPE = cropType;
        }
        private void AddFieldGridTillage(int id, TillageGridCellID tillageType)
        {
            _fieldGridTooAdd[id].TILLAGE_TYPE = tillageType;
        }
        public void GenerateButtonPress()
		{
            if (_fieldGenerator == null) return;

            AudioManager.PlaySFX(AudioID.Audio_But_Click03_SFX);

            _fieldGenerator.SetFieldCount(_fieldCount);


            //see line 663
            //NEED TO DROP THIS APPROACH _fieldGenerator.SetFieldType(FieldGridCellID.FieldGridCell_Soil_Peat);
            if (_selectedFieldSoilType == "Peat Soil") _fieldGenerator.SetFieldType(FieldGridCellID.FieldGridCell_Soil_Peat);
            if (_selectedFieldSoilType == "Light Soil") _fieldGenerator.SetFieldType(FieldGridCellID.FieldGridCell_Soil_Light);
            if (_selectedFieldSoilType == "Heavy Soil") _fieldGenerator.SetFieldType(FieldGridCellID.FieldGridCell_Soil_Heavy);
            for (int i = 0; i < _fieldCount; i++)
            {
                if (_selectedFieldSoilType == "Peat Soil") AddFieldGridSoil(i, FieldGridCellID.FieldGridCell_Soil_Peat);
                if (_selectedFieldSoilType == "Light Soil") AddFieldGridSoil(i, FieldGridCellID.FieldGridCell_Soil_Light);
                if (_selectedFieldSoilType == "Heavy Soil") AddFieldGridSoil(i, FieldGridCellID.FieldGridCell_Soil_Heavy);
            }

            if (_selectedFieldCropType == "None (Saps)") _fieldGenerator.SetCropType(SurfaceGridCellID.Surface_Type_None);
            if (_selectedFieldCropType == "Wheat") _fieldGenerator.SetCropType(SurfaceGridCellID.Surface_Type_Wheat);
            if (_selectedFieldCropType == "Oil Seed Rape") _fieldGenerator.SetCropType(SurfaceGridCellID.Surface_Type_OilSeedRape);
            if (_selectedFieldCropType == "Beans") _fieldGenerator.SetCropType(SurfaceGridCellID.Surface_Type_Beans);
            if (_selectedFieldCropType == "SugarBeat") _fieldGenerator.SetCropType(SurfaceGridCellID.Surface_Type_SugarBeat);

            SetButton(UIButtonID.Button_Generate, false);
            DisableAllUI();
            //SetFieldOutputButtons(true);
            _logoObj.SetLogo(LogoStateID.Logo_White_Text);
            FPSDisplay(true);

            //Allow 10 seconds before enabling access to the tool box windows again.
            _logoRawImgUI.gameObject.SetActive(true);
            _logoObj.gameObject.SetActive(true);
            StartTimer(UITimerID.Timer_Init_Logo_Pos_Change);

            for (int i = 0; i < _fieldCount; i++)
            {
                _fieldGenerator.AddFieldGrid(i, _fieldGridTooAdd[i]);
            }

            _fieldGenerator.InitFieldGeneration();

            
        }

        public void ResetButtonPress()
		{
            //SetWindow(UIWindowID.Window_Control, true);
            //SetWindow(UIWindowID.Window_CurrentOutput, false);
            AudioManager.PlaySFX(AudioID.Audio_But_Click01_SFX);

            DisableAllUI();
            SetWindow(UIWindowID.Window_Start, true);

            //_fieldGenerator.ClearUnUsedGridObjs();
            _fieldGenerator.Unload();
        }

        private void Update()
        {
            CharacterDialogueUIUpdate();
            TimerUpdate();
            InputUpdate();
        }
        private void InputUpdate()
        {
            //if (!_UIWindowObjs[(int)UIWindowID.Window_Toolbox_Enable].activeSelf) return;
            if (CharacterManager.GetCharacterDialogueID() != CharacterDialogueID.Character_Dialogue_Exit) return;
            if (EventManager.GetEventFlag() == EventFlagID.EventFlag_BadgeReward_Process) return;

            if (_cursorController.GetMouseMiddleClick())
            {
                if (!_UIWindowObjs[(int)UIWindowID.Window_Toolbox_Selection].activeSelf)
                {
                    ToolboxEnableButtonPress();
                    return;
                }
                //ToolboxEnableButtonPress();
                //SetMouseCursor(false, MouseClickStateID.OnClick_Nothing);//removes need for cursor tool button press in toolbox (shortcut really)

                //Ensures cursor type isn't jsut visible but it's action has been disabled.
                AudioManager.PlaySFX(AudioID.Audio_But_Click01_SFX);
                SetWindow(UIWindowID.Window_Toolbox_Selection, false);
                SetButton(UIButtonID.Button_Toolbox_Enable, true, "OFF");
                _cameraManager.EnableToolboxVisuals(false);
                EventManager.SetMouseClickStateID(MouseClickStateID.OnClick_Nothing);
                SetMouseCursor(false, MouseClickStateID.OnClick_Nothing);
                //ToolboxEnableButtonPress();
            }
            if (_cursorController.GetMouseFirstClick())
            {
                if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Nothing) return;
                if (EventManager.GetMouseClickStateID() == MouseClickStateID.OnClick_Tool_Selection) return;
                //if (_cameraManager.GetCurrentCameraBehaviour() == CameraBehaviour.TravelFromAPointToBPoint) return;

                //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                //if (Physics.Raycast(ray, out RaycastHit hitInfo))
                //{
                //    // We expect this hits the ground plane or base plane
                //    ProcessGridCellClick(hitInfo.point);
                //}
                //The below removes need for any collider at all! perhaps we can disable physics to buy performance.

                _cachedCellWidth = _fieldGenerator.GetCellWidth();
                _originGridCellPos = _fieldGenerator.GetGridStartPosition() - new Vector3(_cachedCellWidth / 2f, 0f, _cachedCellWidth / 2f);


                _cachedColliderReplacement = new Plane(Vector3.up, _originGridCellPos); // flat on XZ
                _cachedRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (_cachedColliderReplacement.Raycast(_cachedRay, out float enter))
                {
                    Vector3 hitPoint = _cachedRay.GetPoint(enter);
                    ProcessGridCellClick(hitPoint);
                }
            }
            if (_cursorController.GetESCKeyPress())
			{
                _fieldGenerator.Unload();

            }
        
        }


        private void ProcessGridCellClick(Vector3 worldPoint)
        {
            //...ect
        }

        private void CharacterDialogueUIUpdate()
        {
            if (CharacterManager.GetCharacterDialogueID() == CharacterDialogueID.Character_Dialogue_Exit) return;

            if (EventManager.GetEventFlag() == EventFlagID.EventFlag_Initial_Character_Camera_Process)
            {
                if (EventManager.GetEventFlag() != EventFlagID.EventFlag_Initial_Character_Dialogue_Init)
                {
                    SetWindow(UIWindowID.Window_Dialogue_Panel, true);
                    EventManager.SetEventFlag(EventFlagID.EventFlag_Initial_Character_Dialogue_Init);//, true);


                }

            }
            //...ect

        }
        private void TimerUpdate()
		{
            if (_timers[(int)UITimerID.Timer_Init_Logo_Pos_Change].HasTimerFinished(true))
            {
                _logoObj.SetLogoPos(LogoScreenPosID.Logo_Pos_BotRight);
                _logoObj.SetLogo(LogoStateID.Logo_Black_Text);
                _timers[(int)UITimerID.Timer_Field_Select_But_Update].StartTimer(0.25f);
            }

            if (_timers[(int)UITimerID.Timer_Field_Select_But_Update].HasTimerFinished(true))
            {
                SetFieldOutputButtons(true);
                _timers[(int)UITimerID.Timer_Field_Select_But_Update].Reset();
                _timers[(int)UITimerID.Timer_Field_Select_But_Update].StartTimer(0.25f);

            }

            if (_timers[(int)UITimerID.Timer_Startup_Logo_Only_Display].HasTimerFinished(true))
            {
                SetWindow(UIWindowID.Window_Start, true);
            }

            if (_timers[(int)UITimerID.Timer_RootNode_Win_Display].HasTimerFinished(true))
            {
                SetWindow(UIWindowID.Window_RootNode_Activation, false);
                //ResetAttrributeText();
            }

            if (_timers[(int)UITimerID.Timer_Attrribute_Text_Display].HasTimerFinished(true))
            {
             
                ResetAttrributeText();
            }

            if (_timers[(int)UITimerID.Timer_Initial_Character_Welcome].HasTimerFinished(true))
            {
                _cameraManager.EnableTravelToCharacterPoint(
                    CharacterManager.GetCurrentCharacterSpawnPos(), _fieldGridTooAdd[_curSelectedFieldId-1]);

            }
        }
        private void MouseCursorUpdate()
        {
            //...ect
        }




        public bool IsSelectedForPerformance()
        {
            //...ect
        }
        public void DropdownOptimization(int arg)
        {
            //...ect
        }
        public void DropdownFieldCountValueChanged(int arg)
        {
            
            int selectedIndex = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Field_Count].value;
            _selectedFieldCount = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Field_Count].options[selectedIndex].text;
        }
        public void DropdownFieldSizeValueChanged(int dropDownId)
        {
            //int selectedIndex = _dropdownFieldGridSize[dropDownId].value;
            //_selectedFieldGridSize = _dropdownFieldGridSize[dropDownId].options[selectedIndex].text;

            int selectedIndex = 0;

            switch (dropDownId)
            {
                case 1:
                    selectedIndex = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Field_Size_1].value;
                    _selectedFieldGridSize = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Field_Size_1].options[selectedIndex].text;
                    SetButton(UIButtonID.Button_Confirm_Field_1, true, null);
                    break;
                case 2:
                    selectedIndex = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Field_Size_2].value;
                    _selectedFieldGridSize = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Field_Size_2].options[selectedIndex].text;
                    SetButton(UIButtonID.Button_Confirm_Field_2, true, null);
                    break;
                case 3:
                    selectedIndex = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Field_Size_3].value;
                    _selectedFieldGridSize = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Field_Size_3].options[selectedIndex].text;
                    SetButton(UIButtonID.Button_Confirm_Field_3, true, null);
                    break;
                case 4:
                    selectedIndex = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Field_Size_4].value;
                    _selectedFieldGridSize = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Field_Size_4].options[selectedIndex].text;
                    SetButton(UIButtonID.Button_Confirm_Field_4, true, null);
                    break;
            }

            if (_selectedFieldGridSize == "")
            {
                AddFieldGridSize(dropDownId - 1, 5, 5);
                return;
            }
            if (_selectedFieldGridSize == "Extra Small")
            {
                AddFieldGridSize(dropDownId - 1, 5, 5);
                return;
            }
            if (_selectedFieldGridSize == "Small")
            {
                AddFieldGridSize(dropDownId - 1, 10, 10);
                return;
            }
            if (_selectedFieldGridSize == "Medium")
            {
                AddFieldGridSize(dropDownId - 1, 15, 15);
                return;
            }
            if (_selectedFieldGridSize == "Large")
            {
                AddFieldGridSize(dropDownId - 1, 20, 20);
                return;
            }

        }
        public void DropdownFieldSoilTypeValueChanged()//int dropDownId)
        {
            //int selectedIndex = _dropdownSoilTypes[dropDownId].value;
            //_selectedFieldSoilType = _dropdownSoilTypes[dropDownId].options[selectedIndex].text;

            

            int selectedIndex = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Soil_Selection].value;
            _selectedFieldSoilType = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Soil_Selection].options[selectedIndex].text;



            for (int i = 0; i < _fieldCount; i++)
            {
                if (_selectedFieldSoilType == "Peat Soil") AddFieldGridSoil(i, FieldGridCellID.FieldGridCell_Soil_Peat);
                if (_selectedFieldSoilType == "Light Soil") AddFieldGridSoil(i, FieldGridCellID.FieldGridCell_Soil_Light);
                if (_selectedFieldSoilType == "Heavy Soil") AddFieldGridSoil(i, FieldGridCellID.FieldGridCell_Soil_Heavy);
            }

            //AddFieldGridSoil

            SetButton(UIButtonID.Button_Confirm_Field_Count, true, null);
        }
        public void DropdownFieldCropTypeValueChanged(int dropDownId)
        {
            //int selectedIndex = _dropdownCropTypes[dropDownId].value;
            //_selectedFieldCropType = _dropdownCropTypes[dropDownId].options[selectedIndex].text;

            int selectedIndex = 0;

            switch (dropDownId)
            {
                case 1:
                    selectedIndex = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Crop_1].value;
                    _selectedFieldCropType = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Crop_1].options[selectedIndex].text;
                    SetButton(UIButtonID.Button_Confirm_Crops_1, true, null);
                    break;
                case 2:
                    selectedIndex = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Crop_2].value;
                    _selectedFieldCropType = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Crop_2].options[selectedIndex].text;
                    SetButton(UIButtonID.Button_Confirm_Crops_2, true, null);
                    break;
                case 3:
                    selectedIndex = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Crop_3].value;
                    _selectedFieldCropType = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Crop_3].options[selectedIndex].text;
                    SetButton(UIButtonID.Button_Confirm_Crops_3, true, null);
                    break;
                case 4:
                    selectedIndex = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Crop_4].value;
                    _selectedFieldCropType = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Crop_4].options[selectedIndex].text;
                    SetButton(UIButtonID.Button_Confirm_Crops_4, true, null);
                    break;
            }

            if (_selectedFieldCropType == "None (Saps)")
            {
                AddFieldGridCrop(dropDownId - 1, SurfaceGridCellID.Surface_Type_None);
                return;
            }
            if (_selectedFieldCropType == "Wheat")
            {
                AddFieldGridCrop(dropDownId - 1, SurfaceGridCellID.Surface_Type_Wheat);
                return;
            }
            if (_selectedFieldCropType == "Oil Seed Rape")
            {
                AddFieldGridCrop(dropDownId - 1, SurfaceGridCellID.Surface_Type_OilSeedRape);
                return;
            }
            if (_selectedFieldCropType == "Beans")
            {
                AddFieldGridCrop(dropDownId - 1, SurfaceGridCellID.Surface_Type_Beans);
                return;
            }
            if (_selectedFieldCropType == "SugarBeat")
            {
                AddFieldGridCrop(dropDownId - 1, SurfaceGridCellID.Surface_Type_SugarBeat);
                return;
            }
        }
        public void DropdownFieldTillageTypeValueChanged(int dropDownId)
        {
            //int selectedIndex = _dropdownCropTypes[dropDownId].value;
            //_selectedFieldCropType = _dropdownCropTypes[dropDownId].options[selectedIndex].text;

            int selectedIndex = 0;

            switch (dropDownId)
            {
                case 1:
                    selectedIndex = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Tillage_1].value;
                    _selectedFieldTillageType = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Tillage_1].options[selectedIndex].text;
                    SetButton(UIButtonID.Button_Confirm_Tillage_1, true, null);
                    break;
                case 2:
                    selectedIndex = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Tillage_2].value;
                    _selectedFieldTillageType = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Tillage_2].options[selectedIndex].text;
                    SetButton(UIButtonID.Button_Confirm_Tillage_2, true, null);
                    break;
                case 3:
                    selectedIndex = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Tillage_3].value;
                    _selectedFieldTillageType = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Tillage_3].options[selectedIndex].text;
                    SetButton(UIButtonID.Button_Confirm_Tillage_3, true, null);
                    break;
                case 4:
                    selectedIndex = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Tillage_4].value;
                    _selectedFieldTillageType = _UIDropdownsCached[(int)UIDropdownID.Dropdown_Tillage_4].options[selectedIndex].text;
                    SetButton(UIButtonID.Button_Confirm_Tillage_4, true, null);
                    break;
            }

            if (_selectedFieldCropType == "No-Till")
            {
                AddFieldGridTillage(dropDownId - 1, TillageGridCellID.Tillage_Type_NoTill);
                return;
            }
            if (_selectedFieldCropType == "Standard Tillage")
            {
                AddFieldGridTillage(dropDownId - 1, TillageGridCellID.Tillage_Type_Standard);
                return;
            }
        }


        private void InitCursors()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            _cursorController = _cursorObj.GetComponent<CursorController>();
            _cursorObj.SetActive(false);

        }
        private void InitDropdowns()
		{
            int dropdownCount = _UIDropdownObjs.Length;
            for (int i = 0; i < dropdownCount; i++)
            {
                _UIDropdownObjs[i].SetActive(true);
                _UIDropdownsCached[i] = _UIDropdownObjs[i].GetComponent<TMP_Dropdown>();
            }


            _fieldCountOptions = new List<string>();
            _fieldCountOptions.Add("1");
            _fieldCountOptions.Add("2");
            _fieldCountOptions.Add("3");
            _fieldCountOptions.Add("4");
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Field_Count].ClearOptions();
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Field_Count].AddOptions(_fieldCountOptions);
            //_UIDropdownsCached[(int)UIDropdownID.Dropdown_Field_Count].onValueChanged.AddListener(DropdownFieldCountValueChanged);


            _fieldGridSizeOptions = new List<string>();
            _fieldGridSizeOptions.Add("Extra Small");
            _fieldGridSizeOptions.Add("Small");
            _fieldGridSizeOptions.Add("Medium");
            _fieldGridSizeOptions.Add("Large");
            //_fieldGridSizeOptions.Add("Extra Large"); 
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Field_Size_1].ClearOptions();
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Field_Size_2].ClearOptions();
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Field_Size_3].ClearOptions();
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Field_Size_4].ClearOptions();
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Field_Size_1].AddOptions(_fieldGridSizeOptions);
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Field_Size_2].AddOptions(_fieldGridSizeOptions);
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Field_Size_3].AddOptions(_fieldGridSizeOptions);
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Field_Size_4].AddOptions(_fieldGridSizeOptions);


            _optimizeOptions = new List<string>();
            _optimizeOptions.Add("Quality");
            _optimizeOptions.Add("Performance");
            _UIDropdownsCached[(int)UIDropdownID.Dropdowm_Optimization].ClearOptions();
            _UIDropdownsCached[(int)UIDropdownID.Dropdowm_Optimization].AddOptions(_optimizeOptions);
            _selectedOptimizsationOption = "Quality";


            _soilTypeOptions = new List<string>();
            _soilTypeOptions.Add("Peat Soil");
            _soilTypeOptions.Add("Light Soil");
            _soilTypeOptions.Add("Heavy Soil");
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Soil_Selection].ClearOptions();
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Soil_Selection].AddOptions(_soilTypeOptions);




            _cropTypeOptions = new List<string>();
            _cropTypeOptions.Add("None (Saps)");
            _cropTypeOptions.Add("Wheat");
            _cropTypeOptions.Add("Oil Seed Rape");
            _cropTypeOptions.Add("Beans");
            _cropTypeOptions.Add("SugarBeat");
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Crop_1].ClearOptions();
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Crop_2].ClearOptions();
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Crop_3].ClearOptions();
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Crop_4].ClearOptions();
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Crop_1].AddOptions(_cropTypeOptions);
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Crop_2].AddOptions(_cropTypeOptions);
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Crop_3].AddOptions(_cropTypeOptions);
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Crop_4].AddOptions(_cropTypeOptions);


            _tillageTypeOptions = new List<string>();
            _tillageTypeOptions.Add("Standard Tillage");
            _tillageTypeOptions.Add("No-Till");
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Tillage_1].ClearOptions();
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Tillage_2].ClearOptions();
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Tillage_3].ClearOptions();
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Tillage_4].ClearOptions();
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Tillage_1].AddOptions(_tillageTypeOptions);
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Tillage_2].AddOptions(_tillageTypeOptions);
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Tillage_3].AddOptions(_tillageTypeOptions);
            _UIDropdownsCached[(int)UIDropdownID.Dropdown_Tillage_4].AddOptions(_tillageTypeOptions);



        }

        private void InitButtons()
        {
            if (_UIButtonObjs == null) return;

            int buttonCount = _UIButtonObjs.Length;

            for (int i = 0; i < buttonCount; i++)
            {
                _UIButtonsCached[i] = _UIButtonObjs[i].GetComponent<Button>();
            }

            _fieldToWorkUponDecided = false;
            _fieldToWorkUponSelectedId = -1;
        }
        private void InitTimers()
		{
            for (int i = 0; i < _timerCount; i++)
			{
                _timerObjs[i] = new GameObject();
                _timerObjs[i].AddComponent<WTimer>();
                _timers[i] = _timerObjs[i].GetComponent<WTimer>();
            }
            _waitTimes[(int)UITimerID.Timer_Init_Logo_Pos_Change] = 6.0f;//6 seconds
            _waitTimes[(int)UITimerID.Timer_Field_Select_But_Update] = 0.025f;// 0.25f; // 1/4 a second update.
            _waitTimes[(int)UITimerID.Timer_Startup_Logo_Only_Display] = 6.0f;//SetWindow(UIWindowID.Window_Start, true);
            _waitTimes[(int)UITimerID.Timer_RootNode_Win_Display] = 4.0f;
            _waitTimes[(int)UITimerID.Timer_Initial_Character_Welcome] = 4.0f;
            _waitTimes[(int)UITimerID.Timer_Attrribute_Text_Display] = 2.0f;

            _timers[(int)UITimerID.Timer_Startup_Logo_Only_Display].StartTimer(3.0f);//SetWindow(UIWindowID.Window_Start, true);


        }
        private void StartTimer(UITimerID uITimerID)
		{
            _timers[(int)uITimerID].Reset();
            _timers[(int)uITimerID].StartTimer(_waitTimes[(int)uITimerID]);
        }
        private void ProcessTimerProcess(UITimerID uITimerID)
		{
            switch(uITimerID)
			{
                case UITimerID.Timer_Init_Logo_Pos_Change:
                    if (_timers[(int)uITimerID].HasTimerFinished(true))
                    {
                        //SetButton(UIButtonID.Button_Toolbox, true);
                        SetFieldOutputButtons(true);
                        //_timers[(int)uITimerID].Reset();
                        //_timers[(int)uITimerID].StartTimer(0.25f);
                        _logoObj.SetLogoPos(LogoScreenPosID.Logo_Pos_BotRight);
                        _logoObj.SetLogo(LogoStateID.Logo_Black_Text);
                    }
                    break;
			}

        }
        
        
        private void SetWindow(UIWindowID UIWindowID, bool enable)
        {
            if (_UIWindowObjs != null)
                _UIWindowObjs[(int)UIWindowID].SetActive(enable);
        }
        private void SetDropdown(UIDropdownID UIDropdownID, bool enable)
        {
            if (_UIDropdownObjs != null)
                _UIDropdownObjs[(int)UIDropdownID].SetActive(enable);
        }
        private void SetDropdownState(UIDropdownID UIDropdownID, bool interactable)
        {
            if (_UIDropdownObjs == null) return;

            _UIDropdownsCached[(int)UIDropdownID].interactable = interactable;

            //if (UIDropdownID == UIDropdownID.Dropdown_Field_Count) _UIDropdownsCached[(int)UIDropdownID.Dropdown_Field_Count].interactable = interactable;

            //if (UIDropdownID == UIDropdownID.Dropdown_Field_Size_1) _UIDropdownsCached[(int)UIDropdownID.Dropdown_Field_Size_1].interactable = interactable;
            //if (UIDropdownID == UIDropdownID.Dropdown_Field_Size_2) _dropdownFieldGridSize[1].interactable = interactable;
            //if (UIDropdownID == UIDropdownID.Dropdown_Field_Size_3) _dropdownFieldGridSize[2].interactable = interactable;
            //if (UIDropdownID == UIDropdownID.Dropdown_Field_Size_4) _dropdownFieldGridSize[3].interactable = interactable;

            //if (UIDropdownID == UIDropdownID.Dropdown_Soil_1) _dropdownSoilTypes[0].interactable = interactable;
            //if (UIDropdownID == UIDropdownID.Dropdown_Soil_2) _dropdownSoilTypes[1].interactable = interactable;
            //if (UIDropdownID == UIDropdownID.Dropdown_Soil_3) _dropdownSoilTypes[2].interactable = interactable;
            //if (UIDropdownID == UIDropdownID.Dropdown_Soil_4) _dropdownSoilTypes[3].interactable = interactable;

            //if (UIDropdownID == UIDropdownID.Dropdown_Crop_1) _dropdownCropTypes[0].interactable = interactable;
            //if (UIDropdownID == UIDropdownID.Dropdown_Crop_2) _dropdownCropTypes[1].interactable = interactable;
            //if (UIDropdownID == UIDropdownID.Dropdown_Crop_3) _dropdownCropTypes[2].interactable = interactable;
            //if (UIDropdownID == UIDropdownID.Dropdown_Crop_4) _dropdownCropTypes[3].interactable = interactable;

            //if (UIDropdownID == UIDropdownID.Dropdown_Tillage_1) _dropdownTillageTypes[0].interactable = interactable;
            //if (UIDropdownID == UIDropdownID.Dropdown_Tillage_2) _dropdownTillageTypes[1].interactable = interactable;
            //if (UIDropdownID == UIDropdownID.Dropdown_Tillage_3) _dropdownTillageTypes[2].interactable = interactable;
            //if (UIDropdownID == UIDropdownID.Dropdown_Tillage_4) _dropdownTillageTypes[3].interactable = interactable;


            //_UIDropdownsCached[(int)UIDropdownID].interactable = interactable;
            //_UIDropdownObjs[(int)UIDropdownID].GetComponent<Dropdown>().interactable = interactable;
        }
        private void SetButton(UIButtonID UIButtonID, bool interactable, string buttonText)
        {
            if (_UIButtonObjs == null) return;
            if (_UIButtonsCached == null) return;

            _UIButtonsCached[(int)UIButtonID].interactable = interactable;

            if (buttonText == null) return;
            if (buttonText == "") return;

            //_UIButtonsCached[(int)UIButtonID].GetComponentInChildren<Text>().text = buttonText;
            _UIButtonsCached[(int)UIButtonID].GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
        }
        private void SetButton(UIButtonID UIButtonID, bool enable)
        {
            if (_UIButtonObjs == null) return;

            _UIButtonObjs[(int)UIButtonID].SetActive(enable);
        }
        private void SetButtonTextColor(UIButtonID UIButtonID, Color color)
        {
            _UIButtonsCached[(int)UIButtonID].GetComponentInChildren<TextMeshProUGUI>().color = color;
        }
        private void SetAttrributeText(UIAttrributeTextID attrributeID)
        {
            int fieldId = _curSelectedFieldId;
            double value = 0.0;
            string str = "";

            if (attrributeID == UIAttrributeTextID.Text_TotalSustainability)
            {
                value = RulescopeManager._instance.GetRulescope().GetTotalSustainabilityScore();
                str = value.ToString("F2") + "%";
            }
            if (attrributeID == UIAttrributeTextID.Text_Yield)
            {
                value = RulescopeManager._instance.GetRulescope().GetYieldScore(fieldId-1);
                str = value.ToString("F2") + "%";
            }


            _attrributeText[(int)attrributeID].text = str;// "";

            _timers[(int)UITimerID.Timer_Attrribute_Text_Display].StartTimer(_waitTimes[(int)UITimerID.Timer_Attrribute_Text_Display]);
        }
        private void ResetAttrributeText()
        {
            _attrributeText[0].text = _cachedAttrributeStrA;// "Biodiversity";
            _attrributeText[1].text = _cachedAttrributeStrB;//"Yield";
            _attrributeText[2].text = _cachedAttrributeStrC;//"Soil Health";
            _attrributeText[3].text = _cachedAttrributeStrD;//"Fuel/Fibre Yield";
            _attrributeText[4].text = _cachedAttrributeStrE;//"Carbon Sequestration";
            _attrributeText[5].text = _cachedAttrributeStrF;//"Asethetic/Recreation";
            _attrributeText[6].text = _cachedAttrributeStrG;//"Sustainability";
        }
        private void SetRewardUIWindow(bool enable, int hedgerowCorrectCount)
        {
            if (_rewardControllerObj != null)
            {
                SetWindow(UIWindowID.Window_Badge_Reward, enable);
                //_rewardControllerCached.UpdateHedgrowRewardCount();
                _rewardControllerObj.SetActive(enable);//_rewardControllerCached;
                _rewardControllerCached.UpdateHedgrowRewardCount();

            }
        }
        private void SetFieldOutputButtons(bool enable)
        {
            if (_fieldToWorkUponDecided) return;


            //Vector3 screenPos = Camera.main.WorldToScreenPoint(_UIButtonsCached[(int)UIButtonID.Button_Field_Output_1].transform.position);
            //Vector3 uiPos = new Vector3(screenPos.x, Screen.height - screenPos.y, screenPos.z);

            Vector3 screenPos = Vector3.zero;//Camera.main..WorldToScreenPoint(_fieldGenerator.GetGridCenterPosition());

            //if (_usingDefaultSetup)
            //{
            //    SetButton(UIButtonID.Button_Field_Output_1, enable);
            //    screenPos = Camera.main.WorldToScreenPoint(_fieldGenerator.GetGridCenterPosition(0));
            //    _fieldOutputButRects[0].position = screenPos;

            //    SetButton(UIButtonID.Button_Field_Output_1, true, "View Status");
            //    //SetButton(UIButtonID.Button_Field_Output_1, true, "Start Building");
            //}

            if (_fieldCount == 1)
            {
                SetButton(UIButtonID.Button_Field_Output_1, enable);
                screenPos = Camera.main.WorldToScreenPoint(_fieldGenerator.GetGridCenterPosition(0));

                //Vector2 positionOnScreen;
                //RectTransformUtility.ScreenPointToLocalPointInRectangle(_fieldOutputButRects[0], screenPos, Camera.main, out positionOnScreen);

                _fieldOutputButRects[0].position = screenPos;

                //_UIButtonsCached[(int)UIButtonID.Button_Field_Output_1].transform.position = uiPos;
            }
            if (_fieldCount == 2)
            {
                SetButton(UIButtonID.Button_Field_Output_1, enable);
                SetButton(UIButtonID.Button_Field_Output_2, enable);

                screenPos = Camera.main.WorldToScreenPoint(_fieldGenerator.GetGridCenterPosition(0));
                _fieldOutputButRects[0].position = screenPos;
                screenPos = Camera.main.WorldToScreenPoint(_fieldGenerator.GetGridCenterPosition(1));
                _fieldOutputButRects[1].position = screenPos;
            }
            if (_fieldCount == 3)
            {
                SetButton(UIButtonID.Button_Field_Output_1, enable);
                SetButton(UIButtonID.Button_Field_Output_2, enable);
                SetButton(UIButtonID.Button_Field_Output_3, enable);
                screenPos = Camera.main.WorldToScreenPoint(_fieldGenerator.GetGridCenterPosition(0));
                _fieldOutputButRects[0].position = screenPos;
                screenPos = Camera.main.WorldToScreenPoint(_fieldGenerator.GetGridCenterPosition(1));
                _fieldOutputButRects[1].position = screenPos;
                screenPos = Camera.main.WorldToScreenPoint(_fieldGenerator.GetGridCenterPosition(2));
                _fieldOutputButRects[2].position = screenPos;
            }
            if (_fieldCount == 4)
            {
                SetButton(UIButtonID.Button_Field_Output_1, enable);
                SetButton(UIButtonID.Button_Field_Output_2, enable);
                SetButton(UIButtonID.Button_Field_Output_3, enable);
                SetButton(UIButtonID.Button_Field_Output_4, enable);
                screenPos = Camera.main.WorldToScreenPoint(_fieldGenerator.GetGridCenterPosition(0));
                _fieldOutputButRects[0].position = screenPos;
                screenPos = Camera.main.WorldToScreenPoint(_fieldGenerator.GetGridCenterPosition(1));
                _fieldOutputButRects[1].position = screenPos;
                screenPos = Camera.main.WorldToScreenPoint(_fieldGenerator.GetGridCenterPosition(2));
                _fieldOutputButRects[2].position = screenPos;
                screenPos = Camera.main.WorldToScreenPoint(_fieldGenerator.GetGridCenterPosition(3));
                _fieldOutputButRects[3].position = screenPos;
            }
        }
        private void FPSDisplay(bool enable)
		{
            if (_fpsObj != null)
            {
                _fpsObj.SetActive(enable);
            }
        }
        private void DisableAllUI()
        {
            if (_UIWindowObjs != null)
            {
                for (int i = 0; i < _UIWindowObjs.Length; i++)
                    _UIWindowObjs[i].SetActive(false);
            }
            if (_UIDropdownObjs != null)
            {
                //for (int i = 0; i < _UIDropdownObjs.Length; i++)
                //    _UIDropdownObjs[i].SetActive(false);
            }
            if (_UIButtonsCached != null)
            {
                for (int i = 0; i < _UIButtonsCached.Length; i++)
                    _UIButtonsCached[i].interactable = true;
            }
            if (_fpsObj != null)
            {
                _fpsObj.SetActive(false);
            }




        }

    }
}


