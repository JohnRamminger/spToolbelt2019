using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spToolbelt2019Lib
{
    public static class extFieldCollection
    {

        #region Site Columns

        #region Lookup

        public static void EnsureFieldLookup(this FieldCollection flds, string cInternalName, string cDisplayName, string cFieldDescription, string cGroup, string cListName, string cShowField)
        {
            try
            {
                if (!flds.HasField(cDisplayName, cInternalName))
                {
                    flds.AddFieldLookup(cInternalName, cDisplayName, cFieldDescription, cGroup, cListName, cShowField);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.EnsureField -  {0} - {1}", cDisplayName, ex.Message), ex);
            }
        }

        public static void AddFieldLookup(this FieldCollection flds, string cInternalName, string cDisplayName, string cFieldDescription, string cGroup, string cListName, string cShowField)
        {
            try
            {
                string strNewField = string.Format("<Field Type='Lookup'  DisplayName='{0}' StaticName='{1}' Name='{1}' Group = '{2}' Description='{3}' List='{4}' ShowField='{5}' />", cDisplayName, cInternalName, cGroup, cFieldDescription, cListName, cShowField);
                flds.AddFieldAsXml(strNewField, true, AddFieldOptions.AddFieldInternalNameHint);
                flds.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.AddField -  {0} - {1}", cDisplayName, ex.Message), ex);
            }
        }
        #endregion



        #region Text


        public static void EnsureField(this FieldCollection flds, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                if (!flds.HasField(cFieldTitle, cInternalName))
                {
                    flds.AddField(cInternalName, cFieldTitle, cFieldDescription, cGroup);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.EnsureField -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }

        public static void AddField(this FieldCollection flds, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                cInternalName = GetInternalName(cInternalName, cFieldTitle);

                string strNewField = string.Format("<Field Type='Text'  DisplayName='{0}'  StaticName='{1}' Name='{1}' Group = '{2}' Description='{3}' />", cFieldTitle, cInternalName, cGroup, cFieldDescription);
                flds.AddFieldAsXml(strNewField, true, AddFieldOptions.AddFieldInternalNameHint);
                flds.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.AddField -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }
        #endregion

        #region Note


        public static void EnsureFieldNote(this FieldCollection flds, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                if (!flds.HasField(cFieldTitle, cInternalName))
                {
                    flds.AddFieldNote(cInternalName, cFieldTitle, cFieldDescription, cGroup);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.EnsureField -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }

        public static void AddFieldNote(this FieldCollection flds, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                cInternalName = GetInternalName(cInternalName, cFieldTitle);

                string strNewField = string.Format("<Field Type='Note' DisplayName='{0}'  StaticName='{1}' Name='{1}' Group = '{2}' Description='{3}' />", cFieldTitle, cInternalName, cGroup, cFieldDescription);
                flds.AddFieldAsXml(strNewField, true, AddFieldOptions.AddFieldInternalNameHint);
                flds.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.AddField -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }
        #endregion

        #region DateTime


        public static void EnsureFieldDateTime(this FieldCollection flds, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                if (!flds.HasField(cFieldTitle, cInternalName))
                {
                    flds.AddFieldDateTime(cInternalName, cFieldTitle, cFieldDescription, cGroup);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.EnsureField -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }

        public static void AddFieldDateTime(this FieldCollection flds, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                cInternalName = GetInternalName(cInternalName, cFieldTitle);

                string strNewField = string.Format("<Field Type='DateTime' DisplayName='{0}'  StaticName='{1}' Name='{1}' Group = '{2}' Description='{3}' />", cFieldTitle, cInternalName, cGroup, cFieldDescription);
                flds.AddFieldAsXml(strNewField, true, AddFieldOptions.AddFieldInternalNameHint);
                flds.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.AddField -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }
        #endregion

        #region Boolean


        public static void EnsureFieldBoolean(this FieldCollection flds, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                if (!flds.HasField(cFieldTitle, cInternalName))
                {
                    flds.AddFieldBoolean(cInternalName, cFieldTitle, cFieldDescription, cGroup);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.EnsureField -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }

        public static void AddFieldBoolean(this FieldCollection flds, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                cInternalName = GetInternalName(cInternalName, cFieldTitle);
                string strNewField = string.Format("<Field Type='Boolean' DisplayName='{0}'  StaticName='{1}' Name='{1}' Group = '{2}' Description='{3}' ><Default>0</Default></Field>", cFieldTitle, cInternalName, cGroup, cFieldDescription);
                flds.AddFieldAsXml(strNewField, true, AddFieldOptions.AddFieldInternalNameHint);
                flds.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.AddField -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }
        #endregion

        #region Choice


        public static void EnsureFieldChoice(this FieldCollection flds, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup, string[] cChoices)
        {
            try
            {
                if (!flds.HasField(cFieldTitle, cInternalName))
                {
                    flds.AddFieldChoice(cInternalName, cFieldTitle, cFieldDescription, cGroup, cChoices);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.EnsureField -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }

        public static void AddFieldChoice(this FieldCollection flds, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup, string[] cChoices)
        {
            try
            {
                cInternalName = GetInternalName(cInternalName, cFieldTitle);


                string strNewField = string.Format("<Field Type='Choice' DisplayName='{0}'  StaticName='{1}' Name='{1}'  Description='{2}' Group='{3}' ><CHOICES>", cFieldTitle, cInternalName, cFieldDescription, cGroup);

                foreach (string item in cChoices)
                {
                    strNewField += string.Format("<CHOICE>{0}</CHOICE>", item);
                }
                strNewField += "</CHOICES></Field>";
                flds.AddFieldAsXml(strNewField, true, AddFieldOptions.AddFieldInternalNameHint);
                flds.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.AddField -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }

        public static bool HasField(this FieldCollection flds, string cFieldTitle,string cInternalName)
        {
            Field fldOnTitle = flds.GetField(cFieldTitle);
            if (fldOnTitle!=null)
            {
                return true;
            }
            return (flds.GetField(cInternalName) != null);
        }

        public static bool HasField(this FieldCollection flds, Guid fieldID)
        {
            Field fldOnTitle = flds.GetField(fieldID);
            if (fldOnTitle != null)
            {
                return true;
            }
            return false;
        }
        public static Field GetField(this FieldCollection flds, Guid fieldID)
        {
            try
            {
                flds.Context.Load(flds, f => f.Include(finfo=>finfo.Id));
                flds.Context.ExecuteQuery();
                foreach(Field fld in flds)
                {
                    if (fld.Id==fieldID)
                    {
                        return fld;
                    }
                }
            } catch (Exception)
            {
                throw new Exception("Eror finding Field with ID: " + fieldID.ToString());
            }
            return null;
        }


        public static bool HasField(this FieldCollection flds, string cFieldTitle)
        {
            return (flds.GetField(cFieldTitle) != null);
        }
        public static Field GetField(this FieldCollection flds, string cFieldTitle)
        {
            try
            {

                flds.Context.Load(flds,fs=>fs.Include(f=>f.Title,f=>f.InternalName,f=>f.TypeAsString));
                flds.Context.ExecuteQuery();
                foreach (Field fld in flds)
                {
                    if (fld.Title.ToLower() == cFieldTitle.ToLower())
                    {
                        return fld;
                    }
                    if (fld.InternalName.ToLower() == cFieldTitle.ToLower())
                    {
                        return fld;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.GetField -  {0} - {1}", cFieldTitle, ex.Message), ex);

            }
            return null;
        }
        #endregion

        #region Boolean


        public static void EnsureFieldNumber(this FieldCollection flds, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                if (!flds.HasField(cFieldTitle, cInternalName))
                {
                    flds.AddFieldNumber(cInternalName, cFieldTitle, cFieldDescription, cGroup);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.EnsureField -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }

        public static void AddFieldNumber(this FieldCollection flds, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                cInternalName = GetInternalName(cInternalName, cFieldTitle);
                string strNewField = string.Format("<Field Type='Number' DisplayName='{0}'  StaticName='{1}' Name='{1}' Group = '{2}' Description='{3}' ><Default>0</Default></Field>", cFieldTitle, cInternalName, cGroup, cFieldDescription);
                flds.AddFieldAsXml(strNewField, true, AddFieldOptions.AddFieldInternalNameHint);
                flds.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.AddField -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }
        #endregion

        #region Boolean


        public static void EnsureFieldInteger(this FieldCollection flds, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                if (!flds.HasField(cFieldTitle, cInternalName))
                {
                    flds.AddFieldInteger(cInternalName, cFieldTitle, cFieldDescription, cGroup);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.EnsureField -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }

        public static void AddFieldInteger(this FieldCollection flds, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                 cInternalName = GetInternalName(cInternalName, cFieldTitle);
                string strNewField = string.Format("<Field Type='Number' DisplayName='{0}'  StaticName='{1}' Name='{1}' Group = '{2}' Description='{3}' Decimals='0'><Default>0</Default></Field>", cFieldTitle, cInternalName, cGroup, cFieldDescription);
                flds.AddFieldAsXml(strNewField, true, AddFieldOptions.AddFieldInternalNameHint);
                flds.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.AddField -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }
        #endregion

        #region Currency


        public static void EnsureFieldCurrency(this FieldCollection flds, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                if (!flds.HasField(cFieldTitle, cInternalName))
                {
                    flds.AddFieldCurrency(cInternalName, cFieldTitle, cFieldDescription, cGroup);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.EnsureField -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }

        public static void AddFieldCurrency(this FieldCollection flds, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                cInternalName = GetInternalName(cInternalName, cFieldTitle);
                string strNewField = string.Format("<Field Type='Currency' DisplayName='{0}'  StaticName='{1}' Name='{1}' Group = '{2}' Description='{3}' ><Default>0</Default></Field>", cFieldTitle, cInternalName, cGroup, cFieldDescription);
                flds.AddFieldAsXml(strNewField, true, AddFieldOptions.AddFieldInternalNameHint);
                flds.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.AddField -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }

        private static string GetInternalName(string cInternalName, string cFieldTitle)
        {
            string cResult = cInternalName;
            try
            {
                cResult = cResult.Replace(" ", "");
                cResult = cResult.Replace("#", "");
                cResult = cResult.Replace("/", "");


            }
            catch (Exception ex)
            {
                throw new Exception("Error in extFieldCollection.GetInternalName -  " + ex.Message, ex);
            }
            return cResult;
        }
        #endregion


        #endregion


        #region Computed


        public static void EnsureFieldComputed(this FieldCollection flds,string cFieldType, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup,string cCalc)
        {
            try
            {
                if (!flds.HasField(cFieldTitle, cInternalName))
                {
                    flds.AddFieldComputed(cInternalName, cFieldTitle, cFieldDescription, cGroup,cCalc);
                } else
                {
                    Field fld = flds.GetField(cInternalName);
                    if (fld.TypeAsString != cFieldType)
                    {
                        System.Diagnostics.Trace.WriteLine("Alert");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.EnsureField -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }

        public static void AddFieldComputed(this FieldCollection flds, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup, string cCalc)
        {
            try
            {
                cInternalName = GetInternalName(cInternalName, cFieldTitle);

                string strNewField = string.Format("<Field Type='Note' DisplayName='{0}'  StaticName='{1}' Name='{1}' Group = '{2}' Description='{3}' />", cFieldTitle, cInternalName, cGroup, cFieldDescription);
                flds.AddFieldAsXml(strNewField, true, AddFieldOptions.AddFieldInternalNameHint);
                flds.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.AddField -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }
        #endregion


        #region Url


        public static void EnsureFieldUrl(this FieldCollection flds, string cFieldType,string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                if (!flds.HasField(cFieldTitle, cInternalName))
                {
                    flds.AddFieldUrl(cInternalName, cFieldTitle, cFieldDescription, cGroup);
                }
             else
            {
                Field fld = flds.GetField(cInternalName);
                if (fld.TypeAsString != cFieldType)
                {
                    System.Diagnostics.Trace.WriteLine("Alert");
                }
            }
        }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.EnsureField -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }

        public static void AddFieldUrl(this FieldCollection flds, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                cInternalName = GetInternalName(cInternalName, cFieldTitle);

                string strNewField = string.Format("<Field Type='URL' DisplayName='{0}'  StaticName='{1}' Name='{1}' Group = '{2}' Description='{3}' />", cFieldTitle, cInternalName, cGroup, cFieldDescription);
                flds.AddFieldAsXml(strNewField, true, AddFieldOptions.AddFieldInternalNameHint);
                flds.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.AddField -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }
        #endregion


        #region HTML


        public static void EnsureFieldHTML(this FieldCollection flds, string cFieldType, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                if (!flds.HasField(cFieldTitle, cInternalName))
                {
                    flds.AddFieldHTML(cInternalName, cFieldTitle, cFieldDescription, cGroup);
                }
                else
                {
                    Field fld = flds.GetField(cInternalName);
                    if (fld.TypeAsString != cFieldType)
                    {
                        System.Diagnostics.Trace.WriteLine("Alert");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.EnsureField -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }

        public static void AddFieldHTML(this FieldCollection flds, string cInternalName, string cFieldTitle, string cFieldDescription, string cGroup)
        {
            try
            {
                cInternalName = GetInternalName(cInternalName, cFieldTitle);

                string strNewField = string.Format("<Field Type='HTML' DisplayName='{0}'  StaticName='{1}' Name='{1}' Group = '{2}' Description='{3}' />", cFieldTitle, cInternalName, cGroup, cFieldDescription);
                flds.AddFieldAsXml(strNewField, true, AddFieldOptions.AddFieldInternalNameHint);
                flds.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error in extFieldCollection.AddField -  {0} - {1}", cFieldTitle, ex.Message), ex);
            }
        }
        #endregion

    }
}
