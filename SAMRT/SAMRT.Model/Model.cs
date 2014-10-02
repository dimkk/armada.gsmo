using Microsoft.SharePoint.Client;
using SPMeta2.CSOM.Behaviours;
using SPMeta2.CSOM.DefaultSyntax;
using SPMeta2.CSOM.ModelHosts;
using SPMeta2.CSOM.Services;
using SPMeta2.Definitions;
using SPMeta2.Models;
using SPMeta2.Syntax.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using SAMRT.Model.Definitions;
using SAMRT.Model.Definitions.Fields;

namespace SAMRT.Model
{
    public class Model
    {
        protected ClientContext Context;
        protected CSOMProvisionService Service;

        protected IList<List> AllLists;

        public Model(string webUrl, string login, string password, string domain)
        {
            Context = new ClientContext(webUrl);
            Context.ExecutingWebRequest += (sender, e) => e.WebRequestExecutor.RequestHeaders.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
            Context.Credentials = new NetworkCredential(login, password, domain);

            Service = new CSOMProvisionService();

            IEnumerable<List> lists = Context.LoadQuery(Context.Web.Lists.Include(c => c.RootFolder.Name, c => c.Id));
            Context.Load(Context.Web);
            Context.ExecuteQuery();
            AllLists = lists.ToList();
        }

        public void Deploy()
        {
            SiteModelHost siteModelHost = SiteModelHost.FromClientContext(Context);
            WebModelHost webModelHost = WebModelHost.FromClientContext(Context);

            ModelNode siteModel = SPMeta2Model.NewSiteModel(new SiteDefinition() { RequireSelfProcessing = false });
            
            //Добавляем типы поля и типы содержимого
            //AddIssueObjectMVKContentTypes(siteModel);
            //AddIssueAttachmentMVKContentTypes(siteModel);
            //AddMeetingAttachmentMVKContentTypes(siteModel);
            //AddAttachmentContentTypes(siteModel);
            //AddDocumentTPAttachmentContentTypes(siteModel);
            //AddIgHistoryContentTypes(siteModel);
            //AddIgMessageContentTypes(siteModel);
            AddMunicipalityContentTypes(siteModel);

            Service.DeployModel(siteModelHost, siteModel);

            ModelNode webModel = SPMeta2Model.NewWebModel(new WebDefinition() { RequireSelfProcessing = false });

            //Добавляем списки
            //AddIssueObjectMVKList(webModel);
            //AddIssueAttachmentMVKList(webModel);
            //AddMeetingAttachmentMVKList(webModel);
            //AddAttachmentList(webModel);
            //AddDocumentTPAttachmentList(webModel);
            //AddIgHistoryList(webModel);
            //AddIgMessageList(webModel);
            AddMunicipalityList(webModel);

            Service.DeployModel(webModelHost, webModel);

            //CleanListContentTypes(ListModel.IssueObjectMVKList.Url, ContentTypeModel.IssueObjectMVK.Name);
            //CleanListContentTypes(ListModel.MeetingAttachmentMVKList.Url, ContentTypeModel.MeetingAttachmentMVK.Name);
            //CleanListContentTypes(ListModel.IssueAttachmentMVKList.Url, ContentTypeModel.IssueAttachmentMVK.Name);
            //CleanListContentTypes(ListModel.IgHistory.Url, ContentTypeModel.IgHistory.Name);
            //CleanListContentTypes(ListModel.IgMessage.Url, ContentTypeModel.IgMessage.Name);
            CleanListContentTypes(ListModel.Municipality.Url, ContentTypeModel.Municipality.Name);
        }

