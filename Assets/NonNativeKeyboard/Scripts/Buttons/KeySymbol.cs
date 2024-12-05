using TMPro;
using UnityEngine;

/// <summary>
/// Symbol button
/// </summary>

namespace VR.Test.Keyboard
{
    public class KeySymbol : KeyboardButtonBase
    {
        [SerializeField]
        private TMP_Text _tMP_Text;

        [SerializeField]
        private string _symbol;

        #region public methods
        public override void OnClick()
        {
            _keyboard.AddSymbol(_symbol);
        }

        public override void ToUpper()
        {
            _tMP_Text.fontStyle = FontStyles.UpperCase;
        }

        public override void ToLower()
        {
            _tMP_Text.fontStyle = FontStyles.LowerCase;
        }
        #endregion public methods
    }
}
