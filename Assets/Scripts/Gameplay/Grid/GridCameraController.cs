using UnityEngine;
using UnityEngine.InputSystem;

namespace Spotlight.Gameplay.Grid
{
    /// <summary>
    /// 2D 网格相机控制器：正交相机 + 左键拖动平移 + 滚轮缩放 + 地图边界限制。
    /// 与 <see cref="GridDeveloperMode"/> 共存：点击=涂格子，拖动=平移相机。
    /// </summary>
    public sealed class GridCameraController : MonoBehaviour
    {
        [Header("相机")]
        [SerializeField] private Camera targetCamera;
        [SerializeField] private float minOrthographicSize = 2f;
        [SerializeField] private float maxOrthographicSize = 50f;

        [Header("拖拽")]
        [SerializeField] private float dragThresholdPixels = 5f;

        [Header("边界（可引用 Grid2D 自动计算，或手动设置 bounds）")]
        [SerializeField] private Grid2D grid;
        [SerializeField] private Vector2 boundsMin;
        [SerializeField] private Vector2 boundsMax;

        private bool _manualBounds;
        private bool _isPanning;
        private Vector2 _panStartMouse;

        private void Start()
        {
            if (targetCamera == null)
            {
                targetCamera = GetComponent<Camera>();
            }

            _manualBounds = boundsMin != boundsMax;
        }

        private void Update()
        {
            if (targetCamera == null || Mouse.current == null)
            {
                return;
            }

            HandlePan();
            HandleZoom();
            ClampToBounds();
        }

        public void SetBounds(Vector2 min, Vector2 max)
        {
            boundsMin = min;
            boundsMax = max;
            _manualBounds = true;
        }

        public void ClearBounds()
        {
            _manualBounds = false;
        }

        private void HandlePan()
        {
            Mouse mouse = Mouse.current;

            if (mouse.leftButton.wasPressedThisFrame)
            {
                _isPanning = false;
                _panStartMouse = mouse.position.ReadValue();
            }
            else if (mouse.leftButton.isPressed)
            {
                Vector2 current = mouse.position.ReadValue();
                if (!_isPanning && Vector2.Distance(current, _panStartMouse) > dragThresholdPixels)
                {
                    _isPanning = true;
                }

                if (_isPanning)
                {
                    float unitsPerPixel = (targetCamera.orthographicSize * 2f) / Screen.height;
                    Vector2 deltaPixels = mouse.delta.ReadValue();
                    Vector3 delta = new Vector3(deltaPixels.x * unitsPerPixel, deltaPixels.y * unitsPerPixel, 0f);

                    // 拖动地图：鼠标往哪移，相机往反方向移，内容跟随光标。
                    targetCamera.transform.position -= delta;
                }
            }
            else if (mouse.leftButton.wasReleasedThisFrame)
            {
                _isPanning = false;
            }
        }

        private void HandleZoom()
        {
            float scroll = Mouse.current.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) < 0.0001f)
            {
                return;
            }

            float size = targetCamera.orthographicSize * Mathf.Pow(0.9f, scroll);
            targetCamera.orthographicSize = Mathf.Clamp(size, minOrthographicSize, maxOrthographicSize);
        }

        private void ClampToBounds()
        {
            Vector2 min;
            Vector2 max;

            if (grid != null)
            {
                Vector3 origin = grid.transform.position;
                min = new Vector2(origin.x, origin.y);
                max = new Vector2(origin.x + grid.Width * grid.CellSize, origin.y + grid.Height * grid.CellSize);
            }
            else if (_manualBounds)
            {
                min = boundsMin;
                max = boundsMax;
            }
            else
            {
                return;
            }

            float halfHeight = targetCamera.orthographicSize;
            float halfWidth = halfHeight * targetCamera.aspect;

            Vector3 p = targetCamera.transform.position;

            p.x = (max.x - min.x) > halfWidth * 2f
                ? Mathf.Clamp(p.x, min.x + halfWidth, max.x - halfWidth)
                : (min.x + max.x) * 0.5f;
            p.y = (max.y - min.y) > halfHeight * 2f
                ? Mathf.Clamp(p.y, min.y + halfHeight, max.y - halfHeight)
                : (min.y + max.y) * 0.5f;

            p.z = targetCamera.transform.position.z;
            targetCamera.transform.position = p;
        }
    }
}
