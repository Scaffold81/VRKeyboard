/// <summary>
/// Eneter button
/// </summary>

namespace VR.Test.Keyboard
{
    public class KeyEnter : KeyboardButtonBase
    {
        #region public methods

        public override void OnClick()
        {
            _keyboard.Enter();
        }

        #endregion public methods
    }
}
