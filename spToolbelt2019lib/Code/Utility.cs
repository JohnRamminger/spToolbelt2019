using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace spToolbelt2019Lib
{
    public static class Utility
    {
        internal static void EnsureListField(ClientContext ctx, scriptItem oWorkItem)
        {
            try
            {
                ClientContext workCTX = new ClientContext(oWorkItem.GetParm("url")) { Credentials = ctx.Credentials };
                List lst = workCTX.Web.Lists.GetByTitle(oWorkItem.GetParm("listname"));
                FieldCollection flds = lst.Fields;
                workCTX.Load(lst);
                workCTX.Load(flds);
                workCTX.ExecuteQuery();

                switch (oWorkItem.Command)
                {
                    case "ensure-field":
                        flds.EnsureField(oWorkItem.GetParm("InternalName"), oWorkItem.GetParm("Title"), oWorkItem.GetParm("Description"), oWorkItem.GetParm("Group"));
                        break;

                    case "ensure-fieldinteger":

                        flds.EnsureFieldInteger(oWorkItem.GetParm("InternalName"), oWorkItem.GetParm("Title"), oWorkItem.GetParm("Description"), oWorkItem.GetParm("Group"));
                        break;

                    case "ensure-fielddatetime":

                        flds.EnsureFieldDateTime(oWorkItem.GetParm("InternalName"), oWorkItem.GetParm("Title"), oWorkItem.GetParm("Description"), oWorkItem.GetParm("Group"));
                        break;
                    case "ensure-fieldcurrency":

                        flds.EnsureFieldCurrency(oWorkItem.GetParm("InternalName"), oWorkItem.GetParm("Title"), oWorkItem.GetParm("Description"), oWorkItem.GetParm("Group"));
                        break;
                    case "ensure-fieldchoice":

                        flds.EnsureFieldChoice(oWorkItem.GetParm("InternalName"), oWorkItem.GetParm("Title"), oWorkItem.GetParm("Description"), oWorkItem.GetParm("Description"), oWorkItem.GetParm("choices"));
                        break;
                    case "ensure-fieldboolean":

                        flds.EnsureFieldBoolean(oWorkItem.GetParm("InternalName"), oWorkItem.GetParm("Title"), oWorkItem.GetParm("Description"), oWorkItem.GetParm("Group"));
                        break;

                    case "ensure-fieldnote":

                        flds.EnsureFieldNote(oWorkItem.GetParm("InternalName"), oWorkItem.GetParm("Title"), oWorkItem.GetParm("Description"), oWorkItem.GetParm("Group"));
                        break;
                    case "ensure-fieldnnumber":

                        flds.EnsureFieldNumber(oWorkItem.GetParm("InternalName"), oWorkItem.GetParm("Title"), oWorkItem.GetParm("Description"), oWorkItem.GetParm("Group"));
                        break;

                    default:

                        break;
                }



            }
            catch (Exception ex)
            {
                throw new Exception("Error in Utility.EnsureListField -  " + ex.Message, ex);

            }


        }
    }
}
