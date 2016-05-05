using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swpublished;
using SolidWorks.Interop.swconst;
using SolidWorksTools;
using SolidWorksTools.File;
using GazeboExporter.Export;
using GazeboExporter.GazeboException;

namespace GazeboExporter.Storage
{

    /// <summary>
    /// This class manages the exporter's data storage model for a given document
    /// It stores data in Solidworks attributes and allows them to be easily referenced
    /// via key paths and values.
    /// Both strings and doubles can be stored however this object dosen't enforce strict
    /// typing of values. Classes that use this must maintain type consistency for each
    /// key used
    /// </summary>
    public class StorageModel
    {
        //Hash map of all values stored in the given model
        //Maps path names to a linked list of the attributes that have those path names
        Dictionary<string, LinkedList<IAttribute>> data;
        //Identifier to use for the next attribute created
        int nextID;
        //Document this storage model is attached to
        ModelDoc2 model;

        Dictionary<string, object> storedObjects;
        //Dictionary<string, List<string>> storedStrings;
        //Dictionary<string, List<double>> storedDoubles;


        //Attribute definitions for storing string and double values inside the Solidworks document
        private static IAttributeDef stringDataAttribute;
        private static IAttributeDef doubleDataAttribute;

        /// <summary>
        /// Static method to define SolidWorks attributes required by StoreModel ojbects
        /// Must be called when the addin is loaded
        /// </summary>
        /// <param name="app">The solidworks app</param>
        public static void DefineAttributes(SldWorks app)
        {
            stringDataAttribute = app.DefineAttribute("gism_stringAttr");
            stringDataAttribute.AddParameter("name", (int)swParamType_e.swParamTypeString, 0, 0);
            stringDataAttribute.AddParameter("value", (int)swParamType_e.swParamTypeString, 0, 0);
            stringDataAttribute.Register();

            doubleDataAttribute = app.DefineAttribute("gsim_doubleAttr");
            doubleDataAttribute.AddParameter("name", (int)swParamType_e.swParamTypeString, 0, 0);
            doubleDataAttribute.AddParameter("value", (int)swParamType_e.swParamTypeDouble, 0, 0);
            doubleDataAttribute.Register();
        }

        /// <summary>
        /// Creates a StorageModel for reading and writing data on the given Model
        /// </summary>
        /// <param name="model">Model to read and write data keys from</param>
        public StorageModel(ModelDoc2 model)
        {
            this.model = model;
            data = new Dictionary<string, LinkedList<IAttribute>>();
            //storedDoubles = new Dictionary<string,List<double>>();
            storedObjects = new Dictionary<string,object>();
            //storedStrings = new Dictionary<string,List<string>>();
            Feature feat = model.FirstFeature(); //Start scan of robot attribute's sub-features
            IAttribute attr;
            nextID = 0;
            int id;
            string path;
            int stopindex;
            LinkedList<IAttribute> newlist;

            while (feat != null)  //Scan robot attribute's sub-features
            {
                if (feat.GetTypeName().Equals("Attribute"))
                {
                    attr = (IAttribute)feat.GetSpecificFeature2();
                    if (attr.GetName().StartsWith("gsim"))
                    {
                        stopindex = attr.GetName().IndexOf("_");
                        id = Convert.ToInt32(attr.GetName().Substring(4, stopindex-4));
                        if (id >= nextID)
                            nextID = id + 1;
                        path = ((IParameter)attr.GetParameter("name")).GetStringValue();
                        if (!data.ContainsKey(path))
                        {
                            newlist = new LinkedList<IAttribute>();
                            newlist.AddLast(attr);
                            data.Add(path, newlist);
                        }
                        else
                            data[path].AddLast(attr);
                    }
                }
                feat = feat.GetNextFeature();
            }
        }

        /// <summary>
        /// Sets the value at a given path to the given string
        /// If the path dosen't exist it will be created.
        /// If there is more than one value at the given path, only the first value will be set
        /// </summary>
        /// <param name="path">Path to write value to</param>
        /// <param name="value">Value to set path to</param>
        public void SetString(String path, string value)
        {
            if (data.ContainsKey(path))
                SetString(path, value, 0);
            else
                AddString(path, value);
        }

