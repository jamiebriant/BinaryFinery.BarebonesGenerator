// 
// A modification of FileGenReflector: http://filegenreflector.codeplex.com/
// Copyright (c) 2008 (?) Jason R Bock
// Released under the Microsoft Public License:  http://filegenreflector.codeplex.com/license
// Modifications: Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using System;
using System.Windows.Forms;
using BinaryFinery.BareBonesGenerator.AddIn.UI;
using Reflector;

namespace BinaryFinery.BareBonesGenerator.AddIn
{
    public class BareBonesGeneratorPackage : IPackage
    {
        private const Keys MenuKeys = Keys.Control | Keys.Shift | Keys.G;
        private const string ToolsCommandBar = "Tools";
        private const string ToolsTitle = "&Generate Barebone File(s)...";
        private const string WindowID = "FileGeneratorWindow";
        private const string WindowTitle = "Generate Files";

        private ICommandBarButton fileGeneratorCommandBarButton;
        private ICommandBarSeparator fileGeneratorCommandBarSeparator;
        private IWindow fileGeneratorWindow;
        private IServiceProvider serviceProvider;
        private IWindowManager windowManager;

        #region IPackage Members

        public void Load(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;

            var fileGeneratorControl = new FileGeneratorControl(this.serviceProvider);

            windowManager =
                (IWindowManager) this.serviceProvider.GetService(typeof(IWindowManager));
            windowManager.Windows.Add(WindowID, fileGeneratorControl, WindowTitle);

            fileGeneratorWindow = windowManager.Windows[WindowID];
            fileGeneratorWindow.Content.Dock = DockStyle.Fill;

            var commandBarManager =
                (ICommandBarManager) this.serviceProvider.GetService(typeof(ICommandBarManager));
            var commandToolItems = commandBarManager.CommandBars[ToolsCommandBar].Items;
            fileGeneratorCommandBarSeparator = commandToolItems.AddSeparator();
            fileGeneratorCommandBarButton = commandToolItems.AddButton(
                ToolsTitle, new EventHandler(OnFileGeneratorButtonClick), MenuKeys);
        }

        public void Unload()
        {
            var commandBarManager =
                (ICommandBarManager) serviceProvider.GetService(typeof(ICommandBarManager));
            var commandToolItems = commandBarManager.CommandBars[ToolsCommandBar].Items;

            commandToolItems.Remove(fileGeneratorCommandBarButton);
            commandToolItems.Remove(fileGeneratorCommandBarSeparator);
        }

        #endregion

        private void OnFileGeneratorButtonClick(object sender, EventArgs e)
        {
            fileGeneratorWindow.Visible = true;
        }
    }
}