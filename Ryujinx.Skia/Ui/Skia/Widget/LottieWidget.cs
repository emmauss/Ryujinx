using LottieSharp;
using Ryujinx.Skia.Ui.Skia.Scene;
using SkiaSharp;
using SkiaSharp.Elements;
using System;
using System.IO;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class LottieWidget : UIElement
    {
        public event EventHandler Stopped;

        public event EventHandler Paused;
        public event EventHandler Started;

        private readonly LottieDrawable _drawable;
        public LottieComposition Composition { get; private set; }

        public SKRect ScaledBounds { get; private set; }

        public LottieWidget(SKRect bounds)
        {
            Bounds = bounds;
            _drawable = new LottieDrawable
            {
                RepeatCount = LottieDrawable.Infinite,
                RepeatMode = RepeatMode.Restart
            };
            SetOverlayAlpha(0);
        }

        public void Load(string path)
        {
            Stream file = File.OpenRead(path);

            Composition = LottieCompositionFactory.FromJsonInputStreamSync(file, "test").Value;

            _drawable.SetComposition(Composition);
            _drawable.Start();
        }

        public void Load(Stream jsonStream)
        {
            Composition = LottieCompositionFactory.FromJsonInputStreamSync(jsonStream, "test").Value;

            _drawable.SetComposition(Composition);
            _drawable.Start();
        }

        public override void Draw(SkiaSharp.SKCanvas canvas)
        {
            base.Draw(canvas);

            if (!DrawElement)
            {
                return;
            }

            canvas.Save();

            _drawable.Draw(canvas, Bounds);

            canvas.RestoreToCount(-1);
        }

        public void Play()
        {
            _drawable.PlayAnimation();
            Started?.Invoke(this, null);
        }

        public override void FadeOut(){
            Animator?.Stop();
            Animator = new Animation();
            Animator.With(255, 0, 1000, SetAlpha , endCallback: Stop);
            Animator.Play();
        }

        public override void FadeIn(){

        }

        public void Pause()
        {
            _drawable.PauseAnimation();
            Paused?.Invoke(this, null);
        }

        public void Stop(){
            _drawable.Stop();
            Stopped?.Invoke(this, null);
        }

        public float Speed
        {
            get
            {
                return _drawable.Speed;
            }
            set
            {
                _drawable.Speed = value;
            }
        }

        public override bool IsPointInside(SKPoint point)
        {
            return ScaledBounds.Contains(point);
        }

        public override void Measure()
        {
        }

        public override void Measure(SKRect bounds)
        {
            Bounds = bounds;
        }

        public override void Dispose()
        {
            _drawable.Dispose();
        }

        public void SetAlpha(double value){
            _drawable.SetAlpha((byte)value);

            IManager.Instance?.InvalidateMeasure();
        }
    }
}