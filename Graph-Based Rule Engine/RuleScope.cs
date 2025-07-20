using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DoomBreakers;
using wildlogicgames;
using System.Text;

namespace IsometricFarmGenerator
{
    public struct HedgerowCorrectCell
    {
        public CellBorderType CELL_BORDER_TYPE;
        public int X;
        public int Y;
    };
    public struct FieldEntireHedgePlacement
    {
        public int FIELD_ID;
        public bool LEFT_COMPLETE;
        public bool RIGHT_COMPLETE;
        public bool TOP_COMPLETE;
        public bool BOTTOM_COMPLETE;
        public bool ALL_COMPLETE;

        public bool LEFT_AWARDED;
        public bool RIGHT_AWARDED;
        public bool TOP_AWARDED;
        public bool BOTTOM_AWARDED;
        public bool ALL_AWARDED;
    };

    public class RuleScope : MonoBehaviour
    {
        [Header("Field Generator Ref")]
        public FieldGenerator _fieldGenerator;
        private CellObj[,] _baseCellObjsCached = new CellObj[50, 50];
        private int[] _fieldExpenditure = new int[4];
        private int _selectedField;
        private const int _maxGridWidth = 50;
        private const int _maxGridHeight = 50;
        private int _mostRecentX, _mostRecentY;







        private List<HedgerowCorrectCell> _field1_HedgerowValidBorderGridCellPos;
        private List<HedgerowCorrectCell> _field2_HedgerowValidBorderGridCellPos;
        private List<HedgerowCorrectCell> _field3_HedgerowValidBorderGridCellPos;
        private List<HedgerowCorrectCell> _field4_HedgerowValidBorderGridCellPos;
        private FieldEntireHedgePlacement[] _fullHedgePlacement = new FieldEntireHedgePlacement[4];

        private Vector2Int _correctCellTopStart_field1 = new Vector2Int(1, 1);
        private Vector2Int _correctCellTopEnd_field1 = new Vector2Int(24, 1);
        private Vector2Int _correctCellBotStart_field1 = new Vector2Int(1, 24);
        private Vector2Int _correctCellBotEnd_field1 = new Vector2Int(24, 24);
        private Vector2Int _correctCellLeftStart_field1 = new Vector2Int(1, 2);
        private Vector2Int _correctCellLeftEnd_field1 = new Vector2Int(1, 23);
        private Vector2Int _correctCellRightStart_field1 = new Vector2Int(24, 2);
        private Vector2Int _correctCellRightEnd_field1 = new Vector2Int(24, 23);

        private Vector2Int _correctCellTopStart_field2 = new Vector2Int(24, 1);
        private Vector2Int _correctCellTopEnd_field2 = new Vector2Int(47, 1);
        private Vector2Int _correctCellBotStart_field2 = new Vector2Int(24, 24);
        private Vector2Int _correctCellBotEnd_field2 = new Vector2Int(47, 24);
        private Vector2Int _correctCellLeftStart_field2 = new Vector2Int(24, 2);
        private Vector2Int _correctCellLeftEnd_field2 = new Vector2Int(24, 23);
        private Vector2Int _correctCellRightStart_field2 = new Vector2Int(47, 2);
        private Vector2Int _correctCellRightEnd_field2 = new Vector2Int(47, 23);

        private Vector2Int _correctCellTopStart_field3 = new Vector2Int(1, 24);
        private Vector2Int _correctCellTopEnd_field3 = new Vector2Int(24, 24);
        private Vector2Int _correctCellBotStart_field3 = new Vector2Int(1, 47);
        private Vector2Int _correctCellBotEnd_field3 = new Vector2Int(24, 47);
        private Vector2Int _correctCellLeftStart_field3 = new Vector2Int(1, 25);
        private Vector2Int _correctCellLeftEnd_field3 = new Vector2Int(1, 46);
        private Vector2Int _correctCellRightStart_field3 = new Vector2Int(24, 25);
        private Vector2Int _correctCellRightEnd_field3 = new Vector2Int(24, 46);

        private Vector2Int _correctCellTopStart_field4 = new Vector2Int(24, 24);
        private Vector2Int _correctCellTopEnd_field4 = new Vector2Int(47, 24);
        private Vector2Int _correctCellBotStart_field4 = new Vector2Int(24, 47);
        private Vector2Int _correctCellBotEnd_field4 = new Vector2Int(47, 47);
        private Vector2Int _correctCellLeftStart_field4 = new Vector2Int(24, 25);
        private Vector2Int _correctCellLeftEnd_field4 = new Vector2Int(24, 46);
        private Vector2Int _correctCellRightStart_field4 = new Vector2Int(47, 25);
        private Vector2Int _correctCellRightEnd_field4 = new Vector2Int(47, 46);




        [Header("Camera Manager Ref")]
        public CameraManager _cameraManager;

        [Header("Coins Splash FX")]
        public ParticleSystem _coinSplashFX;







        private Node _rootNode;

        private int _coinCount;
        private const int _hedgerowCoinCost = 4; //5
        private const int _fenceCoinCost = 2; //5

        //private int _totalSustainabilityScore;
        private Dictionary<Node, double> _scenarioScores = new Dictionary<Node, double>();
        private double[] _yeildScore = new double[4];//0.0;
        private double _totalSustainabilityScore = 0.0;
        private int _correctHedgeRowPlacementsMade = 0;




