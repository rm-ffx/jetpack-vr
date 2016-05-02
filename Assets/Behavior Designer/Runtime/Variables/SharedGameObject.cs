using UnityEngine;
using System.Collections;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedGameObject : SharedVariable
    {
        public GameObject Value { get { return mValue; } set { mValue = value; } }
        [SerializeField]
        private GameObject mValue;

        public override object GetValue() { return mValue; }
        public override void SetValue(object value) { mValue = (GameObject)value; }

        public override string ToString() { return (mValue == null ? "null" : mValue.name); }
        public static implicit operator SharedGameObject(GameObject value) { var sharedVariable = new SharedGameObject(); sharedVariable.SetValue(value); return sharedVariable; }
    }
}