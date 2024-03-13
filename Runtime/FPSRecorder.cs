using HCG.Extensions;
using UnityEngine;

namespace HCGames.Utilities
{
    public class FPSRecorder : Singleton<FPSRecorder>
    {
        private int _averageFPS;
        private int _highestFPS;
        private int _lowestFPS;
        private int _frameCount;
        private int _totalFps;

        private int _startFrameCount;
        private bool _startedRecording;

        protected override void Awake()
        {
            base.Awake();
            _startFrameCount = Time.frameCount;
            Reset();
        }

        private void Update()
        {
            if (Time.frameCount < _startFrameCount + 200)
            {
                return;
            }

            if (!_startedRecording)
            {
                _startedRecording = true;
            }

            CalculateFPS();
        }

        public void Reset()
        {
            _averageFPS = 0;
            _highestFPS = 0;
            _lowestFPS = int.MaxValue;
            _totalFps = 0;
            _frameCount = 0;
        }

        private void CalculateFPS()
        {
            _frameCount++;
            var fps = (int)(1f / Time.unscaledDeltaTime);
            _totalFps += fps;
            _averageFPS = _totalFps / _frameCount;
            if (_highestFPS < fps)
            {
                _highestFPS = fps;
            }

            if (_lowestFPS > fps)
            {
                _lowestFPS = fps;
            }
        }

        private Vector3Int? GetFpsInfo()
        {
            Vector3Int? result = null;
            if (_startedRecording)
            {
                result = new Vector3Int(_averageFPS, _highestFPS, _lowestFPS);
            }

            Reset();
            return result;
        }

        public string GetDeviceFpsString()
        {
            var fpsInfo = GetFpsInfo();
            return fpsInfo == null ? $"Average(null)-Highest(null)-Lowest(null)" : $"Average({fpsInfo.Value.x})-Highest({fpsInfo.Value.y})-Lowest({fpsInfo.Value.z})";
        }
    }
}