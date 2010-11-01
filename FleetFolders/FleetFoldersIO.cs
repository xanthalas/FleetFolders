/* Copyright (c) 2010 xanthalas.co.uk
 * 
 * Author: Xanthalas
 * Date  : October 2010
 * 
 */

using System;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;

namespace FleetFolders
{

	/// <summary>
	/// Description of FleetFoldersIO.
	/// </summary>
	public class FleetFoldersIO
	{
		public System.Collections.ObjectModel.ObservableCollection<FleetFolder> Folders;
	
		public FleetFoldersIO(string foldersFile)
		{
			if (File.Exists(foldersFile))
			{
				LoadFolders(foldersFile);
			}
		}
		
		public void LoadFolders(string foldersFile)
		{
			if (Folders == null)
			{
				Folders = new ObservableCollection<FleetFolder>();
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
		
		public FleetFolder GetFolderByAccessKey(string key)
		{
			foreach(FleetFolder folder in Folders)
			{
				if (folder.AccessKey == key)
				{
					return folder;
				}
			}
			
			return null;
		}
	}
}