        #region ContentTypes
        protected void AddIssueObjectMVKContentTypes(ModelNode modelNode)
        {
            List issueList = AllLists.Single(s => s.RootFolder.Name == "IssueMVKList");
            List objectList = AllLists.Single(s => s.RootFolder.Name == "Object");

            modelNode
                .WithFields(fields => fields
                    .AddField(IssueObjectMVKModel.IssueObjectIssueMVK, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, issueList.Id, "Title")))
                    .AddField(IssueObjectMVKModel.IssueObjectObjectMVK, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, objectList.Id, "Title")))
                    .AddField(IssueObjectMVKModel.IssueObjectIssueIdMVK, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, objectList.Id, "ID")))
                    .AddField(IssueObjectMVKModel.IssueObjectObjectAddressMVK, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, objectList.Id, "Adress")))
                    .AddField(IssueObjectMVKModel.IssueObjectObjectAreaMVK, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, objectList.Id, "Area")))
                    .AddField(IssueObjectMVKModel.IssueObjectObjectCadastralIdMVK, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, objectList.Id, "Cadastralnumber")))
                    .AddField(IssueObjectMVKModel.IssueObjectObjectFirmNameMVK, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, objectList.Id, "Firmname")))
                    .AddField(IssueObjectMVKModel.IssueObjectObjectFactDateMVK, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, objectList.Id, "factdate")))
                    .AddField(IssueObjectMVKModel.IssueObjectObjectDateMVK, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, objectList.Id, "data")))
                )
                .WithContentTypes(contentTypes => contentTypes
                    .AddContentType(ContentTypeModel.IssueObjectMVK, contentType => contentType
                        .AddContentTypeFieldLink(IssueObjectMVKModel.IssueObjectIssueMVK)
                        .AddContentTypeFieldLink(IssueObjectMVKModel.IssueObjectIssueIdMVK)
                        .AddContentTypeFieldLink(IssueObjectMVKModel.IssueObjectObjectMVK)
                        .AddContentTypeFieldLink(IssueObjectMVKModel.IssueObjectObjectAddressMVK)
                        .AddContentTypeFieldLink(IssueObjectMVKModel.IssueObjectObjectAreaMVK)
                        .AddContentTypeFieldLink(IssueObjectMVKModel.IssueObjectObjectCadastralIdMVK)
                        .AddContentTypeFieldLink(IssueObjectMVKModel.IssueObjectObjectFirmNameMVK)
                        .AddContentTypeFieldLink(IssueObjectMVKModel.IssueObjectObjectFactDateMVK)
                        .AddContentTypeFieldLink(IssueObjectMVKModel.IssueObjectObjectDateMVK)
                    ));

        }

        protected void AddMeetingAttachmentMVKContentTypes(ModelNode modelNode)
        {
            List meetingList = AllLists.Single(s => s.RootFolder.Name == "MeetingMVKList");
            List docTypeList = AllLists.Single(s => s.RootFolder.Name == "DocTypeBookList");

            modelNode
                .WithFields(fields => fields
                    .AddField(MeetingAttachmentMVKModel.MeetingAttachmentMeetingMVK, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, meetingList.Id)))
                    .AddField(MeetingAttachmentMVKModel.MeetingAttachmentDocTypeMVK, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, docTypeList.Id)))
                    .AddField(MeetingAttachmentMVKModel.MeetingAttachmentDescriptionMVK)
                    .AddField(MeetingAttachmentMVKModel.MeetingAttachmentProtocolCopyMVK)
                )
                .WithContentTypes(contentTypes => contentTypes
                    .AddContentType(ContentTypeModel.MeetingAttachmentMVK, contentType => contentType
                        .AddContentTypeFieldLink(MeetingAttachmentMVKModel.MeetingAttachmentMeetingMVK)
                        .AddContentTypeFieldLink(MeetingAttachmentMVKModel.MeetingAttachmentDocTypeMVK)
                        .AddContentTypeFieldLink(MeetingAttachmentMVKModel.MeetingAttachmentDescriptionMVK)
                        .AddContentTypeFieldLink(MeetingAttachmentMVKModel.MeetingAttachmentProtocolCopyMVK)
                    ));
        }

        protected void AddIssueAttachmentMVKContentTypes(ModelNode modelNode)
        {
            List issueList = AllLists.Single(s => s.RootFolder.Name == "IssueMVKList");

            modelNode
                .WithFields(fields => fields
                    .AddField(IssueAttachmentMVKModel.IssueAttachmentIssueMVK, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, issueList.Id)))
                )
                .WithContentTypes(contentTypes => contentTypes
                    .AddContentType(ContentTypeModel.IssueAttachmentMVK, contentType => contentType
                        .AddContentTypeFieldLink(IssueAttachmentMVKModel.IssueAttachmentIssueMVK)
                    ));
        }

        protected void AddAttachmentContentTypes(ModelNode modelNode)
        {
            List orderList = AllLists.Single(s => s.RootFolder.Name == "OrderMVKList");
            List issueRgList = AllLists.Single(s => s.RootFolder.Name == "IssueMVKList");
            List issueAkList = AllLists.Single(s => s.RootFolder.Name == "AgendaQuestionList");

            modelNode
                .WithFields(fields => fields
                    .AddField(AttachmentModel.AttachmentOrder, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, orderList.Id)))
                    .AddField(AttachmentModel.AttachmentIssueRg, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, issueRgList.Id)))
                    .AddField(AttachmentModel.AttachmentIssueAk, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, issueAkList.Id)))
                )
                .WithContentTypes(contentTypes => contentTypes
                    .AddContentType(ContentTypeModel.Attachment, contentType => contentType
                        .AddContentTypeFieldLink(AttachmentModel.AttachmentOrder)
                        .AddContentTypeFieldLink(AttachmentModel.AttachmentIssueRg)
                        .AddContentTypeFieldLink(AttachmentModel.AttachmentIssueAk)
                    ));
        }

        protected void AddDocumentTPAttachmentContentTypes(ModelNode modelNode)
        {
            List documentTPList = AllLists.Single(s => s.RootFolder.Name == "DocumentTP");

            modelNode
                .WithFields(fields => fields
                    .AddField(DocumentTPAttachmentModel.DocumentTPAttachmentDocumentTP, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, documentTPList.Id)))
                )
                .WithContentTypes(contentTypes => contentTypes
                    .AddContentType(ContentTypeModel.DocumentTPAttachment, contentType => contentType
                        .AddContentTypeFieldLink(DocumentTPAttachmentModel.DocumentTPAttachmentDocumentTP)
                    ));
        }

        protected void AddIgHistoryContentTypes(ModelNode modelNode)
        {
            List issueIgList = AllLists.Single(s => s.RootFolder.Name == "IssueMVKList");
            List issueAkList = AllLists.Single(s => s.RootFolder.Name == "AgendaQuestionList");
            List documentTpList = AllLists.Single(s => s.RootFolder.Name == "DocumentTP");

            modelNode
                .WithFields(fields => fields
                    .AddField(IgHistoryModel.IgHistoryIssueRg, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, issueIgList.Id, "Title")))
                    .AddField(IgHistoryModel.IgHistoryIssueAk, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, issueAkList.Id, "Title")))
                    .AddField(IgHistoryModel.IgHistoryDocumentTp, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, documentTpList.Id, "Title")))
                    .AddField(IgHistoryModel.IgHistoryDictionary)
                    .AddField(IgHistoryModel.IgHistoryDirection)
                    .AddField(IgHistoryModel.IgHistorySenderSystem)
                    .AddField(IgHistoryModel.IgHistoryReceiverSystem)
                    .AddField(IgHistoryModel.IgHistoryStatus)
                    .AddField(IgHistoryModel.IgHistoryError)
                    .AddField(IgHistoryModel.IgHistorySendTryCount)
                )
                .WithContentTypes(contentTypes => contentTypes
                    .AddContentType(ContentTypeModel.IgHistory, contentType => contentType
                        .AddContentTypeFieldLinks(
                            IgHistoryModel.IgHistoryIssueRg,
                            IgHistoryModel.IgHistoryIssueAk,
                            IgHistoryModel.IgHistoryDocumentTp,
                            IgHistoryModel.IgHistoryDictionary,
                            IgHistoryModel.IgHistoryDirection,
                            IgHistoryModel.IgHistorySenderSystem,
                            IgHistoryModel.IgHistoryReceiverSystem,
                            IgHistoryModel.IgHistoryStatus,
                            IgHistoryModel.IgHistoryError,
                            IgHistoryModel.IgHistorySendTryCount)
                    ));

        }

        protected void AddIgMessageContentTypes(ModelNode modelNode)
        {
            List igHistoryList = AllLists.Single(s => s.RootFolder.Name == "IgHistory");
            List igMessageList = AllLists.Single(s => s.RootFolder.Name == "IgMessage");

            modelNode
                .WithFields(fields => fields
                    .AddField(IgMessageModel.IgMessageIgHistory, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, igHistoryList.Id, "Title")))
                    .AddField(IgMessageModel.IgMessageParentIgMessage, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, igMessageList.Id, "Title")))
                    .AddField(IgMessageModel.IgMessageHandlerVersion)
                    .AddField(IgMessageModel.IgMessageDirection)
                    .AddField(IgMessageModel.IgMessageSenderSystem)
                    .AddField(IgMessageModel.IgMessageReceiverSystem)
                    .AddField(IgMessageModel.IgMessageDataVersion)
                    .AddField(IgMessageModel.IgMessageType)
                    .AddField(IgMessageModel.IgMessageContent)
                    .AddField(IgMessageModel.IgMessageIsSuccess)
                    .AddField(IgMessageModel.IgMessageError)
                )
                .WithContentTypes(contentTypes => contentTypes
                    .AddContentType(ContentTypeModel.IgMessage, contentType => contentType
                        .AddContentTypeFieldLinks(
                            IgMessageModel.IgMessageIgHistory,
                            IgMessageModel.IgMessageParentIgMessage,
                            IgMessageModel.IgMessageHandlerVersion,
                            IgMessageModel.IgMessageDirection,
                            IgMessageModel.IgMessageSenderSystem,
                            IgMessageModel.IgMessageReceiverSystem,
                            IgMessageModel.IgMessageDataVersion,
                            IgMessageModel.IgMessageType,
                            IgMessageModel.IgMessageContent,
                            IgMessageModel.IgMessageIsSuccess,
                            IgMessageModel.IgMessageError)
                    ));

        }

        protected void AddMunicipalityContentTypes(ModelNode modelNode)
        {
            List igMunicipalityList = AllLists.Single(s => s.RootFolder.Name == "Municipality");

            modelNode
                .WithFields(fields => fields
                    .AddField(MunicipalityModel.MunicipalityParentMunicipality, field =>
                        field.OnCreated((fieldDef, spField) =>
                            spField.MakeLookupConnectionToList(Context.Web.Id, igMunicipalityList.Id, "Title")))
                    .AddField(MunicipalityModel.MunicipalityType)
                    .AddField(MunicipalityModel.MunicipalityOkato)
                )
                .WithContentTypes(contentTypes => contentTypes
                    .AddContentType(ContentTypeModel.Municipality, contentType => contentType
                        .AddContentTypeFieldLinks(
                            MunicipalityModel.MunicipalityParentMunicipality,
                            MunicipalityModel.MunicipalityType,
                            MunicipalityModel.MunicipalityOkato)
                    ));

        }
        #endregion

        #region Lists
        protected void AddIssueObjectMVKList(ModelNode modelNode)
        {
            modelNode
                .WithLists(
                    lists =>
                        lists.AddList(ListModel.IssueObjectMVKList,
                            list => list.AddContentTypeLink(ContentTypeModel.IssueObjectMVK)));
        }

        protected void AddMeetingAttachmentMVKList(ModelNode modelNode)
        {
            modelNode
                .WithLists(
                    lists =>
                        lists.AddList(ListModel.MeetingAttachmentMVKList,
                            list => list.AddContentTypeLink(ContentTypeModel.MeetingAttachmentMVK)));
        }

        protected void AddIssueAttachmentMVKList(ModelNode modelNode)
        {
            modelNode
                .WithLists(
                    lists =>
                        lists.AddList(ListModel.IssueAttachmentMVKList,
                            list => list.AddContentTypeLink(ContentTypeModel.IssueAttachmentMVK)));
        }

        protected void AddAttachmentList(ModelNode modelNode)
        {
            modelNode
                .WithLists(
                    lists =>
                        lists.AddList(ListModel.AttachmentList,
                            list => list.AddContentTypeLink(ContentTypeModel.Attachment)));
        }

        protected void AddDocumentTPAttachmentList(ModelNode modelNode)
        {
            modelNode
                .WithLists(
                    lists =>
                        lists.AddList(ListModel.DocumentTPAttachmentList,
                            list => list.AddContentTypeLink(ContentTypeModel.DocumentTPAttachment)));
        }

        protected void AddIgHistoryList(ModelNode modelNode)
        {
            modelNode
                .WithLists(
                    lists =>
                        lists.AddList(ListModel.IgHistory,
                            list => list.AddContentTypeLink(ContentTypeModel.IgHistory)));
        }

        protected void AddIgMessageList(ModelNode modelNode)
        {
            modelNode
                .WithLists(
                    lists =>
                        lists.AddList(ListModel.IgMessage,
                            list => list.AddContentTypeLink(ContentTypeModel.IgMessage)));
        }

        protected void AddMunicipalityList(ModelNode modelNode)
        {
            modelNode
                .WithLists(
                    lists =>
                        lists.AddList(ListModel.Municipality,
                            list => list.AddContentTypeLink(ContentTypeModel.Municipality)));
        }
        #endregion

        protected void CleanListContentTypes(string listName, string contentTypeName)
        {
            var query = Context.Web.Lists.Where(s => s.RootFolder.Name == listName)
                .Include(c => c.ContentTypes, c => c.RootFolder, c => c.RootFolder.ContentTypeOrder);

            IEnumerable<List> lists = Context.LoadQuery(query);
            Context.ExecuteQuery();
            List list = lists.First();

            ContentType targetType = null;
            var allContentTypes = new List<ContentType>();
            foreach (ContentType ct in list.ContentTypes)
            {
                allContentTypes.Add(ct);
                if (ct.Name.Equals(contentTypeName, StringComparison.OrdinalIgnoreCase))
                    targetType = ct;
            }

            list.RootFolder.UniqueContentTypeOrder = list.RootFolder.ContentTypeOrder.Where(s => s.StringValue == targetType.Id.StringValue).ToList();
            list.RootFolder.Update();
            Context.ExecuteQuery();

            foreach (ContentType ct in allContentTypes)
                if (!ct.Sealed && ct.Id.StringValue != targetType.Id.StringValue)
                    ct.DeleteObject();

            Context.ExecuteQuery();
        }
    }
}
