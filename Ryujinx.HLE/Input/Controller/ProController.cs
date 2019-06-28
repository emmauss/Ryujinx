namespace Ryujinx.HLE.Input
{
    public class ProController : BaseController
    {
        private bool _wired = false;

        private NpadColor _bodyColor;
        private NpadColor _buttonColor;

        public ProController(Switch    device,
                             NpadColor bodyColor,
                             NpadColor buttonColor) : base(ControllerStatus.ProController, device)
        {
            _wired = true;

            _bodyColor   = bodyColor;
            _buttonColor = buttonColor;
        }

        public override void Connect(ControllerId controllerId)
        {
            ControllerColorDesc singleColorDesc =
                ControllerColorDesc.ColorDescColorsNonexistent;

            ControllerColorDesc splitColorDesc = 0;

            ConnectionState = ControllerConnState.ControllerStateConnected | ControllerConnState.ControllerStateWired;

            Initialize(false,
                (0, 0),
                (0, 0),
                singleColorDesc,
                splitColorDesc,
                _bodyColor,
                _buttonColor);

            base.Connect(controllerId);

            SetLayout(ControllerLayouts.ProController);
        }
    }
}
