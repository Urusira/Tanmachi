using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ShiroGe.Scripts.World
{
    public class SkyboxManager : MonoBehaviour
    {
        
        [SerializeField] private Material skyboxMaterial;
        
        [SerializeField] private bool auroraEnabled;
        [SerializeField] private float initialAuroraIntensity;
        public float auroraAwakeningTime = 2000;
        public float auroraOutTime = 3100;
        public float auroraFadeSpeed = 1;
        
        [SerializeField] private float initialStarsIntensity;
        public float starsAwakeningTime = 1500;
        public float starsOutTime = 3000;
        public float starsFadeSpeed = 15;
        
        private TimeManager _timeManager;
        
        private float _currentAuroraIntensity = 0f;
        private float _currentStarsIntensity = 0f;
        
        private static readonly int AuroraIntensityShaderId = Shader.PropertyToID("_AuroraIntensity");
        private static readonly int StarsIntensityShaderId = Shader.PropertyToID("_StarIntensity");

        private void Start()
        {
            _timeManager = GetComponent<TimeManager>();
            
            float auroraIntensValue = skyboxMaterial.GetFloat(AuroraIntensityShaderId);
            initialAuroraIntensity = auroraIntensValue != 0 && !Mathf.Approximately(auroraIntensValue, initialAuroraIntensity) ? auroraIntensValue : initialAuroraIntensity;
            _currentAuroraIntensity = initialAuroraIntensity;
            
            float starsIntensValue = skyboxMaterial.GetFloat(StarsIntensityShaderId);
            initialStarsIntensity = starsIntensValue != 0 && !Mathf.Approximately(starsIntensValue, initialStarsIntensity) ? starsIntensValue : initialStarsIntensity;
            _currentStarsIntensity = initialStarsIntensity;

            if (_timeManager.CurrentDay == 0)
            {
                _currentAuroraIntensity = 0;
                skyboxMaterial.SetFloat(AuroraIntensityShaderId, _currentAuroraIntensity);
                _currentStarsIntensity = 0;
                skyboxMaterial.SetFloat(StarsIntensityShaderId, _currentStarsIntensity);
            }
        }

        private void Update()
        {
            if (_timeManager.CurrentTime < auroraOutTime & _timeManager.CurrentTime > auroraAwakeningTime)
            {
                if(auroraEnabled)
                {
                    _currentAuroraIntensity =
                        Mathf.Clamp(_currentAuroraIntensity + Time.deltaTime * _timeManager.CurrentTimeFactor * auroraFadeSpeed, 0,
                            initialAuroraIntensity);
                    skyboxMaterial.SetFloat(AuroraIntensityShaderId, _currentAuroraIntensity);
                }
            }
            else
            {
                if (auroraEnabled)
                {
                    _currentAuroraIntensity =
                        Mathf.Clamp(_currentAuroraIntensity - Time.deltaTime * _timeManager.CurrentTimeFactor * auroraFadeSpeed, 0,
                            initialAuroraIntensity);
                    skyboxMaterial.SetFloat(AuroraIntensityShaderId, _currentAuroraIntensity);
                }
            }

            if (_timeManager.CurrentTime < starsOutTime & _timeManager.CurrentTime > starsAwakeningTime)
            {
                _currentStarsIntensity = Mathf.Clamp(_currentStarsIntensity + Time.deltaTime * _timeManager.CurrentTimeFactor * starsFadeSpeed, 0, 
                    initialStarsIntensity);
                skyboxMaterial.SetFloat(StarsIntensityShaderId, _currentStarsIntensity);
            }
            else
            {
                _currentStarsIntensity = Mathf.Clamp(_currentStarsIntensity - Time.deltaTime * _timeManager.CurrentTimeFactor * starsFadeSpeed, 0,
                    initialStarsIntensity);
                skyboxMaterial.SetFloat(StarsIntensityShaderId, _currentStarsIntensity);
            }
        }

        private void OnDestroy()
        {
            skyboxMaterial.SetFloat(AuroraIntensityShaderId, initialAuroraIntensity);
            skyboxMaterial.SetFloat(StarsIntensityShaderId, initialStarsIntensity);
        }
    }
}