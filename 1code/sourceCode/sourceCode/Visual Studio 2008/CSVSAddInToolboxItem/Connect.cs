/***************************************** Module Header *****************************************\
* Module Name:  Connect.cs
* Project:      CSVSAddInToolboxItem
* Copyright (c) Microsoft Corporation.
* 
* The CSVSAddInToolboxItem project demostrates how to customize Toolbox items
* in DTE automation model or by Toolbox service.
*
* In this sample, a add-in command button named "Add Customized Toolbox Item" 
* will be registered in the Tool menu. By clicking the menu, sample code will
* do follow two things:
* 1. Use DTE automation model to add an item under CustomTab tab
* 2. Use VS Toolbox service to add an item under CustomTab tab
*
* Both items are HTML content, they can be used by dragging and dropping to a 
* web page designer.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* History:
* * 1/6/2009 5:09 AM Hongye Sun Created
\*************************************************************************************************/

#region Using directives
using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using Microsoft.VisualStudio.OLE.Interop;
using System.Drawing.Design;
using Microsoft.VisualStudio;
using System.IO;
#endregion

namespace CSVSAddInToolboxItem
{
    /// <summary>The object for implementing an Add-in.</summary>
    /// <seealso class='IDTExtensibility2' />
    public class Connect : IDTExtensibility2, IDTCommandTarget
    {
        /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
        public Connect()
        {
        }

        /// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
        /// <param term='application'>Root object of the host application.</param>
        /// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
        /// <param term='addInInst'>Object representing this Add-in.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {
            _applicationObject = (DTE2)application;
            _addInInstance = (AddIn)addInInst;
            if (connectMode == ext_ConnectMode.ext_cm_UISetup)
            {
                object[] contextGUIDS = new object[] { };
                Commands2 commands = (Commands2)_applicationObject.Commands;
                Command command = null;

                try
                {
                    command = commands.AddNamedCommand2(
                        _addInInstance,
                        "CSVSAddInToolboxItem",
                        "CSVSAddInToolboxItem",
                        "Add item into VS Toolbox",
                        true,
                        6743,
                        ref contextGUIDS,
                        (int)vsCommandStatus.vsCommandStatusSupported
                        + (int)vsCommandStatus.vsCommandStatusEnabled,
                        (int)vsCommandStyle.vsCommandStylePictAndText,
                        vsCommandControlType.vsCommandControlTypeButton);
                }
                catch (System.ArgumentException)
                {
                    //If we are here, then the exception is probably because a command with that name
                    //  already exists. If so there is no need to recreate the command and we can 
                    //  safely ignore the exception.
                }

                AddCommandToToolMenubar(_applicationObject, command);
            }
        }

        /// <summary> 
        /// </summary>
        /// <param name="applicationObject"></param>
        /// <param name="command"></param>
        private void AddCommandToToolMenubar(DTE2 applicationObject, Command command)
        {
            string toolsMenuName = "Tools";

            // Place the command on the tools menu.
            // Find the MenuBar command bar, which is the top-level command bar holding all the 
            // main menu items:
            CommandBar menuBarCommandBar =
                ((CommandBars)applicationObject.CommandBars)["MenuBar"];

            //Find the Tools command bar on the MenuBar command bar:
            CommandBarControl toolsControl = menuBarCommandBar.Controls[toolsMenuName];
            CommandBarPopup toolsPopup = (CommandBarPopup)toolsControl;

            //Add a control for the command to the tools menu:
            if ((command != null) && (toolsPopup != null))
            {
                CommandBarControl commandBarControl =
                    (CommandBarControl)command.AddControl(toolsPopup.CommandBar, 1);
                commandBarControl.Caption = "Add Customized Toolbox Item";
            }
        }

