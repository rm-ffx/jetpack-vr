using UnityEngine;
using System.Collections;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedQuaternion : SharedVariable
    {
        public Quaternion Value { get { return mValue; } set { mValue = value; } }
        [SerializeField]
        private Quaternion mValue;

        public override object GetValue() { return mValue; }
        public override void SetValue(object value) { mValue = (Quaternion)value; }

        public override string ToString() { return mValue.ToString(); }
        public static implicit operator SharedQuaternion(Quaternion value) { var sharedVariable = new SharedQuaternion(); sharedVariable.SetValue(value); return sharedVariable; }
    }
}