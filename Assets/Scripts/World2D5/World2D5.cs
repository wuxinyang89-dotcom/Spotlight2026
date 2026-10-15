using UnityEngine;

namespace Spotlight.World2D5
{
    /// <summary>
    /// 2.5D 世界主逻辑：正交相机正对 XY 平面（地图与 X/Y 轴平齐，非倾斜透视）。
    /// 世界坐标约定：X 向右、Y 向上为平面，Z 为深度（越小越靠近相机，用于前后排序）。
    /// 提供世界坐标 → 屏幕归一化坐标（0..1）转换，供对话人物映射等使用。
    /// </summary>
    public sealed class World2D5 : MonoBehaviour
    {
        [Header("相机")]
        [SerializeField] private Camera worldCamera;
        [SerializeField] private float orthographicSize = 5f;

        private static World2D5 _instance;

        /// <summary>全局实例（未创建时为 null）。</summary>
        public static World2D5 Instance => _instance;

        /// <summary>世界相机。</summary>
        public Camera WorldCamera => worldCamera;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            if (worldCamera == null)
            {
                worldCamera = Camera.main;
            }

            ApplyCamera();
        }

        private void ApplyCamera()
        {
            if (worldCamera == null)
            {
                return;
            }

            worldCamera.orthographic = true;
            worldCamera.orthographicSize = orthographicSize;
            worldCamera.transform.rotation = Quaternion.identity;
            worldCamera.transform.position = new Vector3(0f, 0f, -10f);
        }

        /// <summary>世界坐标 → 屏幕归一化坐标（0..1）。</summary>
        public Vector2 WorldToScreen01(Vector3 worldPosition)
        {
            if (worldCamera == null)
            {
                return new Vector2(0.5f, 0.5f);
            }

            Vector3 viewport = worldCamera.WorldToViewportPoint(worldPosition);
            return new Vector2(viewport.x, viewport.y);
        }
    }
}
