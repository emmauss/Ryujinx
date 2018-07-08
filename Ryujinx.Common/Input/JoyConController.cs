namespace Ryujinx.Common.Input
{
    public struct JoyConControllerLeft
    {
        public GamePadStick  Stick;
        public GamePadButton StickButton;
        public GamePadButton DPadUp;
        public GamePadButton DPadDown;
        public GamePadButton DPadLeft;
        public GamePadButton DPadRight;
        public GamePadButton ButtonMinus;
        public GamePadButton ButtonL;
        public GamePadButton ButtonZL;
    }

    public struct JoyConControllerRight
    {
        public GamePadStick  Stick;
        public GamePadButton StickButton;
        public GamePadButton ButtonA;
        public GamePadButton ButtonB;
        public GamePadButton ButtonX;
        public GamePadButton ButtonY;
        public GamePadButton ButtonPlus;
        public GamePadButton ButtonR;
        public GamePadButton ButtonZR;
    }

    public struct JoyConController
    {
        public JoyConControllerLeft Left;
        public JoyConControllerRight Right;
    }
}
