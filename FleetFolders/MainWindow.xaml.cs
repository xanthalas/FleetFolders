/* Copyright (c) 2010 xanthalas.co.uk
 * 
 * Author: Xanthalas
 * Date  : October 2010
 * 
 *  This file is part of FleetFolders.
 *
 *  FleetFolders is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  FleetFolders is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with FleetFolders.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;

namespace FleetFolders
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private const string STORAGEFILE = "folders.json";
		private const string COMMAND_KEYS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private const string FILTER_KEYS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		private const string NUMERIC_KEYS = @"D0D1D2D3D4D5D6D7D8D9D0123456789.,\/";
		
		private FleetFoldersIO io;

		/// <summary>
		/// Indicates whether the program will minimize itself when the user opens a folder.
		/// </summary>
		private bool _minimizeOnSelection = true;

        /// <summary>
        /// Holds the icon which will appear in the System Tray
        /// </summary>
        private System.Windows.Forms.NotifyIcon systemTrayIcon;

        /// <summary>
        /// Holds the current window state
        /// </summary>
        private WindowState storedWindowState = WindowState.Normal;

        /// <summary>
        /// Ensures we only show the tooltip during minimising on the first time through
        /// </summary>
	    private bool isMinimizeTooltipAlreadyShown = false;

		/// <summary>
		/// Main constructor for this class.
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();
			
			io = new FleetFoldersIO(STORAGEFILE);
			
			FoldersList.ItemsSource = io.FilteredFolders;
			
			FilterText.BorderBrush = FilterText.Background;

            systemTrayIcon = new System.Windows.Forms.NotifyIcon();
            systemTrayIcon.BalloonTipText = "FleetFolders has been minimised. Click the tray icon to open.";
            systemTrayIcon.BalloonTipTitle = "FleetFolders";
            systemTrayIcon.Text = "FleetFolders";
            systemTrayIcon.Icon = new System.Drawing.Icon("Folder.ico");
            systemTrayIcon.Click += new EventHandler(m_notifyIcon_Click);
        }
		
		
		/// <summary>
		/// Open the folder passed in using Windows Explorer.
		/// </summary>
		/// <param name="folder">Full path to the folder to be opened</param>
		private void openFolder(FleetFolder folder)
		{
            if (Directory.Exists(folder.Url))
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo("explorer.exe");
                    psi.Arguments += folder.Url;
                    psi.UseShellExecute = false;
                    Process.Start(psi);
                    folder.LastAccessed = DateTime.Now;
                    folder.UsageCount++;

                    io.SaveFolders(STORAGEFILE);

                    if (_minimizeOnSelection)
                    {
                        this.WindowState = WindowState.Minimized;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("FleetFolders Error", "Cannot start explorer: " + e.Message, MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
		}
		
		
		private void recalculateColumnWidths(double width)
		{
			if (Double.IsNaN(FolderColumn.ActualWidth) ||  Double.IsNaN(KeyColumn.ActualWidth) || Double.IsNaN(CountColumn.ActualWidth) || Double.IsNaN(LastAccessedColumn.ActualWidth))
			{
				//do nothing
			}
			else
			{
				double widthOfOtherColumns = KeyColumn.ActualWidth + CountColumn.ActualWidth + LastAccessedColumn.ActualWidth;
				
				double newWidth = width - widthOfOtherColumns - 30;
				
				if (newWidth > 0 && widthOfOtherColumns > 0)
				{
					FolderColumn.Width = newWidth;
				}
			}
		}
		
		#region Event Handlers
		
		/// <summary>
		/// Handles window resize to ensure optimal column display widths
		/// </summary>
		/// <param name="sender">Standard sender object</param>
		/// <param name="e">Size information</param>
		void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			recalculateColumnWidths(e.NewSize.Width);
		}		
		
		/// <summary>
		/// Fires when the program is about to close. Saves the current folder list for next time.
		/// </summary>
		/// <param name="e">Standard cancel argument</param>
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			io.SaveFolders(STORAGEFILE);
		}
		
		/// <summary>
		/// Fires when the text in the filter changes. Applies the filter to the list of folders
		/// </summary>
		/// <param name="sender">Standard sender object</param>
		/// <param name="e">Text change arguments</param>
		void FilterText_TextChanged(object sender, TextChangedEventArgs e)
		{
			io.UpdateFilteredFolders(FilterText.Text);
		}

		/// <summary>
		/// Fires when the user double-clicks on a folder. Opens the selected folder.
		/// </summary>
		/// <param name="sender">Standard sender object</param>
		/// <param name="e">Information on mouse click</param>
		void FoldersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
            if (FoldersList.SelectedItem != null)
            {
                FleetFolder ff = FoldersList.SelectedItem as FleetFolder;
                
                if (ff != null)
                {
                	openFolder(ff);
                }
            }
		}
		
		/// <summary>
		/// Handles keys. Lower case letter selects corresponding folder. Numbers and upper case letters add to filter. Escape clears filter. Backspace removes last character.
		/// </summary>
		/// <param name="sender">Standard sender object</param>
		/// <param name="e">Argument holding information on which keys were pressed</param>
		void MainWindowWindow_KeyUp(object sender, KeyEventArgs e)
		{
			//Take care of Escape first as it is a special case
			if (e.Key == Key.Escape)
			{
				FilterText.Text = string.Empty;
				e.Handled = true;
				return;
			}
			
			//If it's not Escape then see if it is Backspace which is another special case
			if (e.Key == Key.Back)
			{
				if (FilterText.Text.Length > 0)
				{
					FilterText.Text = FilterText.Text.Substring(0, FilterText.Text.Length - 1);
				}
				e.Handled = true;
				return;
			}
			
			
			string keyPressed = e.Key.ToString();
			
			//See if it's an alphabetic filter. If so act on it.
			if (FILTER_KEYS.Contains(keyPressed) && Keyboard.Modifiers == ModifierKeys.Shift)
			{
			    FilterText.Text += keyPressed;
				e.Handled = true;
				return;
			}
			
			//If it isn't then see if it's a numeric filter. If so act on it.
			if (keyPressed.Length == 2 && NUMERIC_KEYS.Contains(keyPressed))
			{
				keyPressed = keyPressed.Substring(1,1);
				
			    FilterText.Text += keyPressed;
				e.Handled = true;
				return;
			}
			
			//If it isn't then see if it is a command key (for opening one of the folders). If so act on it.
			if (COMMAND_KEYS.Contains(keyPressed) && Keyboard.Modifiers != ModifierKeys.Shift)
			{
				if (io != null)
				{
					FleetFolder ff = io.GetFolderByAccessKey(keyPressed);
					
					if (ff != null)
					{
						openFolder(ff);
					}
				}
			}
		}


        /// <summary>
        /// Fires when the user clicks the icon in the system tray. Shows the main window.
        /// </summary>
        /// <param name="sender">Standard sender object</param>
        /// <param name="e">Standard event args</param>
        void m_notifyIcon_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = storedWindowState;
        }

        /// <summary>
        /// Fires when the main window is closing. Disposes of the icon object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindowWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            systemTrayIcon.Dispose();
            systemTrayIcon = null;
        }

        /// <summary>
        /// When the window state changes sets it up according to it's new state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindowWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                if (systemTrayIcon != null && !isMinimizeTooltipAlreadyShown)
                    systemTrayIcon.ShowBalloonTip(2000);
                isMinimizeTooltipAlreadyShown = true;

            }
            else
            {
                storedWindowState = WindowState;
            }
        }

        private void MainWindowWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CheckTrayIcon();
        }

        void CheckTrayIcon()
        {
            ShowTrayIcon(!IsVisible);
        }

        void ShowTrayIcon(bool show)
        {
            if (systemTrayIcon != null)
                systemTrayIcon.Visible = show;
        }

        #endregion
    }
}
