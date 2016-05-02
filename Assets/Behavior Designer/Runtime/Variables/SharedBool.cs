using UnityEngine;
using System.Collections;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedBool : SharedVariable
    {
        public bool Value { get { return mValue; } set { mValue = value; } }
        [SerializeField]
        private bool mValue;

        public override object GetValue() { return mValue; }
        public override void SetValue(object value) { mValue = (bool)value; }

        public override string ToString() { return mValue.ToString(); }
        public static implicit operator SharedBool(bool value) { var sharedVariable = new SharedBool(); sharedVariable.SetValue(value); return sharedVariable; }
    }
}