using UI;
using UnityEngine;

namespace FX
{
    public class WingHoverEffect : MonoBehaviour
    {
        [Header("Animation Settings")]
        public float animationDuration;
        public Vector2 animationFinalOffset;
        public float animationFinalRotation;
        public AnimationCurve animationCurve;
        public float animationOffset;
        
        private Vector2 _animationInitialOffset;
        private float _animationInitialRotation;        
        void Start()
        {
            animationCurve.postWrapMode = WrapMode.PingPong;
            _animationInitialOffset = transform.localPosition;
            _animationInitialRotation = transform.localRotation.eulerAngles.z;
        }
        void Update()
        {
            var t = animationCurve.Evaluate((Time.time + animationOffset) / animationDuration);
            transform.localPosition = Vector2.Lerp(_animationInitialOffset, animationFinalOffset + _animationInitialOffset, t);
            transform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(_animationInitialRotation, animationFinalRotation + _animationInitialRotation, t));
        }
    }
}