        private const string _Indicator_CropPestPathogen = "Crop, pest & pathogens";
        private const string _Indicator_SoilInvertebrateHabitat = "Soil invertebrate habitat";
        private const string _Indicator_SoilWaterCapacity = "Soil water capacity";
        private const string _Indicator_SoilNutientsOM = "Soil nutrients & OM";
        private const string _Indicator_SoilTextureAggregate = "Soil texture aggregate";
        private const string _Indicator_SoilCarbonStorage = "Soil carbon storage";
        private const string _Indicator_EdgeCentreRatio = "Edge to centre ratio";
        private const string _Indicator_HabitatDiversity = "Habitat diversity";
        private const string _Indicator_FarmWildlifeHabitat = "Farm wildlife habitat";
        private const string _Indicator_PredatorHabitat = "Predator habitat";
        private const string _Indicator_PollinatorHabitat = "Pollinator habitat";
        private const string _Indicator_VegetationCarbonStorage = "Vegetation carbon storage";

        private const string _Ecosystem_PestWeedDisease = "Pest, weed, disease control";
        private const string _Ecosystem_Pollination = "Pollination";
        private const string _Ecosystem_WaterRegulation = "Water regulation";

        private const string _Attribute_Yield = "Yield";
        private const string _Attribute_FuelFibreYield = "Fuel-fibre yield";
        private const string _Attribute_Biodiversity = "Biodiversity";
        private const string _Attribute_AestheticRecreation = "Aesthetic-recreation";
        private const string _Attribute_SoilHealth = "Soil Health";
        private const string _Attribute_CarbonSequestration = "Carbon sequestration";

        private const string _Sustainability_Score = "Sustainability score";

        private StringBuilder _stringBuilder;// = new StringBuilder();
        private string _outputStr, _scoringStr;

        private Action[] _actionListener = new Action[4];


