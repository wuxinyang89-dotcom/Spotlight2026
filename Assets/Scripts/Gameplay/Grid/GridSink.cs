using UnityEngine;

namespace Spotlight.Gameplay.Grid
{
    /// <summary>
    /// 沉降机关组件：挂在 Sink 方块上，检测是否有足够重的物体压在上面。
    /// 重物压下时 Pressed 为 true（供机关效果读取）。
    /// </summary>
    public sealed class GridSink : MonoBehaviour
    {
        [Header("触发所需质量（Melancholy 增重后可超过）")]
        [SerializeField, Min(0.1f)] private float triggerMass = 3f;

        private bool _pressed;

        /// <summary>当前是否被重物压下。</summary>
        public bool Pressed => _pressed;

        private void OnTriggerStay2D(Collider2D other)
        {
            Rigidbody2D body = other.attachedRigidbody;
            if (body != null && body.mass >= triggerMass)
            {
                _pressed = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Rigidbody2D body = other.attachedRigidbody;
            if (body != null && body.mass >= triggerMass)
            {
                _pressed = false;
            }
        }
    }
}