        /// <summary>
        /// Sets the value at a given path to the given double
        /// If the path doesn't exist it will be created
        /// IF there is more than one value at the given path only the first value will be set
        /// </summary>
        /// <param name="path">Path to write value to</param>
        /// <param name="value">Value to set path to</param>
        public void SetDouble(String path, double value)
        {
            if (data.ContainsKey(path))
                SetDouble(path, value, 0);
            else
                AddDouble(path, value);
        }

        public void SetObject(String path, object obj)
        {
            if (obj == null)
            {
                Delete(path);
                if(storedObjects.ContainsKey(path))
                    storedObjects.Remove(path);
            }
            else
            {
                if (storedObjects.ContainsKey(path))
                    storedObjects[path] = obj;
                else
                    storedObjects.Add(path, obj);
                byte[] pid = model.Extension.GetPersistReference3(obj);
                SetString(path, Convert.ToBase64String(pid));
            }
        }

        /// <summary>
        /// Sets the value of the given index of the given path to the given string
        /// If the path dosn't exist or the index is out of bounds, nothing will happen
        /// </summary>
        /// <param name="path">Path to write value to</param>
        /// <param name="value">Value to set path to</param>
        /// <param name="index">Index of value in path to set</param>
        public void SetString(String path, string value, int index)
        {
            if (data.ContainsKey(path) && data[path].Count > index)
                ((IParameter)data[path].ElementAt(0).GetParameter("value")).SetStringValue(value);
        }

        /// <summary>
        /// Sets the value of the given index of the given path to the given value
        /// If the path dosn't exist or the index is out of bounds, nothing will happen
        /// </summary>
        /// <param name="path">Path to write value to</param>
        /// <param name="value">Value to set path to</param>
        /// <param name="index">Index of value in path to set</param>
        public void SetDouble(String path, double value, int index)
        {
            if (data.ContainsKey(path) && data[path].Count > index)
                ((IParameter)data[path].ElementAt(index).GetParameter("value")).SetDoubleValue(value);
        }

        /// <summary>
        /// Adds a new string value to the given path
        /// If the path dosen't exist it will be created
        /// </summary>
        /// <param name="path">Path to add value to</param>
        /// <param name="value">Value to add to path</param>
        public void AddString(String path, string value)
        {
            IAttribute newAttr = stringDataAttribute.CreateInstance5(model, null, "gsim" + nextID + "_" + path, 1, (int)swInConfigurationOpts_e.swAllConfiguration);//change 1 to 0 to unhide storages
            nextID++;
            ((IParameter)newAttr.GetParameter("name")).SetStringValue(path);
            ((IParameter)newAttr.GetParameter("value")).SetStringValue(value);
            if (!data.ContainsKey(path))
            {
                LinkedList<IAttribute> newData = new LinkedList<IAttribute>();
                newData.AddLast(newAttr);             
                data.Add(path, newData);
            }
            else
                data[path].AddLast(newAttr);
        }

        /// <summary>
        /// Adds a new double value to the given path
        /// If the path dosen't exist it will be created
        /// </summary>
        /// <param name="path">Path to add value to</param>
        /// <param name="value">Value to add to path</param>
        public void AddDouble(String path, double value)
        {
            IAttribute newAttr = doubleDataAttribute.CreateInstance5(model, null, "gsim" + nextID + "_" + path, 0, (int)swInConfigurationOpts_e.swAllConfiguration);//change 1 to 0 to unhide storages
            nextID++;
            ((IParameter)newAttr.GetParameter("name")).SetStringValue(path);
            ((IParameter)newAttr.GetParameter("value")).SetDoubleValue(value);
            if (!data.ContainsKey(path))
            {
                LinkedList<IAttribute> newData = new LinkedList<IAttribute>();         
                newData.AddLast(newAttr);
                data.Add(path, newData);
            }
            else
                data[path].AddLast(newAttr);
        }