        private void OnDisable()
        {
            EventManager.Unsubscribe("Report_Placement_OnBorder_Hedgerow", _actionListener[0]);
            EventManager.Unsubscribe("Report_OnClick_Hedgerow", _actionListener[1]);
            EventManager.Unsubscribe("Report_OnClick_Fence", _actionListener[2]);
            EventManager.Unsubscribe("Report_OnClick_Tree", _actionListener[3]);
        }
        private void Awake() => Setup();
        private void Setup()
        {
            _stringBuilder = new StringBuilder();
            _outputStr = "";

            _coinCount = 1000;// 1000;

            int defaultSetupFenceCost = 805;

            _coinCount += defaultSetupFenceCost;
            _totalSustainabilityScore = 0;
            for (int i = 0; i < _yeildScore.Length; i++) _yeildScore[i] = 100.0;

            //Setup ruleset nodes.
            InitHedgerow();

            if (_field1_HedgerowValidBorderGridCellPos == null) _field1_HedgerowValidBorderGridCellPos = new List<HedgerowCorrectCell>();
            if (_field2_HedgerowValidBorderGridCellPos == null) _field2_HedgerowValidBorderGridCellPos = new List<HedgerowCorrectCell>();
            if (_field3_HedgerowValidBorderGridCellPos == null) _field3_HedgerowValidBorderGridCellPos = new List<HedgerowCorrectCell>();
            if (_field4_HedgerowValidBorderGridCellPos == null) _field4_HedgerowValidBorderGridCellPos = new List<HedgerowCorrectCell>();


            _actionListener[0] = new Action(ProcessHedgerowBorder);//DelegateMethod()
            _actionListener[1] = new Action(ProcessHedgerow);//DelegateMethod()
            _actionListener[2] = new Action(ProcessFence);//DelegateMethod()
            _actionListener[3] = new Action(ProcessTree);//DelegateMethod()

            EventManager.Subscribe("Report_Placement_OnBorder_Hedgerow", _actionListener[0]);
            EventManager.Subscribe("Report_OnClick_Hedgerow", _actionListener[1]);
            EventManager.Subscribe("Report_OnClick_Fence", _actionListener[2]);
            EventManager.Subscribe("Report_OnClick_Tree", _actionListener[3]);
            //EventManager.TriggerEvent("Report_Placement_OnBorder_Hedgerow");
        }
        private void InitHedgerow()
        {
            for (int i = 0; i < 4; i++)
            {
                _fullHedgePlacement[i].FIELD_ID = i;
                _fullHedgePlacement[i].LEFT_COMPLETE = false;
                _fullHedgePlacement[i].RIGHT_COMPLETE = false;
                _fullHedgePlacement[i].TOP_COMPLETE = false;
                _fullHedgePlacement[i].BOTTOM_COMPLETE = false;
                _fullHedgePlacement[i].ALL_COMPLETE = false;

                _fullHedgePlacement[i].ALL_AWARDED = false;
                _fullHedgePlacement[i].LEFT_AWARDED = false;
                _fullHedgePlacement[i].RIGHT_AWARDED = false;
                _fullHedgePlacement[i].TOP_AWARDED = false;
                _fullHedgePlacement[i].BOTTOM_AWARDED = false;
            }



            //Define rules as actions.
            //var managementA = new Node(NodeID.ID_Management_Added_Hedgerow, "Add Hedgerow", () => print("\nAdd Hedgerow triggered"));
            _rootNode = new Node(NodeID.ID_Management_Added_Hedgerow, "Add Hedgerow", () => Nothing());


            var indicatorA = new Node(NodeID.ID_Indicator_Measure, _Indicator_CropPestPathogen, () => Nothing());
            var indicatorB = new Node(NodeID.ID_Indicator_Measure, _Indicator_SoilInvertebrateHabitat, () => Nothing());
            var indicatorC = new Node(NodeID.ID_Indicator_Measure, _Indicator_SoilWaterCapacity, () => Nothing());
            var indicatorD = new Node(NodeID.ID_Indicator_Measure, _Indicator_SoilNutientsOM, () => Nothing());
            var indicatorE = new Node(NodeID.ID_Indicator_Measure, _Indicator_SoilTextureAggregate, () => Nothing());
            var indicatorF = new Node(NodeID.ID_Indicator_Measure, _Indicator_SoilCarbonStorage, () => Nothing());
            var indicatorG = new Node(NodeID.ID_Indicator_Measure, _Indicator_EdgeCentreRatio, () => Nothing());
            var indicatorH = new Node(NodeID.ID_Indicator_Measure, _Indicator_HabitatDiversity, () => Nothing());
            var indicatorI = new Node(NodeID.ID_Indicator_Measure, _Indicator_FarmWildlifeHabitat, () => Nothing());
            var indicatorJ = new Node(NodeID.ID_Indicator_Measure, _Indicator_PredatorHabitat, () => Nothing());
            var indicatorK = new Node(NodeID.ID_Indicator_Measure, _Indicator_PollinatorHabitat, () => Nothing());
            var indicatorL = new Node(NodeID.ID_Indicator_Measure, _Indicator_VegetationCarbonStorage, () => Nothing());

            _scenarioScores[indicatorA] = -2;
            _scenarioScores[indicatorB] = 2;
            _scenarioScores[indicatorC] = 2.75;
            _scenarioScores[indicatorD] = 3.5;
            _scenarioScores[indicatorE] = 2;
            _scenarioScores[indicatorF] = 1.25;
            _scenarioScores[indicatorG] = 2;
            _scenarioScores[indicatorH] = 4.5;
            _scenarioScores[indicatorI] = 2.75;
            _scenarioScores[indicatorJ] = 3.5;
            _scenarioScores[indicatorK] = 2.75;
            _scenarioScores[indicatorL] = 4.75;


			//_scenarioScores.Clear();
			// ... add new values based on the scenario

			indicatorA.Score = _scenarioScores[indicatorA];
			indicatorB.Score = _scenarioScores[indicatorB];
            indicatorC.Score = _scenarioScores[indicatorC];
            indicatorD.Score = _scenarioScores[indicatorD];
            indicatorE.Score = _scenarioScores[indicatorE];
            indicatorF.Score = _scenarioScores[indicatorF];
            indicatorG.Score = _scenarioScores[indicatorG];
            indicatorH.Score = _scenarioScores[indicatorH];
            indicatorI.Score = _scenarioScores[indicatorI];
            indicatorJ.Score = _scenarioScores[indicatorJ];
            indicatorK.Score = _scenarioScores[indicatorK];
            indicatorL.Score = _scenarioScores[indicatorL];

            //indicatorA.Rule = () => ScoreRule(indicatorA);
            //indicatorB.Rule = () => ScoreRule(indicatorB);
            //indicatorC.Rule = () => ScoreRule(indicatorC);
            //indicatorD.Rule = () => ScoreRule(indicatorD);
            //indicatorE.Rule = () => ScoreRule(indicatorE);
            //indicatorF.Rule = () => ScoreRule(indicatorF);
            //indicatorG.Rule = () => ScoreRule(indicatorG);
            //indicatorH.Rule = () => ScoreRule(indicatorH);
            //indicatorI.Rule = () => ScoreRule(indicatorI);
            //indicatorJ.Rule = () => ScoreRule(indicatorJ);
            //indicatorK.Rule = () => ScoreRule(indicatorK);
            //indicatorL.Rule = () => ScoreRule(indicatorL);


            var ecosystemA = new Node(NodeID.ID_Supporting_Ecosystem, _Ecosystem_PestWeedDisease, () => Nothing());
            var ecosystemB = new Node(NodeID.ID_Supporting_Ecosystem, _Ecosystem_Pollination, () => Nothing());
            var ecosystemC = new Node(NodeID.ID_Supporting_Ecosystem, _Ecosystem_WaterRegulation, () => Nothing());

            var attributeA = new Node(NodeID.ID_Key_Attribute, _Attribute_Yield, () => Nothing());
            var attributeB = new Node(NodeID.ID_Key_Attribute, _Attribute_FuelFibreYield, () => Nothing());
            var attributeC = new Node(NodeID.ID_Key_Attribute, _Attribute_Biodiversity, () => Nothing());
            var attributeD = new Node(NodeID.ID_Key_Attribute, _Attribute_AestheticRecreation, () => Nothing());
            var attributeE = new Node(NodeID.ID_Key_Attribute, _Attribute_SoilHealth, () => Nothing());
            var attributeF = new Node(NodeID.ID_Key_Attribute, _Attribute_CarbonSequestration, () => Nothing());

            var sustainabilityScore = new Node(NodeID.ID_Sustainability_Score, _Sustainability_Score, () => _totalSustainabilityScore++);


            //Define their relationships.

            _rootNode.AddChild(indicatorA);
            //_rootNode.AddChild(indicatorB);
            _rootNode.AddChild(indicatorC);
            _rootNode.AddChild(indicatorD);
            _rootNode.AddChild(indicatorE);
            _rootNode.AddChild(indicatorG);
            _rootNode.AddChild(indicatorH);
            _rootNode.AddChild(indicatorI);
            _rootNode.AddChild(indicatorJ);
            _rootNode.AddChild(indicatorK);
            _rootNode.AddChild(indicatorL);


            //indicatorA.AddChild(ecosystemA);
            //indicatorB.AddChild(attributeE);
            ////indicatorB.AddChild(indicatorJ);
            indicatorC.AddChild(ecosystemC);
            indicatorC.AddChild(attributeE);
            indicatorD.AddChild(ecosystemA);
            indicatorD.AddChild(ecosystemC);
            indicatorE.AddChild(ecosystemC);
            indicatorE.AddChild(attributeE);
            //indicatorF.AddChild(attributeE);
            //indicatorF.AddChild(attributeF);
            indicatorG.AddChild(ecosystemA);
            indicatorG.AddChild(ecosystemB);
            ////indicatorG.AddChild(indicatorH);
            indicatorH.AddChild(attributeC);
            indicatorI.AddChild(attributeC);
            indicatorI.AddChild(attributeE);
            indicatorJ.AddChild(ecosystemA);
            indicatorJ.AddChild(ecosystemC);
            indicatorJ.AddChild(attributeE);
            indicatorK.AddChild(ecosystemB);
            ////indicatorK.AddChild(indicatorI);
            indicatorL.AddChild(attributeF);

            ecosystemA.AddChild(attributeA);
            ecosystemA.AddChild(attributeB);
            ecosystemA.AddChild(attributeC);
            ecosystemB.AddChild(attributeA);
            ecosystemB.AddChild(attributeB);
            ecosystemB.AddChild(attributeC);
            ecosystemC.AddChild(attributeA);
            ecosystemC.AddChild(attributeB);

            attributeA.AddChild(sustainabilityScore);
            attributeB.AddChild(sustainabilityScore);
            attributeC.AddChild(sustainabilityScore);
            attributeD.AddChild(sustainabilityScore);
            attributeE.AddChild(sustainabilityScore);
            attributeF.AddChild(sustainabilityScore);

            //_rootNode.Activate();
            //managementA.Activate();

        }
        private void Nothing()
        {
            return;
        }
        private void ScoreRule(Node node)
        {
            if (_scenarioScores.TryGetValue(node, out var rawScore))
            {
                double v = 750.0;// 750.0;
                double scaled = rawScore / v;// 100.0;// 75.0; // scale to 0–1
                //_totalSustainabilityScore += scaled;

                //_totalSustainabilityScore = Math.Min(_totalSustainabilityScore, 1.0); // or whatever your cap is
                //double moreScale = 4.0;
                _totalSustainabilityScore = (0.91 * _totalSustainabilityScore + 0.09 * scaled);// / moreScale;
                //_totalSustainabilityScore = 0.9 * _totalSustainabilityScore + 0.1 * scaled;
                //…is an example of exponential smoothing or a moving average, where:
                //0.9 is the "retention weight" (how much of the previous score you keep),
                //0.1 is the "learning rate"(how much the new value affects the total).
                _totalSustainabilityScore += scaled;

                //_totalSustainabilityScore = _totalSustainabilityScore / 100;

                _stringBuilder.Append($"\nActivated {node.Name}, raw = {rawScore}, scaled = {scaled:F2} ({scaled * v:F1}%)");
                //print($"\nActivated {node.Name}, raw = {rawScore}, scaled = {scaled:F2} ({scaled * 1000:F1}%)");
                //_scoringStr = "\nActivated { node.Name}, raw = { rawScore}, scaled = { scaled: F2} ({ scaled * 100:F1}%)";
            }
            else
            {
                print($"\nActivated {node.Name}, but no score found.");
            }
        }
        private void ProcessHedgerowBorder() //=> _rootNode.Activate();
        {




            //_scenarioScores.Clear();
            // ... add new values based on the scenario



            _rootNode.Activate();

            _stringBuilder.Clear();
            _outputStr = "";

            string lineA = "12 of 12 indicator/measures processed.\n";
            _stringBuilder.Append(lineA);

            //int i = 0;
			foreach (Node child in _rootNode.Children)
			{
                ScoreRule(child);
                //print("\n" + child.Name + " Indicator/Measure visited = " + child.ActivationCount);
                //string lineB = "\n" + child.Name + " Indicator/Measure visited! Score = " + child.Score;// + " |Visit Count = " + child.ActivationCount;
				//_stringBuilder.Append(lineB);


			}
            //_stringBuilder.Append(_scoringStr);
            //string lineD = "\nSustainability Score = " + "$({_totalSustainabilityScore * 1000:F1}%)";// +
            //"\n_totalSustainabilityScore = " + _totalSustainabilityScore.ToString();

            _stringBuilder.Append("\nTotal Sustainability Score = " + _totalSustainabilityScore.ToString());
            //_stringBuilder.Append(lineD);

            _outputStr = _stringBuilder.ToString();

            if (HasCompletedFullHedgeRowColPlaement())//_mostRecentX, _mostRecentY))
            {
                //if (AwardLeftHedgeCompletion()) return;
                //if (AwardRightHedgeCompletion()) return;
                //if (AwardTopHedgeCompletion()) return;
                //if (AwardBottomHedgeCompletion()) return;
                //if (AwardAllHedgeCompletion()) return;

                _correctHedgeRowPlacementsMade++;

                AwardLeftHedgeCompletion();
                AwardRightHedgeCompletion();
                AwardTopHedgeCompletion();
                AwardBottomHedgeCompletion();
                AwardAllHedgeCompletion();

                //this.GetCorrectPlacementCellPoint()
                //_baseCellObjsCached[0,0]
                //_cameraManager.EnableTravelFromAPointToBPoint()
            }
            return;

            //TestHasCompletedFullHedgeRowColPlaement(_mostRecentX, _mostRecentY);

            //if (_coinCount >= _hedgerowCoinCost) _coinCount -= _hedgerowCoinCost;

            //print("\n_totalSustainabilityScore = " + _totalSustainabilityScore);
        }
        private void InitAwardCameraSetupProcess(CellBorderType cellBorderType)
        {
            Vector3 startPos;
            Vector3 endPos;


            startPos = GetCorrectPlacementCellPoint(cellBorderType, true, _selectedField);
            endPos = GetCorrectPlacementCellPoint(cellBorderType, false, _selectedField);
            _cameraManager.EnableTravelFromAPointToBPoint(startPos, endPos);
            EventManager.SetEventFlag(EventFlagID.EventFlag_Hedgerow_FullPlacement_Character_Dialogue_Init);//, true);
            CharacterManager.SetCharacterDialogueID(CharacterDialogueID.Character_Dialogue_None);
            //if (cellBorderType == CellBorderType.Cell_Border_Top)
            //{
            //    startPos = GetCorrectPlacementCellPoint(CellBorderType.Cell_Border_Top, true, _selectedField);
            //    endPos = GetCorrectPlacementCellPoint(CellBorderType.Cell_Border_Top, false, _selectedField);
            //    _cameraManager.EnableTravelFromAPointToBPoint(startPos, endPos);
            //    EventManager.SetEventFlag(EventFlagID.EventFlag_Hedgerow_FullPlacement_Character_Dialogue_Init);//, true);
            //    CharacterManager.SetCharacterDialogueID(CharacterDialogueID.Character_Dialogue_None);
            //    return;
            //}

        }
        private bool AwardLeftHedgeCompletion()
        {
            if (_fullHedgePlacement[_selectedField].LEFT_COMPLETE &&
                !_fullHedgePlacement[_selectedField].LEFT_AWARDED)
            {
                //Activate reward event for this here.
                EventManager.TriggerEvent("Report_Left_Hedgerow_Award");
                _fullHedgePlacement[_selectedField].LEFT_AWARDED = true;
                InitAwardCameraSetupProcess(CellBorderType.Cell_Border_Left);
                return true;
            }
            return false;
        }
        private bool AwardRightHedgeCompletion()
        {
            if (_fullHedgePlacement[_selectedField].RIGHT_COMPLETE &&
                !_fullHedgePlacement[_selectedField].RIGHT_AWARDED)
            {
                //Activate reward event for this here.
                EventManager.TriggerEvent("Report_Right_Hedgerow_Award");
                _fullHedgePlacement[_selectedField].RIGHT_AWARDED = true;
                InitAwardCameraSetupProcess(CellBorderType.Cell_Border_Right);
                return true;
            }
            return false;
        }
        private bool AwardTopHedgeCompletion()
        {
            if (_fullHedgePlacement[_selectedField].TOP_COMPLETE &&
                !_fullHedgePlacement[_selectedField].TOP_AWARDED)
            {
                //Activate reward event for this here.
                EventManager.TriggerEvent("Report_Top_Hedgerow_Award");
                _fullHedgePlacement[_selectedField].TOP_AWARDED = true;
                InitAwardCameraSetupProcess(CellBorderType.Cell_Border_Top);
                return true;
            }
            return false;
        }
        private bool AwardBottomHedgeCompletion()
        {
            if (_fullHedgePlacement[_selectedField].BOTTOM_COMPLETE &&
                !_fullHedgePlacement[_selectedField].BOTTOM_AWARDED)
            {
                //Activate reward event for this here.
                EventManager.TriggerEvent("Report_Bottom_Hedgerow_Award");
                _fullHedgePlacement[_selectedField].BOTTOM_AWARDED = true;
                InitAwardCameraSetupProcess(CellBorderType.Cell_Border_Bottom);
                return true;
            }
            return false;
        }
        private bool AwardAllHedgeCompletion()
        {
            if (_fullHedgePlacement[_selectedField].ALL_COMPLETE &&
                !_fullHedgePlacement[_selectedField].ALL_AWARDED)
            {
                //Activate reward event for this here.
                EventManager.TriggerEvent("Report_AllSides_Hedgerow_Award");
                _fullHedgePlacement[_selectedField].ALL_AWARDED = true;
                return true;
            }
            return false;
        }

