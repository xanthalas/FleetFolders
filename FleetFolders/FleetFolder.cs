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
	/// FleetFolder holds a single folder
	/// </summary>
	public class FleetFolder : INotifyPropertyChanged, IComparable
	{
		private int _usageCount = 0;
		private DateTime _lastAccessed;
		
		/// <summary>
		/// The keyboard key used to select this folder
		/// </summary>
		public string AccessKey {get; set;}
		
		/// <summary>
		/// The URL of the folder
		/// </summary>
		public string Url {get; set;}
		
		/// <summary>
		/// Number of times this folder has been selected
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
		/// The last time this folder was accessed
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
		
		public FleetFolder(string accessKey, string url, int usageCount, DateTime lastAccessed)
		{
			AccessKey = accessKey;
			Url = url;
			UsageCount = usageCount;
			LastAccessed = lastAccessed;
		}
		
		public string LastAccessedInDisplayFormat
		{
			get
			{
				return LastAccessed.Day.ToString() + "/" + LastAccessed.Month.ToString() + "/" + LastAccessed.Year.ToString() +
					" " + LastAccessed.Hour.ToString() + ":" + LastAccessed.Minute.ToString() + ":" + LastAccessed.Second.ToString();
			}
		}
		
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Notify interested parties when property changes
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
