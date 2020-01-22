using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace spToolbelt2019Lib
{

    public class scriptItems : CollectionBase
    {
        // Fields...
        
        public bool IsValid
        {
            get
            {
                foreach (scriptItem itm in this)
                {
                    System.Diagnostics.Trace.WriteLine(itm.RawLine);
                    if (!itm.Verified) return false;
                }
                return true;
            }
            
        }
        
        public string RawText
        {
            get
            {
                string cOutText = "";
                foreach (scriptItem itm in this)
                {
                    cOutText += itm.RawLine + Environment.NewLine;
                   
                }
                return cOutText;
            }
        }

        // public methods...

        #region LoadFromFile

        public string LoadFromFile(string cFileName)
        {
            string cResults = "";
            try
            {
                StreamReader oReader = new StreamReader(cFileName);

                string line;
                while ((line = oReader.ReadLine()) != null)
                {
                    int iOrder = 1;
                    try
                    {
                        if (!string.IsNullOrEmpty(line) && !line.StartsWith("#"))
                        {
                            scriptItem itm = new scriptItem(line);
                            cResults += itm.Status;
                            itm.Order = iOrder;
                            Add(itm);
                            iOrder += 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        cResults += string.Format("Error in LoadFromFile:{0} :: {1}", line, ex.Message);

                    }


                }

               
            }
            catch (Exception ex)
            {
                cResults += "Error in Load From File: " + ex.Message;
            }
            return cResults;
        }

        #endregion

        #region Add
        public int Add(scriptItem scriptItem)
        {
            return List.Add(scriptItem);
        }
        #endregion
        #region IndexOf
        public int IndexOf(scriptItem scriptItem)
        {
            for (int i = 0; i < List.Count; i++)
                if (this[i] == scriptItem)    // Found it
                    return i;
            return -1;
        }
        #endregion
        #region Insert
        public void Insert(int index, scriptItem scriptItem)
        {
            List.Insert(index, scriptItem);
        }
        #endregion
        #region Remove
        public void Remove(scriptItem scriptItem)
        {
            List.Remove(scriptItem);
        }
        #endregion
        #region Find
        // TODO: If desired, change parameters to Find method to search based on a property of scriptItem.
        public scriptItem Find(scriptItem scriptItem)
        {
            foreach (scriptItem scriptItemItem in this)
                if (scriptItemItem == scriptItem)    // Found it
                    return scriptItemItem;
            return null;    // Not found
        }
        #endregion
        #region Contains
        // TODO: If you changed the parameters to Find (above), change them here as well.
        public bool Contains(scriptItem scriptItem)
        {
            return (Find(scriptItem) != null);
        }
        #endregion

        // public properties...
        #region this[int index]
        public scriptItem this[int index]
        {
            get
            {
                return (scriptItem)List[index];
            }
            set
            {
                List[index] = value;
            }
        }
        #endregion
    }

}
