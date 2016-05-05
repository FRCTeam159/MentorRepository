using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazeboExporter.Storage
{
    /// <summary>
    /// This class repressents an array of strings that are storred together in the solidworks document
    /// </summary>
    public class StringStorageArray : IEnumerable
    {
        private StorageModel swData;
        private String path;

        /// <summary>
        /// The count of how many items are in the array
        /// </summary>
        public int Count
        {
            get
            {
                return swData.GetCount(path);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="swData">Storage model that contains this array</param>
        /// <param name="path">Path this array will use to store data</param>
        public StringStorageArray(StorageModel swData, String path)
        {
            this.swData = swData;
            this.path = path;
        }

        /// <summary>
        /// Adds an item to the array
        /// </summary>
        /// <param name="item">item to be added to the array</param>
        public void AddItem(String item)
        {
            swData.AddString(path, item);
        }

        /// <summary>
        /// Removes the item at the given index
        /// </summary>
        /// <param name="index">index of item to remove</param>
        public void RemoveItem(int index)
        {
            swData.Delete(path, index);
        }


        /// <summary>
        /// Used to acess the data in the array like a conventional array
        /// </summary>
        /// <param name="index">index of the array</param>
        /// <returns>the value at the given index</returns>
        public string this[int index]
        {
            get
            {
                return swData.GetString(path, index);
            }
            set
            {
                swData.SetString(path, value, index);
            }
        }

        /// <summary>
        /// allows for iteration of this array using a foreach loop
        /// </summary>
        /// <returns>The Enumerator being used</returns>
        public IEnumerator GetEnumerator()
        {
            for (int l = 0; l < Count; l++)
                yield return swData.GetString(path, l);
        }

        /// <summary>
        /// get the path to its data
        /// </summary>
        /// <returns>the path of this storage array</returns>
        public String getPath()
        {
            return path;
        }
    }
}
