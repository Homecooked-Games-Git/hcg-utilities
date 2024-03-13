using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HCGames.Utilities
{
    public static class Tweens
    {
        public static Sequence FloatingText(TextMeshProUGUI tmp, float yMove, float scaleMultiplier = 1,float speedMultiplier = 1,bool shake = false)
        {
            var speed = speedMultiplier * UnityEngine.Random.Range(0.8f, 1f);  // Adjust speed randomly for variety
            var sequence = DOTween.Sequence();
            sequence.PrependCallback(() =>
            {
                tmp.transform.localScale = Vector3.one * (1.5f * scaleMultiplier);  // Start with a slightly bigger scale
                tmp.alpha = 1;
                tmp.gameObject.SetActive(true);
            });
            

            sequence.Append(tmp.transform.DOScale(Vector3.one * 0.9f, 0.4f / speed)); // Gradually scale down to 90%
            if (shake)
            {
                sequence.Join(tmp.transform.DOShakeRotation(0.15f / speed,30f,50));
                sequence.Join(tmp.transform.DOShakePosition(0.25f / speed,new Vector3(50,0,0),50));
            }
            sequence.Join(tmp.transform.DOLocalMoveY(yMove, 0.3f / speed).SetEase(Ease.OutQuart).SetRelative()); // Move on Y with the OutBack easing function for a "jump" effect
            sequence.Join(tmp.DOFade(0, 0.5f / speed).SetDelay(0.5f / speed)); // Fade out slower starting halfway through
            return sequence;
        }
        
        public static Tween ProjectileTween(int side, ParticleSystem projectileEffect)
        {
            var endPos = Vector3.zero;
            var startPos = projectileEffect.transform.localPosition;
            var xDiff = Mathf.Abs(endPos.x - startPos.x);
            var xDiffRatio = Mathf.Lerp(0.5f, 0.8f, xDiff / 800);
            var isStartClose = side == 1 ? startPos.x > endPos.x : startPos.x < endPos.x;
            var closePos = isStartClose ? startPos : endPos;
            var farPos = isStartClose ? endPos : startPos;
            var midY = Mathf.Lerp(farPos.y, closePos.y, xDiffRatio);
            var midX = closePos.x + 200f * side;
            var midPos = new Vector3(midX, midY,0);
            var a = startPos + new Vector3(side * Mathf.Abs(midPos.x - startPos.x)/2, 0, 0);
            var b = midPos - new Vector3(0, (midPos.y - startPos.y)/2, 0);
            var c = midPos + new Vector3(0, (endPos.y - midPos.y)/2, 0);
            var d = endPos + new Vector3(side * Mathf.Abs(midPos.x - endPos.x)/2, 0, 0);
            return projectileEffect.transform
                .DOLocalPath(new[] { midPos, a, b, Vector3.zero, c, d }, 2750, PathType.CubicBezier,
                    gizmoColor: Color.green)
                .SetEase(Ease.Linear)
                .SetUpdate(true)
                .SetSpeedBased(true);
        }

        public static Sequence GlowAndScale(GameObject go, Image glow)
        {
            var sequence = DOTween.Sequence();
            sequence.AppendCallback(() =>
            {
                go.transform.localScale = Vector3.one;
                glow.gameObject.SetActive(true);
                var color = glow.color;
                color.a = 0;
                glow.color = color;
            });
            sequence.Append(go.transform.DOScale(1.05f, 0.15f).SetLoops(4, LoopType.Yoyo));
            sequence.Join(glow.DOFade(1, 0.15f).SetLoops(4, LoopType.Yoyo));
            return sequence;
        }
        
        public static Sequence GlowAndFadeImage(Image image, float duration = 0.3f)
        {
            var sequence = DOTween.Sequence().SetEase(Ease.InSine);
            sequence.PrependCallback(() =>
            {
                var color = image.color;
                color.a = 1;
                image.color = color;
                image.gameObject.SetActive(true);
            });
            sequence.Append(image.DOFade(0, duration));
            sequence.OnComplete(() => { image.gameObject.SetActive(false); });
            return sequence;
        }

        public static Sequence Float(Transform transform, float yPos,float zRotation, float duration)
        {
            var sequence = DOTween.Sequence();
            var position = transform.position;
            sequence.Append(transform.DOMoveY(position.y + yPos, duration).SetEase(Ease.InOutSine));
            sequence.Append(transform.DOMoveY(position.y, duration).SetEase(Ease.InOutSine));
            sequence.SetLoops(-1, LoopType.Restart);
            return sequence;
        }

        public static Sequence Jump(RectTransform transform)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOAnchorPosY(12, 0.5f).SetEase(Ease.InOutQuart).SetRelative(true));
            sequence.Join(transform.DOScaleY(1.3f, 0.5f).SetEase(Ease.InOutSine));
            sequence.SetLoops(-1, LoopType.Yoyo);
            return sequence;
        }

        public class TweenChain
        {
            private readonly Queue<Sequence> _sequenceQueue = new();
            private readonly Action _onComplete;

            public TweenChain(Action onComplete)
            {
                _onComplete = onComplete;
            }

            public void AddAndPlay(Tween tween)
            {
                var sequence = DOTween.Sequence();
                sequence.SetUpdate(true);
                sequence.Pause();
                sequence.Append(tween);
                _sequenceQueue.Enqueue(sequence);
                if (_sequenceQueue.Count == 1)
                {
                    _sequenceQueue.Peek().Play();
                }

                sequence.OnComplete(OnComplete);
            }

            private void OnComplete()
            {
                _sequenceQueue.Dequeue();
                if (_sequenceQueue.Any())
                {
                    _sequenceQueue.Peek().Play();
                }
                else
                {
                    _onComplete?.Invoke();
                }
            }

            public bool IsRunning()
            {
                return _sequenceQueue.Any();
            }

            public void Complete()
            {
                while (_sequenceQueue.Any())
                {
                    var sequence = _sequenceQueue.Dequeue();
                    sequence.OnComplete(null);
                    sequence.Complete(true);
                }

                _onComplete?.Invoke();
            }

            public void Kill()
            {
                foreach (var sequence in _sequenceQueue)
                {
                    sequence.Kill();
                }

                _sequenceQueue.Clear();
            }
        }
    }
}