        /// <summary>
        /// Gets the first string at the specified path
        /// </summary>
        /// <param name="path">The path to the string</param>
        /// <returns>the string at the path location</returns>
        public string GetString(String path)
        {
            if (data.ContainsKey(path))
                return ((IParameter)data[path].ElementAt(0).GetParameter("value")).GetStringValue();
            else
                return "";
        }

        /// <summary>
        /// gets the string at the path location and at the given index
        /// </summary>
        /// <param name="path">Path to String array</param>
        /// <param name="index">index in the array</param>
        /// <returns>Desired string</returns>
        public string GetString(String path, int index)
        {
            if (data.ContainsKey(path) && data[path].Count > index)
                return ((IParameter)data[path].ElementAt(index).GetParameter("value")).GetStringValue();
            else
                return "";
        }
        
        /// <summary>
        /// gets the value at the path location and at the given index
        /// </summary>
        /// <param name="path">Path to the array</param>
        /// <param name="index">index in the array</param>
        /// <returns>desired value</returns>
        public double GetDouble(String path, int index)
        {
            if (data.ContainsKey(path) && data[path].Count > index)
                return ((IParameter)data[path].ElementAt(index).GetParameter("value")).GetDoubleValue();
            else
                return 0;
        }

        /// <summary>
        /// Gets the first double at the specified path
        /// </summary>
        /// <param name="path">The path to the double</param>
        /// <returns>the double at the path location</returns>
        public double GetDouble(String path)
        {
            if (data.ContainsKey(path))
                return ((IParameter)data[path].ElementAt(0).GetParameter("value")).GetDoubleValue();
            else
                return 0;
        }


        public object GetObject(String path)
        {
            if (storedObjects.ContainsKey(path) && storedObjects[path] != null)
                if ((storedObjects[path] is Component2) && ((Component2)storedObjects[path]).GetSuppression() != -1)
                    return storedObjects[path];

            int Errors;
            byte[] pid = Convert.FromBase64String(GetString(path));
            object axis = model.Extension.GetObjectByPersistReference3(pid, out Errors);
            int Errors2;
            object axis2 = model.Extension.GetObjectByPersistReference3(pid, out Errors2);//called twice due to some inconsistant behaviour in the getObjectByPersustReference3 method
            if (Errors == (int)swPersistReferencedObjectStates_e.swPersistReferencedObject_Ok && Errors2 == (int)swPersistReferencedObjectStates_e.swPersistReferencedObject_Ok)
            {
                if (storedObjects.ContainsKey(path))
                    storedObjects[path] = axis;
                else
                    storedObjects.Add(path, axis);
                return axis;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// gets the number of items at the given path
        /// </summary>
        /// <param name="path">Desired path</param>
        /// <returns>Number of items at path</returns>
        public int GetCount(String path)
        {
            if (data.ContainsKey(path))
                return data[path].Count;
            else
                return 0;
        }

        /// <summary>
        /// deletes all values at the given path
        /// </summary>
        /// <param name="path">location to be deleted</param>
        public void Delete(String path)
        {
            if (data.ContainsKey(path))
            {
                foreach (IAttribute a in data[path])
                    a.Delete(false);
                data.Remove(path);
            }
        }

        /// <summary>
        /// deletes the item at the given path and index
        /// </summary>
        /// <param name="path">Location of the item</param>
        /// <param name="index">index of the item</param>
        public void Delete(String path, int index)
        {
            if (data.ContainsKey(path) && data[path].Count > index)
            {
                data[path].ElementAt(index).Delete(false);
                data[path].Remove(data[path].ElementAt(index));
                if (data[path].Count == 0)
                    data.Remove(path);
            }
        }

        /// <summary>
        /// deletes everything at the desired path
        /// </summary>
        /// <param name="path">Location to be deleted</param>
        public void DeleteAll(String path)
        {
            string[] keys = data.Keys.ToArray();
            foreach (string s in keys)
            {
                if (s.StartsWith(path))
                    Delete(s);
            }
        }
        
      
    }
}
