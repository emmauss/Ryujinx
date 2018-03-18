using System;
using Ryujinx;
using Ryujinx.Core;
using Ryujinx.Graphics.Gal;
using Eto.Forms;
using Eto.Drawing;
using OpenTK.Graphics;
using Eto;

namespace Ryujinx.EUI
{
	public partial class MainForm : Form
	{
        OpenTK.GLWidget gLWidget;

        private Switch Ns;

        private IGalRenderer Renderer;

        public MainForm()
		{
            Title = "My Eto Form";

            var LoadGameCommand = new Command { MenuText = "Load Game", ToolBarText = "Load Game" };
            LoadGameCommand.Executed += LoadGameCommand_Executed;

            var quitCommand = new Command { MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };
            quitCommand.Executed += (sender, e) => Application.Instance.Quit();

            // create menu
            Menu = new MenuBar
            {
                Items =
                {
					// File submenu
					new ButtonMenuItem { Text = "&File", Items = {
                            LoadGameCommand,
                            quitCommand
                        }
                    },
					// new ButtonMenuItem { Text = "&Edit", Items = { /* commands/items */ } },
					// new ButtonMenuItem { Text = "&View", Items = { /* commands/items */ } },
				},
			};

            Content = new DynamicLayout {
                Size = new Size(1280, 720)

            };

            gLWidget = new OpenTK.GLWidget(GraphicsMode.Default, 3, 3,GraphicsContextFlags.ForwardCompatible);
            ((DynamicLayout)Content).Add(gLWidget.ToEto());

            Renderer = new Graphics.Gal.OpenGL.OpenGLRenderer();
            gLWidget.Initialized += GLWidget_Initialized;

		}

        private void GLWidget_Initialized(object sender, EventArgs e)
        {
            Renderer.InitializeFrameBuffer();
        }

        private void LoadGameCommand_Executed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
