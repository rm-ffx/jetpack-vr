using UnityEngine;
using System.Collections;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedRect : SharedVariable
    {
        public Rect Value { get { return mValue; } set { mValue = value; } }
        [SerializeField]
        private Rect mValue;

        public override object GetValue() { return mValue; }
        public override void SetValue(object value) { mValue = (Rect)value; }

        public override string ToString() { return mValue.ToString(); }
        public static implicit operator SharedRect(Rect value) { var sharedVariable = new SharedRect(); sharedVariable.SetValue(value); return sharedVariable; }
    }
}