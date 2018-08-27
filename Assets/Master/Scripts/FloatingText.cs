// (c) Copyright Cleverous 2015. All rights reserved.

using UnityEngine;
using System.Collections;

namespace Deftly
{
    public class FloatingText : MonoBehaviour
    {
        public string Value = "123abc";
        public float Speed = 1f;
        public float Duration = 1f;

        [HideInInspector]
        public GUIText TextReference;
        [HideInInspector]
        public Vector3 StartPosition;
        [HideInInspector]
        public float TotalTime;

        private Vector3 _dir = Vector3.up;

        void Reset()
        {
            Value = "123abc";
            Speed = 1f;
            Duration = 1f;
        }
        void Start()
        {
            _dir *= Speed;
            StartCoroutine(Life());
            TextReference = GetComponent<GUIText>();
            if (TextReference == null)
            {
                Debug.LogError("No GUIText Component was found on the Floating Text Prefab! Add a <color=green>GUIText</color> (Not [UI] <color=red>Text</color>) component.");
                return;
            }
            TextReference.text = Value;
        }
        void Update()
        {
            TotalTime += Time.deltaTime;
            transform.position = Camera.main.WorldToViewportPoint(StartPosition + Vector3.up) + (_dir * TotalTime * Speed);
        }
        IEnumerator Life()
        {
            yield return new WaitForSeconds(Duration);
            //StaticUtil.DeSpawn(gameObject);
        }
    }
}