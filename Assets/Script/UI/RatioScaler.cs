using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

namespace Std.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class RatioScaler : MonoBehaviour
    {
        enum PresetPivot
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
        };

        private RectTransform rect;

        [SerializeField] private float widthRatio = 0.1f;
        [SerializeField] private float heightRatio = 0.1f;

        [SerializeField] private PresetPivot pivot = PresetPivot.TopLeft;
        [SerializeField] private Vector2 position = Vector2.zero;

        private void Update()
        {
            var height = Screen.height * heightRatio;
            var width = Screen.width * widthRatio;

            rect.sizeDelta = new Vector2(width, height);
        }

        private void OnEnable()
        {
            TryGetComponent(out rect);
        }
    }
}