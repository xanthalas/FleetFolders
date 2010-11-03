/* Copyright (c) 2010 xanthalas.co.uk
 * 
 * Author: Xanthalas
 * Date  : October 2010
 * 
 */
using System;
using System.ComponentModel;

namespace FleetFolders
{
	/// <summary>
	/// FleetFolder holds a single folder.
	/// </summary>
	public class FleetFolder : INotifyPropertyChanged, IComparable
	{
		private int _usageCount = 0;
		private DateTime _lastAccessed;
		
		/// <summary>
		/// Gets/Sets the keyboard key used to select this folder.
		/// </summary>
		public string AccessKey {get; set;}
		
		/// <summary>
		/// Gets/Sets the URL of the folder.
		/// </summary>
		public string Url {get; set;}
		
		/// <summary>
		/// Gets/Sets the number of times this folder has been selected.
		/// </summary>
		public int UsageCount {
			get
			{
				return _usageCount;
			}
			set
			{
				if (_usageCount != value)
				{
					_usageCount = value;
					NotifyPropertyChanged(UsageCount);
				}
			}
		}
		
		/// <summary>
		/// Gets/Sets the last time this folder was accessed.
		/// </summary>
		public DateTime LastAccessed {
			get
			{
				return _lastAccessed;
			}
			set
			{
				if (_lastAccessed != value)
				{
					_lastAccessed = value;
					NotifyPropertyChanged(LastAccessed);
				}
			}
		}
		
		/// <summary>
		/// Construct a new FleetFolder object.
		/// </summary>
		/// <param name="accessKey">Keybord key (A-Z) used to access this folder</param>
		/// <param name="url">Full URL of the folder</param>
		/// <param name="usageCount">Number of times this folder has been opened</param>
		/// <param name="lastAccessed">Date and time this folder was last opened</param>
		public FleetFolder(string accessKey, string url, int usageCount, DateTime lastAccessed)
		{
			AccessKey = accessKey;
			Url = url;
			UsageCount = usageCount;
			LastAccessed = lastAccessed;
		}
		
        #region INotifyPropertyChanged Members

        /// <summary>
        /// Event to raise when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notify interested parties when property changes
        /// </summary>
        /// <param name="changed">The object which has changed</param>
        protected void NotifyPropertyChanged(object changed)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
            	if (changed.GetType() == typeof(DateTime))
            	{
            		handler(this, new PropertyChangedEventArgs("LastAccessed"));	
            	}
               else
               {
               	handler(this, new PropertyChangedEventArgs("UsageCount"));	
               }
            }
        }

        #endregion


        /// <summary>
        /// Compares the current object to the one passed in. 
        /// </summary>
        /// <param name="obj">The object (usually a ConnectIniData) to compare</param>
        /// <returns>0 if they are equal or -1 if they are not equal</returns>
        public int CompareTo(object obj)
        {
            FleetFolder f = (FleetFolder) obj;

            if (f.Url == this.Url && f.UsageCount == this.UsageCount && f.LastAccessed == this.LastAccessed)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
		
	}
}
