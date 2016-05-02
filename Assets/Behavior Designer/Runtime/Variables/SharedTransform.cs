using UnityEngine;
using System.Collections;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedTransform : SharedVariable
    {
        public Transform Value { get { return mValue; } set { mValue = value; } }
        [SerializeField]
        private Transform mValue;

        public override object GetValue() { return mValue; }
        public override void SetValue(object value) { mValue = (Transform)value; }

        public override string ToString() { return (mValue == null ? "null" : mValue.name); }
        public static implicit operator SharedTransform(Transform value) { var sharedVariable = new SharedTransform(); sharedVariable.SetValue(value); return sharedVariable; }
    }
}