        /// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
        /// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
        }

        /// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />		
        public void OnAddInsUpdate(ref Array custom)
        {
        }

        /// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnStartupComplete(ref Array custom)
        {
        }

        /// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnBeginShutdown(ref Array custom)
        {
        }

        /// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
        /// <param term='commandName'>The name of the command to determine state for.</param>
        /// <param term='neededText'>Text that is needed for the command.</param>
        /// <param term='status'>The state of the command in the user interface.</param>
        /// <param term='commandText'>Text requested by the neededText parameter.</param>
        /// <seealso class='Exec' />
        public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
        {
            if (neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
            {
                if (commandName == "CSVSAddInToolboxItem.Connect.CSVSAddInToolboxItem")
                {
                    status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported |
                        vsCommandStatus.vsCommandStatusEnabled;
                    return;
                }
            }
        }

        /// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
        /// <param term='commandName'>The name of the command to execute.</param>
        /// <param term='executeOption'>Describes how the command should be run.</param>
        /// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
        /// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
        /// <param term='handled'>Informs the caller if the command was handled or not.</param>
        /// <seealso class='Exec' />
        public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
        {
            handled = false;
            if (executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
            {
                if (commandName == "CSVSAddInToolboxItem.Connect.CSVSAddInToolboxItem")
                {
                    handled = true;

                    // The first sample demostrate how to add item by DTE automation model
                    AddItemByDTE();

                    // The second sample demostrat how to add item by Toolbox service
                    AddItemByVsToolboxService();
                    return;
                }
            }
        }

        /// <summary>
        /// This method adds a Toolbox item by DTE automation model
        /// </summary>
        private void AddItemByDTE()
        {
            // Get tabs from automation model
            ToolBoxTabs tabs = _applicationObject.ToolWindows.ToolBox.ToolBoxTabs;
            IEnumerator e = tabs.GetEnumerator();

            ToolBoxTab tab = null;
            while (e.MoveNext())
            {
                ToolBoxTab ct = e.Current as ToolBoxTab;
                if (ct.Name == "CustomTab")
                {
                    tab = ct;
                    break;
                }
            }

            // If there is no CustomTab, add one
            if (tab == null)
                tab = tabs.Add("CustomTab");

            // Add Toolbox Item, but we can't customize other information for 
            // Toolbox item like icon and transparency.
            tab.ToolBoxItems.Add(
                "DTE Added HTML Content", 
                "<input id=\"Button1\" type=\"button\" value=\"button\" />", 
                vsToolBoxItemFormat.vsToolBoxItemFormatHTML);
        }

        /// <summary>
        /// This method adds a Toolbox item by VS Toolbox service.
        /// This way provides more flexibilities than DTE way.
        /// </summary>
        private void AddItemByVsToolboxService()
        {
            // Get shell service provider.
            ServiceProvider sp =
                new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)_applicationObject);

            // Get the IVsToolbox interface.
            IVsToolbox tbs = sp.GetService(typeof(SVsToolbox)) as IVsToolbox;

            // Toolbox Item Info data 
            TBXITEMINFO[] itemInfo = new TBXITEMINFO[1];
            Bitmap bitmap =
                new Bitmap(this.GetType().Assembly.GetManifestResourceStream("CSVSAddInToolboxItem.Demo.bmp"));
            itemInfo[0].bstrText = "Service Added HTML Content";
            itemInfo[0].hBmp = bitmap.GetHbitmap();
            itemInfo[0].dwFlags = (uint)__TBXITEMINFOFLAGS.TBXIF_DONTPERSIST;

            // OleDataObject to host toolbox data
            OleDataObject tbItem = new OleDataObject();
            tbItem.SetText(
                ConvertToClipboardFormat("<input id=\"Button1\" type=\"button\" value=\"button\" />", null, null),
                TextDataFormat.Html);

            // Add a new toolbox item to MyCustomTab tab
            tbs.AddItem(tbItem, itemInfo, "CustomTab");
        }


        #region ConvertToClipboardFormat method (Copied from http://blogs.msdn.com/jmstall/pages/sample-code-html-clipboard.aspx)
        /// <summary>
        /// Clears clipboard and copy a HTML fragment to the clipboard, providing additional meta-information.
        /// </summary>
        /// <param name="htmlFragment">a html fragment</param>
        /// <param name="title">optional title of the HTML document (can be null)</param>
        /// <param name="sourceUrl">optional Source URL of the HTML document, for resolving relative links (can be null)</param>
        public static string ConvertToClipboardFormat(string htmlFragment, string title, Uri sourceUrl)
        {
            if (title == null) title = "From Clipboard";

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            // Builds the CF_HTML header. See format specification here:
            // http://msdn.microsoft.com/library/default.asp?url=/workshop/networking/clipboard/htmlclipboard.asp

            // The string contains index references to other spots in the string, so we need placeholders so we can compute the offsets. 
            // The <<<<<<<_ strings are just placeholders. We'll backpatch them actual values afterwards.
            // The string layout (<<<) also ensures that it can't appear in the body of the html because the <
            // character must be escaped.
            string header =
@"Format:HTML Format
Version:1.0
StartHTML:<<<<<<<1
EndHTML:<<<<<<<2
StartFragment:<<<<<<<3
EndFragment:<<<<<<<4
StartSelection:<<<<<<<3
EndSelection:<<<<<<<4
";

            string pre =
    @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">
<HTML><HEAD><TITLE>" + title + @"</TITLE></HEAD><BODY><!--StartFragment-->";

            string post = @"<!--EndFragment--></BODY></HTML>";

            sb.Append(header);
            if (sourceUrl != null)
            {
                sb.AppendFormat("SourceURL:{0}", sourceUrl);
            }
            int startHTML = sb.Length;

            sb.Append(pre);
            int fragmentStart = sb.Length;

            sb.Append(htmlFragment);
            int fragmentEnd = sb.Length;

            sb.Append(post);
            int endHTML = sb.Length;

            // Backpatch offsets
            sb.Replace("<<<<<<<1", To8DigitString(startHTML));
            sb.Replace("<<<<<<<2", To8DigitString(endHTML));
            sb.Replace("<<<<<<<3", To8DigitString(fragmentStart));
            sb.Replace("<<<<<<<4", To8DigitString(fragmentEnd));

            return sb.ToString();
        }

        // Helper to convert an integer into an 8 digit string.
        // String must be 8 characters, because it will be used to replace an 8 character string within a larger string.    
        static string To8DigitString(int x)
        {
            return String.Format("{0,8}", x);
        }
        #endregion

        private DTE2 _applicationObject;
        private AddIn _addInInstance;
    }
}