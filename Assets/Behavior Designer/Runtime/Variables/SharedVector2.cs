using UnityEngine;
using System.Collections;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedVector2 : SharedVariable
    {
        public Vector2 Value { get { return mValue; } set { mValue = value; } }
        [SerializeField]
        private Vector2 mValue;

        public override object GetValue() { return mValue; }
        public override void SetValue(object value) { mValue = (Vector2)value; }

        public override string ToString() { return mValue.ToString(); }
        public static implicit operator SharedVector2(Vector2 value) { var sharedVariable = new SharedVector2(); sharedVariable.SetValue(value); return sharedVariable; }
    }
}