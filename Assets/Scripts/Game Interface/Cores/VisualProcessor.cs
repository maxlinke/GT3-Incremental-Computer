using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cores {

    public class VisualProcessor : MonoBehaviour {

        [SerializeField, RedIfEmpty] Text m_text;

        public ID id { get; private set; }  // TODO needs to go in state. maybe make this visualprocessor like visualtask

        void OnEnable () {
            this.id = ID.GetNext();
        }

        void OnDisable () {
            ID.ReturnId(this.id);
            this.id = null;
        }

        void Start () {
            
        }

        void Update () {
            
        }

    }

}