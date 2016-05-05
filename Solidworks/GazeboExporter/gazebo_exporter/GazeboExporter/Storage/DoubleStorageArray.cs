using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazeboExporter.Storage
{
    /// <summary>
    /// This class repressents an array of doubles that are storred together in the solidworks document
    /// </summary>
    class DoubleStorageArray : IEnumerable
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
        public DoubleStorageArray(StorageModel swData, String path)
        {
            this.swData = swData;
            this.path = path;
        }

        /// <summary>
        /// Adds an item to the array
        /// </summary>
        /// <param name="item">item to be added to the array</param>
        public void AddItem(double item)
        {
            swData.AddDouble(path, item);
        }

        /// <summary>
        /// Removes the item at the given index
        /// </summary>
        /// <param name="index">index of item to remove</param>
        public void RemoveIndex(int index)
        {
            swData.Delete(path, index);
        }

        /// <summary>
        /// removes every instance of the specified item
        /// </summary>
        /// <param name="item">item to be removed</param>
        public void RemoveItem(double item)
        {
            for (int l = 0; l < Count; l++)
            {
                if (swData.GetDouble(path, l) == item)
                    swData.Delete(path, l);
            }
        }

        /// <summary>
        /// Used to acess the data in the array like a conventional array
        /// </summary>
        /// <param name="index">index of the array</param>
        /// <returns>the value at the given index</returns>
        public double this[int index]
        {
            get
            {
                return swData.GetDouble(path, index);
            }
            set
            {
                swData.SetDouble(path, value, index);
            }
        }

        /// <summary>
        /// allows for iteration of this array using a foreach loop
        /// </summary>
        /// <returns>The Enumerator being used</returns>
        public IEnumerator GetEnumerator()
        {
            for (int l = 0; l < Count; l++)
                yield return swData.GetDouble(path, l);
        }
    }
}
