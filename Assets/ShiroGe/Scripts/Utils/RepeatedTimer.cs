using ShiroGe.Scripts.World;
using System;
using UnityEngine;

namespace ShiroGe.Scripts.Utils
{
    public class RepeatedTimer
    {
        private float _duration;
        private Action _onComplete;
        private Action<float> _onRemainingChanged;
        private int _currentTimerId = -1;
        private bool _isActive = false;
    
        public RepeatedTimer(float duration, Action onComplete, Action<float> onRemainingChanged = null)
        {
            _duration = duration;
            _onComplete = onComplete;
            _onRemainingChanged = onRemainingChanged;
        }
    
        public void Start()
        {
            if (_isActive) return;
            _isActive = true;
            ScheduleNext();
        }
    
        public void Stop()
        {
            _isActive = false;
            if (_currentTimerId != -1)
            {
                TimerService.Instance.RemoveTimer(_currentTimerId);
                _currentTimerId = -1;
            }
        }
    
        private void ScheduleNext()
        {
            if (!_isActive) return;

            if (_onRemainingChanged != null)
            {
                _currentTimerId = TimerService.Instance.AddTimer(
                    _duration,
                    OnTimerComplete,
                    (remaining) =>
                    {
                        _onRemainingChanged?.Invoke(remaining);
                    }
                );
            }
            else
            {
                _currentTimerId = TimerService.Instance.AddTimer(
                    _duration,
                    OnTimerComplete,
                    null
                );
            }
        }

    
        private void OnTimerComplete()
        {
            _currentTimerId = -1;
            _onComplete?.Invoke();
        
            if (_isActive)
            {
                ScheduleNext();
            }
        }
    }
}