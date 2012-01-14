/* Copyright (c) 2012 xanthalas.co.uk
 * 
 * Author: Xanthalas
 * Date  : January 2012
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
using System.Text.RegularExpressions;
using System.Windows;
using System.IO;
using System.Windows.Forms;

namespace FleetFolders
{
    /// <summary>
    /// Interaction logic for AddFolder.xaml
    /// </summary>
    public partial class AddFolder : Window
    {
        public string Path = string.Empty;

        /// <summary>
        /// Constructor for the AddFolder class.
        /// </summary>
        public AddFolder()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Check whether the path contains invalid characters (those not allowed in folder and filenames)
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <returns>True if the path is valid, otherwise false</returns>
        /// <remarks>Displays an error message if the path is invalid</remarks>
        private bool isPathValid(string path)
        {
            string invalidCharsString = new string(System.IO.Path.GetInvalidPathChars());
            string regexString = "[" + Regex.Escape(invalidCharsString) + "]";
            Regex containsInvalidCharacter = new Regex(regexString);

            if (containsInvalidCharacter.IsMatch(path))
            {
                errorMessage.Text = "Path contains invalid characters";
                errorMessage.Foreground = System.Windows.Media.Brushes.Red;
                errorMessage.Visibility = Visibility.Visible;
                return false;
            }
            else
            {
                errorMessage.Text = string.Empty;
                errorMessage.Visibility = Visibility.Hidden;
                return true;
            }

        }

        /// <summary>
        /// Checks whether the path exists
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <returns>True if the path exists, otherwise false</returns>
        /// <remarks>Displays a warning message if the path doesn't exist</remarks>
        private bool seeIfPathExists(string path)
        {
            if (Directory.Exists(path))
            {
                errorMessage.Text = string.Empty;
                errorMessage.Visibility = Visibility.Hidden;
                return true;
            }
            else
            {
                errorMessage.Text = "Warning: path doesn't exist";
                errorMessage.Foreground = System.Windows.Media.Brushes.DarkOrange;
                errorMessage.Visibility = Visibility.Visible;
                return false;
            }
        }

        /// <summary>
        /// Event handler for the OK button click event
        /// </summary>
        /// <param name="sender">Standard sender</param>
        /// <param name="e">Arguments associated with this event</param>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (folderPath.Text.Length > 3 && isPathValid(folderPath.Text))
            {
                Path = folderPath.Text;
                this.DialogResult = true;
                this.Close();
            }
        }

        /// <summary>
        /// Event handler for the Cancel button click event
        /// </summary>
        /// <param name="sender">Standard sender</param>
        /// <param name="e">Arguments associated with this event</param>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Event handler for the text changed event on the folder textbox
        /// </summary>
        /// <param name="sender">Standard sender</param>
        /// <param name="e">Arguments associated with this event</param>
        private void folderPath_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (folderPath.Text.Length == 0)
            {
                okButton.IsEnabled = false;
            }
            else
            {
                if (okButton != null)
                {
                    bool validPath = isPathValid(folderPath.Text);
                    okButton.IsEnabled = validPath;

                    if (validPath)
                    {
                        seeIfPathExists(folderPath.Text);
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for the browse folders button
        /// </summary>
        /// <param name="sender">Standard sender</param>
        /// <param name="e">Arguments associated with this event</param>
        private void browseForFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {

                var result = folderDialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    folderPath.Text = folderDialog.SelectedPath;
                }
            }
        }
    }
}
