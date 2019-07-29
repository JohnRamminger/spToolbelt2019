using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client.Publishing;

namespace spToolbelt2019Lib
{
    public static class spcUtil
    {
        public static ListTemplateType GetTemplateType(string cTemplateType)
        {
            ListTemplateType oRetval = ListTemplateType.GenericList;

            try
            {
              
                switch (cTemplateType)
                {
                    case "InvalidType":
                        oRetval= ListTemplateType.InvalidType;
                        
                        break;
                    case "NoListTemplate":
                        oRetval = ListTemplateType.NoListTemplate;
                        
                        break;
                    case "GenericList":
                        oRetval = ListTemplateType.GenericList;
                        
                        break;
                    case "DocumentLibrary":
                        oRetval = ListTemplateType.DocumentLibrary;
                        
                        break;
                    case "Survey":
                        oRetval = ListTemplateType.Survey;
                        
                        break;
                    case "Links":
                        oRetval = ListTemplateType.Links;
                        
                        break;
                    case "Announcements":
                        oRetval = ListTemplateType.Announcements;
                        
                        break;
                    case "Contacts":
                        oRetval = ListTemplateType.Contacts;
                        
                        break;
                    case "Events":
                        oRetval = ListTemplateType.Events;
                        
                        break;
                    case "Tasks":
                        oRetval = ListTemplateType.Tasks;
                        
                        break;
                    case "DiscussionBoard":
                        oRetval = ListTemplateType.DiscussionBoard;
                        
                        break;
                    case "PictureLibrary":
                        oRetval = ListTemplateType.PictureLibrary;
                        
                        break;
                    case "DataSources":
                        oRetval = ListTemplateType.DataSources;
                        
                        break;
                    case "WebTemplateCatalog":
                        oRetval = ListTemplateType.WebTemplateCatalog;
                        
                        break;
                    case "UserInformation":
                        oRetval = ListTemplateType.UserInformation;
                        
                        break;
                    case "WebPartCatalog":
                        oRetval = ListTemplateType.WebPartCatalog;
                        
                        break;
                    case "ListTemplateCatalog":
                        oRetval = ListTemplateType.ListTemplateCatalog;
                        
                        break;
                    case "XMLForm":
                        oRetval = ListTemplateType.XMLForm;
                        
                        break;
                    case "":
                        oRetval = ListTemplateType.MasterPageCatalog;
                        
                        break;
                    case "NoCodeWorkflows":
                        oRetval = ListTemplateType.NoCodeWorkflows;
                        
                        break;
                    case "WorkflowProcess":
                        oRetval = ListTemplateType.WorkflowProcess;
                        
                        break;
                    case "WebPageLibrary":
                        oRetval = ListTemplateType.WebPageLibrary;
                        
                        break;
                    case "CustomGrid":
                        oRetval = ListTemplateType.CustomGrid;
                        
                        break;
                    case "SolutionCatalog":
                        oRetval = ListTemplateType.SolutionCatalog;
                        
                        break;
                    case "NoCodePublic":
                        oRetval = ListTemplateType.NoCodePublic;
                        
                        break;
                    case "ThemeCatalog":
                        oRetval = ListTemplateType.ThemeCatalog;
                        
                        break;
                    case "DesignCatalog":
                        oRetval = ListTemplateType.DesignCatalog;
                        
                        break;
                    case "AppDataCatalog":
                        oRetval = ListTemplateType.AppDataCatalog;
                        
                        break;
                    case "DataConnectionLibrary":
                        oRetval = ListTemplateType.DataConnectionLibrary;
                        
                        break;
                    case "WorkflowHistory":
                        oRetval = ListTemplateType.WorkflowHistory;
                        
                        break;
                    case "GanttTasks":
                        oRetval = ListTemplateType.GanttTasks;
                        
                        break;
                    case "HelpLibrary":
                        oRetval = ListTemplateType.HelpLibrary;
                        
                        break;
                    case "AccessRequest":
                        oRetval = ListTemplateType.AccessRequest;
                        
                        break;
                    case "TasksWithTimelineAndHierarchy":
                        oRetval = ListTemplateType.TasksWithTimelineAndHierarchy;
                        
                        break;
                    case "MaintenanceLogs":
                        oRetval = ListTemplateType.MaintenanceLogs;
                        
                        break;
                    case "Meetings":
                        oRetval = ListTemplateType.Meetings;
                        
                        break;
                    case "Agenda":
                        oRetval = ListTemplateType.Agenda;
                        
                        break;
                    case "MeetingUser":
                        oRetval = ListTemplateType.MeetingUser;
                        
                        break;
                    case "Decision":
                        oRetval = ListTemplateType.Decision;
                        
                        break;
                    case "MeetingObjective":
                        oRetval = ListTemplateType.MeetingObjective;
                        
                        break;
                    case "TextBox":
                        oRetval = ListTemplateType.TextBox;
                        
                        break;
                    case "ThingsToBring":
                        oRetval = ListTemplateType.ThingsToBring;
                        
                        break;
                    case "HomePageLibrary":
                        oRetval = ListTemplateType.HomePageLibrary;
                        
                        break;
                    case "Posts":
                        oRetval = ListTemplateType.Posts;
                        
                        break;
                    case "Comments":
                        oRetval = ListTemplateType.Comments;
                        
                        break;
                    case "Categories":
                        oRetval = ListTemplateType.Categories;
                        
                        break;
                    case "Facility":
                        oRetval = ListTemplateType.Facility;
                        
                        break;
                    case "Whereabouts":
                        oRetval = ListTemplateType.Whereabouts;
                        
                        break;
                    case "CallTrack":
                        oRetval = ListTemplateType.CallTrack;
                        
                        break;
                    case "Circulation":
                        oRetval = ListTemplateType.Circulation;
                        
                        break;
                    case "Timecard":
                        oRetval = ListTemplateType.Timecard;
                        
                        break;
                    case "Holidays":
                        oRetval = ListTemplateType.Holidays;
                        
                        break;
                    case "IMEDic":
                        oRetval = ListTemplateType.IMEDic;
                        
                        break;
                    case "ExternalList":
                        oRetval = ListTemplateType.ExternalList;
                        
                        break;
                    case "MySiteDocumentLibrary":
                        oRetval = ListTemplateType.MySiteDocumentLibrary;
                        
                        break;
                    case "PublishingPages":
                        //oRetval = ListTemplateType.PublishingPages;
                        
                        break;
                    case "IssueTracking":
                        oRetval = ListTemplateType.IssueTracking;
                        
                        break;
                    case "AdminTasks":
                        oRetval = ListTemplateType.AdminTasks;
                        
                        break;
                    case "HealthRules":
                        oRetval = ListTemplateType.HealthRules;
                        
                        break;
                    case "HealthReports":
                        oRetval = ListTemplateType.HealthReports;
                        
                        break;
                    case "DeveloperSiteDraftApps":
                        oRetval = ListTemplateType.DeveloperSiteDraftApps;
                        
                        break;
                    default:
                        oRetval = ListTemplateType.GenericList;
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
            return oRetval;
        }
    }
}