        private void ProcessHedgerow() => ProcessCoinSpending(_hedgerowCoinCost);
        private void ProcessFence() => ProcessCoinSpending(_fenceCoinCost);
        private void ProcessTree() => ProcessCoinSpending(_fenceCoinCost*3);
        private void ProcessCoinSpending(int costValue)
        {
            if (_coinCount >= costValue)
            {
                _coinCount -= costValue;
                if (_coinSplashFX != null)
                {
                    _fieldExpenditure[_selectedField] += costValue;
                    //EventManager.SetCoinCount(_coinCount);
                    //_coinSplashFX.main.duration = 0.65f;
                    _coinSplashFX.Play();
                    AudioManager.PlaySFX(AudioID.Audio_Coin_Spend_SFX);
                }
            }
            
        }
        public string GetOutputStr() => _outputStr;


        public int GetCoinCount() => _coinCount;
        public double GetTotalSustainabilityScore() => _totalSustainabilityScore;
        public double GetYieldScore(int fieldId) => _yeildScore[fieldId];
        public void DeductYieldScore(int fieldId)
        {
            int fieldCellWidthCount = 22;//_fieldGenerator.get
            int fieldCellHeightCount = 22;

            int totalCellCount = fieldCellWidthCount * fieldCellHeightCount;

            //Determine 1% of the total.
            double onePercent = totalCellCount * 0.01; // 4.84

            double v = 20.0;
            double scaled = onePercent / v;// 100.0;

            if (_yeildScore[fieldId] >= 0.0)
                _yeildScore[fieldId] -= scaled;// onePercent;
            else
                _yeildScore[fieldId] = 0.0;
        }
        public int GetHedgerowCost() => _hedgerowCoinCost;
        public int GetFenceCost() => _fenceCoinCost;
        public int GetCorrectHedgerowPlacementCount() => _correctHedgeRowPlacementsMade;
        public void SetSelectedField(int fieldId) => _selectedField = fieldId;
        public int GetSelectedField() => _selectedField;
        public int GetFieldExpenditure(int fieldId) => _fieldExpenditure[fieldId];
        public CellBorderType GetCellBorderTypeByID(int x, int y, int fieldId)
		{
            //return _baseCellObjsCached[x, y].GetCellBorderType(fieldId);// _selectedField);
            return _baseCellObjsCached[x, y].GetCellBorderType(_selectedField);
        }
        public Vector3 GetCellPositionByID(int x, int y)
		{
            return _baseCellObjsCached[x, y].transform.position;

        }
        public Transform GetCellTransformByID(int x, int y)
        {
            return _baseCellObjsCached[x, y].transform;

        }



