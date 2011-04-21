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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace FleetFolders
{

	/// <summary>
	/// Handles reading and writing of folder list to a file. Manages the collection of folders.
	/// </summary>
	public class FleetFoldersIO
	{
		/// <summary>
		/// Collection of all the folders which the user can open
		/// </summary>
		public System.Collections.ObjectModel.ObservableCollection<FleetFolder> Folders;

		/// <summary>
		/// Filtered collection holding only those folders whose name matches the current filter
		/// </summary>
		public System.Collections.ObjectModel.ObservableCollection<FleetFolder> FilteredFolders;
	
		/// <summary>
		/// Create a new FleetFoldersIO object and initialise it from the file whose path is passed in.
		/// </summary>
		/// <param name="foldersFile">Full path to the file containing the folders</param>
		public FleetFoldersIO(string foldersFile)
		{
			if (File.Exists(foldersFile))
			{
				LoadFolders(foldersFile);
				
				UpdateFilteredFolders(string.Empty);
			}
		}
		
		/// <summary>
		/// Loads the folders from the file.
		/// </summary>
		/// <param name="foldersFile">Full path to the file containing the folders</param>
		public void LoadFolders(string foldersFile)
		{
			if (Folders == null)
			{
				Folders = new ObservableCollection<FleetFolder>();
			}

			if (FilteredFolders == null)
			{
				FilteredFolders = new ObservableCollection<FleetFolder>();
			}
			
			if (!File.Exists(foldersFile))
			{
				throw new ArgumentException("File doesn't exist: " + foldersFile);
			}
			
            using (StreamReader reader = new StreamReader(foldersFile))
            {
                string line;

                FleetFolder ff;
                
                while ((line = reader.ReadLine()) != null)
                {
                	if (line.Trim().Length > 0)
                	{
                		ff = Newtonsoft.Json.JsonConvert.DeserializeObject<FleetFolder>(line);
                		
                		Folders.Add(ff);
                	}
                }
            }
		}
		
		/// <summary>
		/// Saves the current collection of folders back to the file.
		/// </summary>
		/// <param name="foldersFile">Full path of the file to write the folders to</param>
		public void SaveFolders(string foldersFile)
		{
			string backupFilename = foldersFile + "~";
			
			if (File.Exists(foldersFile))
		    {
				File.Move(foldersFile, backupFilename);
		    }
			
			using (StreamWriter sw = new StreamWriter(foldersFile))
	       {
				foreach (FleetFolder ff in Folders)
				{
					string line = Newtonsoft.Json.JsonConvert.SerializeObject(ff);
					sw.WriteLine(line);
				}
				
				sw.Close();
	       }
			
			if (File.Exists(backupFilename))
		    {
				File.Delete(backupFilename);
		    }
		}
		
		/// <summary>
		/// Updates the filtered collection based on the filter passed in.
		/// </summary>
		/// <param name="filter">Filter (simple text string) to apply to the URL of each folder</param>
		public void UpdateFilteredFolders(string filter)
		{
			filter = filter.ToUpper();
			
			FilteredFolders.Clear();
			
			var filtered = from f in Folders where f.Url.ToUpper().Contains(filter) select f;
				
			foreach (FleetFolder f in filtered)
			{
				FilteredFolders.Add(f);
			}
		}
		
		/// <summary>
		/// Searches the filtered collection of folders for the key passed in and returns the folder if found.
		/// </summary>
		/// <param name="key">The key (A-Z) to search for</param>
		/// <returns>The folder if a match is found, otherwise null.</returns>
		public FleetFolder GetFolderByAccessKey(string key)
		{
			FleetFolder firstMatch = FilteredFolders
				.Where(f => f.AccessKey.ToUpper() == key)
				.Select (f => f) 
				.SingleOrDefault();
			
			return firstMatch;
		}
	}
}
