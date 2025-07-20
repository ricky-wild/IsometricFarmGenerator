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

    public class RulescopeManager : MonoBehaviour
    {
        public static RulescopeManager _instance;

        [Header("Rulescope Ref")]
        public RuleScope _ruleScopeRef;

        private void Awake() => Setup();
        private void Setup()
        {
            if (_instance == null)
            {
                _instance = this;

                DontDestroyOnLoad(this.gameObject);
            }
        }

        public RuleScope GetRulescope() => _ruleScopeRef;
    }
}
