using UnityEngine;
using UnityEngine.UI;

namespace GameInterfaceElements.Header {

    public class SystemClock : MonoBehaviour {

        [SerializeField, RedIfEmpty] Text m_textField;

        public void Initialize () {
            // ??? 
        }

        void Update () {
            m_textField.text = System.DateTime.Now.ToString();
        }

    }

}