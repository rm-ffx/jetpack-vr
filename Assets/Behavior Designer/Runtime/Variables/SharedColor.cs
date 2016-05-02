using UnityEngine;
using System.Collections;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedColor : SharedVariable
    {
        public Color Value { get { return mValue; } set { mValue = value; } }
        [SerializeField]
        private Color mValue;

        public override object GetValue() { return mValue; }
        public override void SetValue(object value) { mValue = (Color)value; }

        public override string ToString() { return mValue.ToString(); }
        public static implicit operator SharedColor(Color value) { var sharedVariable = new SharedColor(); sharedVariable.SetValue(value); return sharedVariable; }
    }
}