        public void SetFieldCellBorder(int fieldId, int x, int y, CellBorderType cellBorderType)
        {

            HedgerowCorrectCell itemToAdd;
            itemToAdd.CELL_BORDER_TYPE = cellBorderType;
            itemToAdd.X = x;
            itemToAdd.Y = y;

            //if (_baseGridCellObjDict.TryGetValue(fieldGridCellID, out List<GameObject> listPlatforms)) listPlatforms.Add(newGameObject);
            switch (fieldId)
            {
                case 0:
                    //_field1_HedgerowValidBorderGridCellPos.Add(cellBorderType, new Vector2Int(x, y));
                    _field1_HedgerowValidBorderGridCellPos.Add(itemToAdd);
                    break;
                case 1:
                    _field2_HedgerowValidBorderGridCellPos.Add(itemToAdd);
                    break;
                case 2:
                    _field3_HedgerowValidBorderGridCellPos.Add(itemToAdd);
                    break;
                case 3:
                    _field4_HedgerowValidBorderGridCellPos.Add(itemToAdd);
                    break;
            }
        }
        public void HasCorrectHedgerowPlacementOccured(CellObj[,] grid, int x, int y)
        {
            _baseCellObjsCached = grid;

            //Has it been placed on any of the field border cells?
            if (_baseCellObjsCached[x, y].GetCellBorderType(_selectedField) != CellBorderType.Cell_Border_Unspecified)
            {
                //Yes, then we must process this in the scoring sum.
                //EventManager.ProcessHedgerowPlacementScoring();
                EventManager.TriggerEvent("Report_Placement_OnBorder_Hedgerow");

                _mostRecentX = x;
                _mostRecentY = y;
                //TriggerEvent calls ProcessHedgerowBorder() here, after maning the placement.
                //So we do the TestHasCompletedFullHedgeRowColPlaement(x, y) call there.
                //if (TestHasCompletedFullHedgeRowColPlaement(x, y))
                //{
                //Trigger visuals for completed correct row/col hedg placement
                //and trigger a reward.

                //}
            }


        }
        public Vector3 GetCorrectPlacementCellPoint(CellBorderType cellBorderType, bool isStartPont, int fieldId)
        {
            int x = 0;
            int y = 0;

            if (isStartPont) //START POINT
            {

                if (cellBorderType == CellBorderType.Cell_Border_Top)
                {
                    if (fieldId == 0)
                    {
                        x = _correctCellTopStart_field1.x;
                        y = _correctCellTopStart_field1.y;
                    }
                    if (fieldId == 1)
                    {
                        x = _correctCellTopStart_field2.x;
                        y = _correctCellTopStart_field2.y;
                    }
                    if (fieldId == 2)
                    {
                        x = _correctCellTopStart_field3.x;
                        y = _correctCellTopStart_field3.y;
                    }
                    if (fieldId == 3)
                    {
                        x = _correctCellTopStart_field4.x;
                        y = _correctCellTopStart_field4.y;
                    }

                    return _baseCellObjsCached[x, y].GetCellPosition();
                }
                if (cellBorderType == CellBorderType.Cell_Border_Bottom)
                {
                    if (fieldId == 0)
                    {
                        x = _correctCellBotStart_field1.x;
                        y = _correctCellBotStart_field1.y;
                    }
                    if (fieldId == 1)
                    {
                        x = _correctCellBotStart_field2.x;
                        y = _correctCellBotStart_field2.y;
                    }
                    if (fieldId == 2)
                    {
                        x = _correctCellBotStart_field3.x;
                        y = _correctCellBotStart_field3.y;
                    }
                    if (fieldId == 3)
                    {
                        x = _correctCellBotStart_field4.x;
                        y = _correctCellBotStart_field4.y;
                    }

                    return _baseCellObjsCached[x, y].GetCellPosition();
                }
                if (cellBorderType == CellBorderType.Cell_Border_Left)
                {
                    if (fieldId == 0)
                    {
                        x = _correctCellLeftStart_field1.x;
                        y = _correctCellLeftStart_field1.y;
                    }
                    if (fieldId == 1)
                    {
                        x = _correctCellLeftStart_field2.x;
                        y = _correctCellLeftStart_field2.y;
                    }
                    if (fieldId == 2)
                    {
                        x = _correctCellLeftStart_field3.x;
                        y = _correctCellLeftStart_field3.y;
                    }
                    if (fieldId == 3)
                    {
                        x = _correctCellLeftStart_field4.x;
                        y = _correctCellLeftStart_field4.y;
                    }

                    return _baseCellObjsCached[x, y].GetCellPosition();
                }
                if (cellBorderType == CellBorderType.Cell_Border_Right)
                {
                    if (fieldId == 0)
                    {
                        x = _correctCellRightStart_field1.x;
                        y = _correctCellRightStart_field1.y;
                    }
                    if (fieldId == 1)
                    {
                        x = _correctCellRightStart_field2.x;
                        y = _correctCellRightStart_field2.y;
                    }
                    if (fieldId == 2)
                    {
                        x = _correctCellRightStart_field3.x;
                        y = _correctCellRightStart_field3.y;
                    }
                    if (fieldId == 3)
                    {
                        x = _correctCellRightStart_field4.x;
                        y = _correctCellRightStart_field4.y;
                    }

                    return _baseCellObjsCached[x, y].GetCellPosition();
                }
            }

            if (!isStartPont) //END POINT
            {

                if (cellBorderType == CellBorderType.Cell_Border_Top)
                {
                    if (fieldId == 0)
                    {
                        x = _correctCellTopEnd_field1.x;
                        y = _correctCellTopEnd_field1.y;
                    }
                    if (fieldId == 1)
                    {
                        x = _correctCellTopEnd_field2.x;
                        y = _correctCellTopEnd_field2.y;
                    }
                    if (fieldId == 2)
                    {
                        x = _correctCellTopEnd_field3.x;
                        y = _correctCellTopEnd_field3.y;
                    }
                    if (fieldId == 3)
                    {
                        x = _correctCellTopEnd_field4.x;
                        y = _correctCellTopEnd_field4.y;
                    }

                    return _baseCellObjsCached[x, y].GetCellPosition();
                }
                if (cellBorderType == CellBorderType.Cell_Border_Bottom)
                {
                    if (fieldId == 0)
                    {
                        x = _correctCellBotEnd_field1.x;
                        y = _correctCellBotEnd_field1.y;
                    }
                    if (fieldId == 1)
                    {
                        x = _correctCellBotEnd_field2.x;
                        y = _correctCellBotEnd_field2.y;
                    }
                    if (fieldId == 2)
                    {
                        x = _correctCellBotEnd_field3.x;
                        y = _correctCellBotEnd_field3.y;
                    }
                    if (fieldId == 3)
                    {
                        x = _correctCellBotEnd_field4.x;
                        y = _correctCellBotEnd_field4.y;
                    }

                    return _baseCellObjsCached[x, y].GetCellPosition();
                }
                if (cellBorderType == CellBorderType.Cell_Border_Left)
                {
                    if (fieldId == 0)
                    {
                        x = _correctCellLeftEnd_field1.x;
                        y = _correctCellLeftEnd_field1.y;
                    }
                    if (fieldId == 1)
                    {
                        x = _correctCellLeftEnd_field2.x;
                        y = _correctCellLeftEnd_field2.y;
                    }
                    if (fieldId == 2)
                    {
                        x = _correctCellLeftEnd_field3.x;
                        y = _correctCellLeftEnd_field3.y;
                    }
                    if (fieldId == 3)
                    {
                        x = _correctCellLeftEnd_field4.x;
                        y = _correctCellLeftEnd_field4.y;
                    }

                    return _baseCellObjsCached[x, y].GetCellPosition();
                }
                if (cellBorderType == CellBorderType.Cell_Border_Right)
                {
                    if (fieldId == 0)
                    {
                        x = _correctCellRightEnd_field1.x;
                        y = _correctCellRightEnd_field1.y;
                    }
                    if (fieldId == 1)
                    {
                        x = _correctCellRightEnd_field2.x;
                        y = _correctCellRightEnd_field2.y;
                    }
                    if (fieldId == 2)
                    {
                        x = _correctCellRightEnd_field3.x;
                        y = _correctCellRightEnd_field3.y;
                    }
                    if (fieldId == 3)
                    {
                        x = _correctCellRightEnd_field4.x;
                        y = _correctCellRightEnd_field4.y;
                    }

                    return _baseCellObjsCached[x, y].GetCellPosition();
                }
            }


            return Vector3.zero;
        }
        private bool HasCompletedFullHedgeRowColPlaement()//int curX, int curY)
        {
            if (_fieldGenerator == null) return false;

            int currentFieldId = _selectedField;

            int curX = _fieldGenerator.GetGridStartX(currentFieldId) - 1;
            int curY = _fieldGenerator.GetGridStartY(currentFieldId) - 1;
            int maxX = (curX + 1) + _fieldGenerator.GetGridX(currentFieldId) + 1; //_maxGridWidth;
            int maxY = (curY + 1) + _fieldGenerator.GetGridY(currentFieldId) + 1; //_maxGridHeight;

            List<HedgerowCorrectCell> validBorderCells = new List<HedgerowCorrectCell>();

            if (currentFieldId == 0)
            {
                validBorderCells = _field1_HedgerowValidBorderGridCellPos;
            }
            if (currentFieldId == 1)
            {
                validBorderCells = _field2_HedgerowValidBorderGridCellPos;
            }
            if (currentFieldId == 2)
            {
                validBorderCells = _field3_HedgerowValidBorderGridCellPos;
            }
            if (currentFieldId == 3)
            {
                validBorderCells = _field4_HedgerowValidBorderGridCellPos;
            }

            if (validBorderCells == null || validBorderCells.Count == 0)
                return false;

            int validBorderTypeCount = validBorderCells.Count;
            //int validIncrement = 0; //<-- works but only for all 4 sides.
            int validLeftCount = 0;
            int validRightCount = 0;
            int validTopCount = 0;
            int validBottomCount = 0;

            //Don't forget to insert the current x & y! otherwise we will always start from 0
            //and detect/look for the first field!

            for (int x = curX; x < maxX; x++)
            {
                for (int y = curY; y < maxY; y++)
                {
                    if (_baseCellObjsCached[x, y].GetCellCurrentSurfaceObj() == CellSurfObjType.Cell_Surface_Hedge)
                    {
                        if (IsHedgeBorder(_baseCellObjsCached[x, y].GetCellBorderType(_selectedField), CellBorderType.Cell_Border_Top))
                        {
                            //validIncrement++;
                            //print("\nvalidIncrement=" + validIncrement.ToString());
                            //if (validIncrement == validBorderTypeCount) return true;
                            validTopCount++;
                        }
                        if (IsHedgeBorder(_baseCellObjsCached[x, y].GetCellBorderType(_selectedField), CellBorderType.Cell_Border_Bottom))
                            validBottomCount++;
                        if (IsHedgeBorder(_baseCellObjsCached[x, y].GetCellBorderType(_selectedField), CellBorderType.Cell_Border_Left))
                            validLeftCount++;
                        if (IsHedgeBorder(_baseCellObjsCached[x, y].GetCellBorderType(_selectedField), CellBorderType.Cell_Border_Right))
                            validRightCount++;
                    }

                }
            }


            //if ()
            //extra small
            //BOT or TOP == 7
            //left OR right == 5
            //so plus 2 to Y(TOP OR BOT) for any size field and
            //X will just be its value.

            int currentFieldSizeLeft = _fieldGenerator.GetGridX(currentFieldId);
            int currentFieldSizeRight = currentFieldSizeLeft;

            int currentFieldSizeTop = _fieldGenerator.GetGridY(currentFieldId) + 2;
            int currentFieldSizeBot = currentFieldSizeTop;


            int totalValidCount = validTopCount + validBottomCount + validLeftCount + validRightCount;

            //if (totalValidCount == validBorderCells.Count)
            //    print("\nvalidTOTALCount=" + totalValidCount.ToString());

            //print("\nvalidTOPCount=" + validTopCount.ToString());
            //print("\nvalidBOTCount=" + validBottomCount.ToString());
            //print("\nvalidLEFTCount=" + validLeftCount.ToString());
            //print("\nvalidRIGHTCount=" + validRightCount.ToString());


            if (!_fullHedgePlacement[currentFieldId].LEFT_COMPLETE)
            {
                if (currentFieldSizeLeft == validLeftCount)
                {
                    _fullHedgePlacement[currentFieldId].LEFT_COMPLETE = true;
                    HasPartCompletedToConnectedFieldCheck(currentFieldId);
                    return true;
                }
            }

            if (!_fullHedgePlacement[currentFieldId].RIGHT_COMPLETE)
            {
                if (currentFieldSizeRight == validRightCount)
                {
                    _fullHedgePlacement[currentFieldId].RIGHT_COMPLETE = true;
                    HasPartCompletedToConnectedFieldCheck(currentFieldId);
                    return true;
                }
            }

            if (!_fullHedgePlacement[currentFieldId].TOP_COMPLETE)
            {
                if (currentFieldSizeTop == validTopCount)
                {
                    _fullHedgePlacement[currentFieldId].TOP_COMPLETE = true;
                    HasPartCompletedToConnectedFieldCheck(currentFieldId);
                    return true;
                }
            }

            if (!_fullHedgePlacement[currentFieldId].BOTTOM_COMPLETE)
            {
                if (currentFieldSizeBot == validBottomCount)
                {
                    _fullHedgePlacement[currentFieldId].BOTTOM_COMPLETE = true;
                    HasPartCompletedToConnectedFieldCheck(currentFieldId);
                    return true;
                }
            }

            if (!_fullHedgePlacement[currentFieldId].ALL_COMPLETE)
            {
                if (currentFieldSizeLeft == validLeftCount && currentFieldSizeRight == validRightCount &&
                    currentFieldSizeTop + 2 == validTopCount && currentFieldSizeBot + 2 == validBottomCount)
                {
                    _fullHedgePlacement[currentFieldId].LEFT_COMPLETE = true;
                    _fullHedgePlacement[currentFieldId].RIGHT_COMPLETE = true;
                    _fullHedgePlacement[currentFieldId].TOP_COMPLETE = true;
                    _fullHedgePlacement[currentFieldId].BOTTOM_COMPLETE = true;
                    _fullHedgePlacement[currentFieldId].ALL_COMPLETE = true;
                    return true;
                }

            }


            return false;
        }

