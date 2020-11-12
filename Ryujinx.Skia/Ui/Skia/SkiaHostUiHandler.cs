using System.Threading;
using Ryujinx.HLE;
using Ryujinx.HLE.HOS.Applets;
using Ryujinx.HLE.HOS.Services.Am.AppletOE.ApplicationProxyService.ApplicationProxy.Types;
using Ryujinx.Skia.Ui.Skia.Scene;

namespace Ryujinx.Skia.Ui
{
    public class SkiaHostUiHandler : IHostUiHandler
    {
        public GameScene Scene { get; }

        public SkiaHostUiHandler(GameScene scene)
        {
            Scene = scene;
        }

        public bool DisplayMessageDialog(ControllerAppletUiArgs args)
        {
            string playerCount = args.PlayerCountMin == args.PlayerCountMax
                ? $"exactly {args.PlayerCountMin}"
                : $"{args.PlayerCountMin}-{args.PlayerCountMax}";

            string message =
                $"Application requests <b>{playerCount}</b> player(s) with:\n\n"
                + $"<tt><b>TYPES:</b> {args.SupportedStyles}</tt>\n\n"
                + $"<tt><b>PLAYERS:</b> {string.Join(", ", args.SupportedPlayers)}</tt>\n\n"
                + (args.IsDocked ? "Docked mode set. <tt>Handheld</tt> is also invalid.\n\n" : "")
                + "<i>Please reconfigure Input now and then press OK.</i>";

            return DisplayMessageDialog("Controller Applet", message);
        }

        public bool DisplayMessageDialog(string title, string message)
        {
            return true;
        }

        public bool DisplayInputDialog(SoftwareKeyboardUiArgs args, out string userText)
        {
            // TODO: Implement input dilog
            userText = "Ryujinx";
            return true;
        }

        public void ExecuteProgram(HLE.Switch device, ProgramSpecifyKind kind, ulong value)
        {
            device.UserChannelPersistence.ExecuteProgram(kind, value);
            Scene?.Exit();
        }

        public bool DisplayErrorAppletDialog(string title, string message, string[] buttons)
        {
            ManualResetEvent dialogCloseEvent = new ManualResetEvent(false);
            bool showDetails = false;

            /*Application.Invoke(delegate
            {
                try
                {
                    ErrorAppletDialog msgDialog = new ErrorAppletDialog(_parent, DialogFlags.DestroyWithParent, MessageType.Error, buttons)
                    {
                        Title = title,
                        Text = message,
                        UseMarkup = true,
                        WindowPosition = WindowPosition.CenterAlways
                    };

                    msgDialog.SetDefaultSize(400, 0);

                    msgDialog.Response += (object o, ResponseArgs args) =>
                    {
                        if (buttons != null)
                        {
                            if (buttons.Length > 1)
                            {
                                if (args.ResponseId != (ResponseType)(buttons.Length - 1))
                                {
                                    showDetails = true;
                                }
                            }
                        }

                        dialogCloseEvent.Set();
                        msgDialog?.Dispose();
                    };

                    msgDialog.Show();
                }
                catch (Exception e)
                {
                    Logger.Error?.Print(LogClass.Application, $"Error displaying ErrorApplet Dialog: {e}");

                    dialogCloseEvent.Set();
                }
            });*/

            dialogCloseEvent.WaitOne();

            return showDetails;
        }
    }
}