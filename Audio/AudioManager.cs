
using System.Collections.Generic;
using UnityEngine;
using wildlogicgames;

namespace IsometricFarmGenerator
{
    public enum AudioID
    {
        Audio_Placement_SFX,
        Audio_Dig_SFX, 
        Audio_But_Click01_SFX,
        Audio_But_Click02_SFX,
        Audio_But_Click03_SFX,
        Audio_Coin_Spend_SFX,
        Audio_Cheering01_SFX,
    };

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager _instance = null;

        private static Transform _transform;

        private static float _volumeSFX, _volumeMUSIC, _volumeDIALOGUE;

        private static Dictionary<AudioID, WAudio> _audioDict;

        private void Awake() => Setup();
        private void Setup()
        {
            
            if (_instance == null)
            {
                _instance = this;
                _transform = this.transform;


                _volumeSFX = 0.75f;
                _volumeMUSIC = 0.75f;
                _volumeDIALOGUE = 0.75f;

                if (_audioDict == null) _audioDict = new Dictionary<AudioID, WAudio>();

                _audioDict.Add(AudioID.Audio_Placement_SFX, new WAudio("SFX_Placement_01", _volumeSFX, false, 1.0f, _transform));
                _audioDict.Add(AudioID.Audio_Dig_SFX, new WAudio("SFX_Dig_01", _volumeSFX, false, 1.0f, _transform));
                _audioDict.Add(AudioID.Audio_But_Click01_SFX, new WAudio("SFX_ButClick_01", _volumeSFX, false, 1.0f, _transform));
                _audioDict.Add(AudioID.Audio_But_Click02_SFX, new WAudio("SFX_ButClick_02", _volumeSFX, false, 1.0f, _transform));
                _audioDict.Add(AudioID.Audio_But_Click03_SFX, new WAudio("SFX_ButClick_03", _volumeSFX, false, 1.0f, _transform));
                _audioDict.Add(AudioID.Audio_Coin_Spend_SFX, new WAudio("SFX_CoinSpend_01", 0.33f, false, 1.0f, _transform));//coinsSpendSFX
                _audioDict.Add(AudioID.Audio_Cheering01_SFX, new WAudio("SFX_Cheering_01", 0.85f, false, 1.0f, _transform));
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }

        public static void PlaySFX(AudioID audioID)
        {
            if (_audioDict == null || !_audioDict.ContainsKey(audioID)) return;
            var soundEffect = _audioDict[audioID];
            if (soundEffect.AudioSrc() == null) return;

            _audioDict[audioID].PlaySound();
        }

        public static void StopSFX(AudioID audioID)
        {
            if (_audioDict == null || !_audioDict.ContainsKey(audioID)) return;
            var soundEffect = _audioDict[audioID];
            if (soundEffect.AudioSrc() == null) return;

            _audioDict[audioID].StopSound();
        }
    }
}
