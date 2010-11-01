/* Copyright (c) 2010 xanthalas.co.uk
 * 
 * Author: Xanthalas
 * Date  : October 2010
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;
using System.Globalization;

namespace FleetFolders
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private const string STORAGEFILE = "folders.json";
		
		private FleetFoldersIO io;

		private bool _minimizeOnSelection = true;

		public MainWindow()
		{
			InitializeComponent();
			
			io = new FleetFoldersIO(STORAGEFILE);
			
			FoldersList.ItemsSource = io.Folders;
		}
		
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
		
		private void openFolder(FleetFolder folder)
		{
			try {
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
				MessageBox.Show("FleetFolders Error", "Cannot start explorer: " + e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		
		private void KeyDownEventHandler(object sender, KeyEventArgs e)
		{
			if (!FilterText.IsFocused)
			{
				if (io != null)
				{
					FleetFolder ff = io.GetFolderByAccessKey(e.Key.ToString().ToUpper());
					
					if (ff != null)
					{
						openFolder(ff);
					}
				}
			}
		}
		
		void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			recalculateColumnWidths(e.NewSize.Width);
		}
		
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			io.SaveFolders(STORAGEFILE);
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
	}
}