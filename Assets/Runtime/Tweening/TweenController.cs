﻿using System;
using System.Collections.Generic;
using AuraTween;
using Cysharp.Threading.Tasks;
using Resat.Models;
using UnityEngine;

namespace Resat.Tweening
{
    // specific tweening utils for things like the camera viewport
    public class TweenController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField]
        private TweenManager _tweenManager = null!;

        // not sure if this is really the best place for this, but you gotta put it somewhere
        [Header("Durations")] 
        [SerializeField]
        private float _biomeImageTransitionDuration = 1f;
        
        [SerializeField]
        private float _biomeSkyboxTransitionDuration = 1f;
        
        [SerializeField]
        private float _biomeLightingTransitionDuration = 1f;

        // idk if this is overkill/already implemented in AuraTween but i'm feeling the crunch
        private Dictionary<ColorTweenType, Tween> _tweensByColorType = new();
        
        // this method is essentially just syntax sugar for auratween
        public async UniTask RunTween(float duration, Action<float> run, Ease ease = Ease.Linear, float start = 0f, float end = 1f)
        {
            await _tweenManager.Run(start, end, duration, run, ease.ToProcedure(), this);
        }

        // returns false if cancelled
        public async UniTask<bool> TweenColors(Color startColor, Color endColor, Action<Color> run, ColorTweenType invoker, Ease ease = Ease.Linear)
        {
            // Cancel tween on same object, if existing
            // Used for cancelling and retrying a color tween (eg if the user is moving between two biomes rapidly)
            if (_tweensByColorType.TryGetValue(invoker, out Tween existingTween))
            {
                Debug.Log("Cancelling...");
                existingTween.Cancel();
                _tweensByColorType.Remove(invoker);
            }
            
            // To smoothly lerp between color values 
            var tween = _tweenManager.Run(startColor, endColor, GetDurationFromTweenType(invoker), run, ease.ToProcedure(), HSV, this);
            
            _tweensByColorType.Add(invoker, tween);

            await tween;
            
            // weird hack, don't see a cancellation property on the tween
            bool cancelled = _tweensByColorType.TryGetValue(invoker, out Tween activeTween) && activeTween.IsAlive;

            if (!cancelled)
            {
                _tweensByColorType.Remove(invoker);
            }
            
            return !cancelled;
        }

        public async UniTask<bool> TweenVectors(Vector2 startVector, Vector2 endVector, float duration, Action<Vector2> run, Ease xEase = Ease.Linear, Ease yEase = Ease.Linear)
        {
            var xEaser = xEase.ToProcedure();
            var yEaser = yEase.ToProcedure();
            
            // Fairly fucked way to make this work with unmatched eases
            // I want to use the same tween if at all possible though
            var tween = _tweenManager.Run(0f, 1f, duration, (value) =>
            {
                // scuffed lerp based on the linear tween
                run.Invoke(InterpolateVector(startVector, endVector, xEaser, yEaser, value));
            }, Ease.Linear.ToProcedure(), this);

            await tween;

            return true;
        }

        private float GetDurationFromTweenType(ColorTweenType tweenType)
        {
            return tweenType switch
            {
                ColorTweenType.ImageBehaviourColor => _biomeImageTransitionDuration,
                ColorTweenType.SkyboxBottom => _biomeSkyboxTransitionDuration,
                ColorTweenType.SkyboxTop => _biomeSkyboxTransitionDuration,
                ColorTweenType.Lighting => _biomeLightingTransitionDuration,
                _ => 1f
            };
        }

        private Vector2 InterpolateVector(Vector2 start, Vector2 end, EaseProcedure xEase, EaseProcedure yEase, float time)
        {
            float xTime = time;
            float yTime = time;
            
            var xLerp = xEase(ref xTime);
            var yLerp = yEase(ref yTime);

            var x = Mathf.Lerp(start.x, end.x, xLerp);
            var y = Mathf.Lerp(start.y, end.y, yLerp);

            return new Vector2(x, y);
        }

        // TODO: OKHSL
        private static Color HSV(ref Color start, ref Color end, ref float time)
        {
            Color.RGBToHSV(start, out var startH, out var startS, out var startV);
            Color.RGBToHSV(end, out var endH, out var endS, out var endV);
            var h = Mathf.Lerp(startH, endH, time);
            var s = Mathf.Lerp(startS, endS, time);
            var v = Mathf.Lerp(startV, endV, time);
            return Color.HSVToRGB(h, s, v);
        }
    }
}