        private void HasPartCompletedToConnectedFieldCheck(int currentFieldId)
		{
            //If field 1 then check and identify for field 3 and 2 (neighbour fields of 1)
            if (currentFieldId == 0)
			{
                if (_fullHedgePlacement[currentFieldId].BOTTOM_COMPLETE)
                {
                    _fullHedgePlacement[2].TOP_COMPLETE = true;
                }
                if (_fullHedgePlacement[currentFieldId].RIGHT_COMPLETE)
                {
                    _fullHedgePlacement[1].LEFT_COMPLETE = true;
                }
                return;
            }

            //If field 2 then check and identify for field 1 and 4 (neighbour fields of 2)
            if (currentFieldId == 1)
            {
                if (_fullHedgePlacement[currentFieldId].RIGHT_COMPLETE)
                {
                    _fullHedgePlacement[0].RIGHT_COMPLETE = true;
                }
                if (_fullHedgePlacement[currentFieldId].BOTTOM_COMPLETE)
                {
                    _fullHedgePlacement[3].TOP_COMPLETE = true;
                }
                return;
            }

            //If field 3 then check and identify for field 1 and 4 (neighbour fields of 3)
            if (currentFieldId == 2)
            {
                if (_fullHedgePlacement[currentFieldId].TOP_COMPLETE)
                {
                    _fullHedgePlacement[0].BOTTOM_COMPLETE = true;
                }
                if (_fullHedgePlacement[currentFieldId].RIGHT_COMPLETE)
                {
                    _fullHedgePlacement[3].LEFT_COMPLETE = true;
                }
                return;
            }

            //If field 4 then check and identify for field 3 and 2 (neighbour fields of 4)
            if (currentFieldId == 3)
            {
                if (_fullHedgePlacement[currentFieldId].LEFT_COMPLETE)
                {
                    _fullHedgePlacement[2].RIGHT_COMPLETE = true;
                }
                if (_fullHedgePlacement[currentFieldId].TOP_COMPLETE)
                {
                    _fullHedgePlacement[1].BOTTOM_COMPLETE = true;
                }
                return;
            }
        }

        private bool IsHedgeBorder(CellBorderType cellBorderType, CellBorderType typeToCheckAgainst)
        {
            if (cellBorderType == typeToCheckAgainst) return true;

            return false;
        }


    }
}
