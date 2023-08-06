using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Wave
{
    internal class WPPSin : WavePointProvider
    {
        public float Height = 1;
        public float Width  = 1;
        public float Center  = 0;
        public float LeftRange = 3000;
        public float RightRange = 100;
        public float MoveSpeed = 0;
        public bool isTemp = false;
        public override float GetY(float x, float width)
        {
            var a = x - Center;
            return Height * Mathf.Sin(a * Width);
        }

        public override bool HasPoint(float x, float width)
        {
            if(isTemp && Mathf.Abs(width - x) < 0.01f && DrawCount == 0)
            {
                Destroy(this);
                return false;
            }
            var o = (x - Center) * Width;
            var left =  - LeftRange * Mathf.PI;
            var right = RightRange * Mathf.PI;
            return left <= o && right >= o;
        }

        private void Update()
        {
            Center += MoveSpeed * Time.deltaTime;
        }

        private void OnEnable()
        {
            GetComponent<VoidWaveMesh>().points.Add(this);
        }
        private void OnDisable()
        {
            GetComponent<VoidWaveMesh>().points.Remove(this);
        }
    }
}
