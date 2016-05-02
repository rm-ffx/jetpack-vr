using UnityEngine;
using System.Collections;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedVector3 : SharedVariable
    {
        public Vector3 Value { get { return mValue; } set { mValue = value; } }
        [SerializeField]
        private Vector3 mValue;

        public override object GetValue() { return mValue; }
        public override void SetValue(object value) { mValue = (Vector3)value; }

        public override string ToString() { return mValue.ToString(); }
        public static implicit operator SharedVector3(Vector3 value) { var sharedVariable = new SharedVector3(); sharedVariable.SetValue(value); return sharedVariable; }
    }
}