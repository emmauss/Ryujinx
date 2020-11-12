using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using Easing;

namespace Ryujinx.Skia.Ui.Skia
{
    public class Animation
    {
        private static readonly System.Threading.Timer _globalTimer;
        private static List<Animation> _animations;
        private static readonly List<Animation> _clearList;

        static Animation()
        {
            _animations = new List<Animation>();
            _globalTimer = new System.Threading.Timer((x) => Update(), null, 0, 16);
            _clearList = new List<Animation>();
        }

        public static void Commit(Animation animation)
        {
            lock (_animations)
            {
                _animations.Add(animation);

                _globalTimer.Change(0, 16);
            }
        }

        public static void Remove(Animation animation)
        {
            lock (_animations)
            {
                lock (_clearList)
                {
                    if (!_clearList.Contains(animation))
                    {
                        _clearList.Add(animation);
                    }
                }
            }
        }

        public static void Update()
        {
            lock (_animations)
            {
                if (_animations.Count == 0)
                {
                    _clearList.Clear();

                    return;
                }

                for (int i = 0; i < _animations.Count; i++)
                {
                    Animation animation = _animations[i];

                    animation?.UpdateValue();
                }

                lock (_clearList)
                {
                    for (int i = 0; i < _clearList.Count; i++)
                    {
                        Animation animation = _clearList[i];
                        var animationIndex = _animations.FindIndex((x) => x == animation);
                        if (animationIndex > -1)
                        {
                            _animations.RemoveAt(animationIndex);
                        }

                        _clearList.RemoveAll(x => x == animation);
                    }
                }

                if (_animations.Count == 0)
                {
                    _globalTimer.Change(0, int.MaxValue);
                }
            }
        }

        private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private Easing.Ease _ease;
        private long _startTime;
        private double _fromValue;
        private double _toValue;
        private long _duration;

        private double _distance;

        private Action<double> _frameCallback;

        private Action _endCallback;
        private Action _startCallback;
        private bool _initialized;
        private Animation _nextAnimation;
        private bool _isActive;
        private bool _isUpdating;

        public bool IsActive {
            get
            {
                bool active = false;
                if (_nextAnimation != null)
                {
                    active = _nextAnimation.IsActive;
                }
                return _isActive || active;
            }
            set => _isActive = value; }

        public Animation()
        {
        }

        private void UpdateValue()
        {
            if (_isUpdating)
            {
                return;
            }

            _isUpdating = true;
            long elapsed = _stopwatch.ElapsedMilliseconds;

            long delta = elapsed - _startTime;

            delta = delta > _duration ? _duration : delta;

            double value = _ease.InOut(delta) + _fromValue;

            _frameCallback?.Invoke(value);

            if (delta >= _duration)
            {
                Stop();

                _nextAnimation?.Play();
            }

            _isUpdating = false;
        }

        public void Play()
        {
            if (_initialized)
            {
                IsActive = true;

                _startTime = _stopwatch.ElapsedMilliseconds;

                _distance = _toValue - _fromValue;

                _ease = new Linear(new Vector(_duration, (float)_distance));

                Animation.Commit(this);

                _startCallback?.Invoke();
            }
            else
            {
                throw new Exception("Animation has not been initialized");
            }
        }

        public void ContinueWith(Animation animation)
        {
            _nextAnimation = animation;
        }

        public void Stop(bool invokeCallback = true)
        {
            Remove(this);

            if (invokeCallback)
            {
                Task.Run(() =>
                {
                    _endCallback?.Invoke();

                    IsActive = false;
                })
                    ;
            }
            else
            {
                IsActive = false;
            }
        }

        public void Restart()
        {
            Play();
        }

        public void With(double from,
                         double to,
                         long duration,
                         Action<double> frameCallback = null,
                         Action startCallback = null,
                         Action endCallback = null)
        {
            _fromValue = from;
            _toValue = to;
            _duration = duration;

            _frameCallback = frameCallback;
            _endCallback = endCallback;
            _startCallback = startCallback;

            _initialized = true;
        }
    }
}