using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VR.Test.Keyboard
{
    /// <summary>
    /// A keyboard key containing basic functionality. Highlight, pressing, registering in the keyboard.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class KeyboardButtonBase : MonoBehaviour
    {
        private Button _btn;

        [SerializeField]
        protected private Keyboard _keyboard;

        [SerializeField]
        private Vector2Int _positionOnKeyboard;
        public Vector2Int PositionOnKeyboard { get => _positionOnKeyboard; set => _positionOnKeyboard = value; }

        #region private methods

        private void Awake()
        {
            _btn = GetComponent<Button>();
        }

        private void OnEnable()
        {
            Subscribe();
            RegisterButton();
        }
        
        public void RegisterButton()
        {
            _keyboard.AddButton(this);
        }

        private void Subscribe() => _btn.onClick.AddListener(OnClick);

        private void UnSubscribe() => _btn.onClick.RemoveListener(OnClick);

        private void OnDisable() => UnSubscribe();

        #endregion private methods

        #region public methods

        public virtual void OnClick() { }

        public virtual void ToLower() { }

        public virtual void ToUpper() { }
        
        public void OnSelected()
        {
            EventSystem.current.SetSelectedGameObject(_btn.gameObject, new BaseEventData(EventSystem.current));
        }

        public void Unselected()
        {
            
        }
        
        #endregion public methods
    }
}
