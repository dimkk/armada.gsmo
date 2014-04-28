using ClosedXML.Excel;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;
using System.Web.UI.WebControls.WebParts;
using SPWebParts = Microsoft.SharePoint.WebPartPages;
using Microsoft.SharePoint.WebPartPages.Communication;
using System.Web.Script.Serialization;

namespace ExportToExcel.Control
{
    internal static class SPGlobal
    {
        internal enum DelimiterType
        {
            Internal,
            InternalSub,
        }
    }

    internal static class SPFieldMultiColumnValue
    {
        internal static List<string> ParseMultiColumnValue(string fieldValue, SPGlobal.DelimiterType delimiterType, bool bIncludeEmpty)
        {
            List<string> subColumnValues = (List<string>)null;
            if (!SPFieldMultiColumnValue.TryParseMultiColumnValue(fieldValue, delimiterType, bIncludeEmpty, out subColumnValues))
                throw new ArgumentException();
            else
                return subColumnValues;
        }

        internal static bool TryParseMultiColumnValue(string fieldValue, SPGlobal.DelimiterType delimiterType, bool bIncludeEmpty, out List<string> subColumnValues)
        {
            subColumnValues = new List<string>();
            if (string.IsNullOrEmpty(fieldValue))
                return true;
            string str1 = delimiterType == SPGlobal.DelimiterType.Internal ? ";#" : ",#";
            if (str1.Length != 2)
                return false;
            char c = str1[0];
            char ch = str1[1];
            string oldValue = new string(c, 2);
            string newValue = new string(c, 1);
            int startIndex = 0;
            if (fieldValue.StartsWith(str1, StringComparison.Ordinal))
            {
                if (bIncludeEmpty)
                    subColumnValues.Add(string.Empty);
                startIndex = str1.Length;
            }
            int index = startIndex;
            bool flag = false;
            while (index < fieldValue.Length)
            {
                if ((int)fieldValue[index] == (int)c)
                {
                    ++index;
                    if (index < fieldValue.Length)
                    {
                        if ((int)fieldValue[index] == (int)ch)
                        {
                            if (index - 1 > startIndex)
                            {
                                string str2 = fieldValue.Substring(startIndex, index - startIndex - 1);
                                if (flag)
                                    str2 = str2.Replace(oldValue, newValue);
                                subColumnValues.Add(str2);
                                flag = false;
                            }
                            else
                                subColumnValues.Add(string.Empty);
                            ++index;
                            startIndex = index;
                        }
                        else
                        {
                            if ((int)fieldValue[index] != (int)c)
                                return false;
                            ++index;
                            flag = true;
                        }
                    }
                    else
                        break;
                }
                else
                    ++index;
            }
            if (index > startIndex)
            {
                string str2 = fieldValue.Substring(startIndex, index - startIndex);
                if (flag)
                    str2 = str2.Replace(oldValue, newValue);
                subColumnValues.Add(str2);
            }
            else if (bIncludeEmpty)
                subColumnValues.Add(string.Empty);
            return true;
        }
    }

    internal static class DataFormWebPart
    {
        // added by atarutin
        internal static bool IsDateTimeField(SPField field)
        {
            SPFieldLookup spFieldLookup = field as SPFieldLookup;
            if (spFieldLookup != null)
            {
                SPWeb curWeb = field.ParentList != null ? field.ParentList.ParentWeb : SPContext.Current.Web;

                // try to get showfield
                if (!String.IsNullOrEmpty(spFieldLookup.LookupList) && !String.IsNullOrEmpty(spFieldLookup.LookupField))
                {
                    // Is this the primary or secondary field for a list relationship?
                    string strRelationship = spFieldLookup.IsRelationship ? "Primary" : "Secondary";

                    // Get the name of the list where this field gets information.
                    SPList targetList = curWeb.Lists[new Guid(spFieldLookup.LookupList)];

                    // Get the name of the field where this field gets information.
                    SPField targetField = targetList.Fields.GetFieldByInternalName(spFieldLookup.LookupField);

                    if (targetField is SPFieldDateTime)
                        return true;
                }
                else 
                    return false;
                
            }

            if (field.Type == SPFieldType.DateTime)
                return true;
            if (field is SPFieldCalculated)
                return (field as SPFieldCalculated).OutputType == SPFieldType.DateTime;
            else
                return false;
        }

        private static string[] VariantTypeToFieldType = new string[12]
        {
          ((object) SPFieldType.Text).ToString(),
          ((object) SPFieldType.Text).ToString(),
          ((object) SPFieldType.Text).ToString(),
          ((object) SPFieldType.Text).ToString(),
          ((object) SPFieldType.Text).ToString(),
          ((object) SPFieldType.Number).ToString(),
          ((object) SPFieldType.Text).ToString(),
          ((object) SPFieldType.DateTime).ToString(),
          ((object) SPFieldType.Text).ToString(),
          ((object) SPFieldType.Text).ToString(),
          ((object) SPFieldType.Error).ToString(),
          ((object) SPFieldType.Boolean).ToString()
        };

        internal static void GetValueStringAndFieldType(SPList list, string fieldXpath, SPWebParts.FilterOperation op, ref string valueString, ref string fieldType)
        {
            if (list == null)
                return;
            SPField fieldByInternalName = list.Fields.GetFieldByInternalName(fieldXpath);
            SPFieldCalculated spFieldCalculated = fieldByInternalName as SPFieldCalculated;
            if (spFieldCalculated != null)
            {
                int length = valueString.IndexOf('_');
                if (length > 0)
                {
                    ushort result;
                    if (ushort.TryParse(valueString.Substring(0, length), NumberStyles.None, (IFormatProvider)CultureInfo.InvariantCulture, out result) && (int)result < DataFormWebPart.VariantTypeToFieldType.Length)
                        fieldType = DataFormWebPart.VariantTypeToFieldType[(int)result];
                    valueString = valueString.Substring(length + 1);
                }
            }
            if (fieldType != null)
                return;
            SPFieldDecimal spFieldDecimal = fieldByInternalName as SPFieldDecimal;
            if (fieldByInternalName != null && (fieldByInternalName.Type != SPFieldType.Invalid || spFieldDecimal != null))
            {
                fieldType = spFieldCalculated == null ? fieldByInternalName.TypeAsString : ((object)spFieldCalculated.OutputType).ToString();
                // edited by atarutin in order to emulate SPField.IsDateTimeField internal property
                // old code:
                // if (fieldByInternalName.IsDateTimeField)
                if (IsDateTimeField(fieldByInternalName))
                {
                    DateTime result;
                    // edited by atarutin because of FilterOperation.IsFromConnection not accessible outside 
                    // old code:
                    // if (!DateTime.TryParse(valueString, op.IsFromConnection ? (IFormatProvider)CultureInfo.CurrentCulture : (IFormatProvider)CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                    if (!DateTime.TryParse(valueString, (IFormatProvider)CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                        return;
                    valueString = result.ToString("yyyy-MM-ddTHH:mm:ssZ", (IFormatProvider)DateTimeFormatInfo.InvariantInfo);
                }
                else
                {
                    if (spFieldDecimal == null)
                        return;
                    Decimal result;
                    // edited by atarutin because of FilterOperation.IsFromConnection not accessible outside 
                    // old code:
                    // if (Decimal.TryParse(valueString, NumberStyles.Number, op.IsFromConnection ? (IFormatProvider)CultureInfo.InvariantCulture : (IFormatProvider)CultureInfo.CurrentCulture, out result))
                    if (Decimal.TryParse(valueString, NumberStyles.Number, (IFormatProvider)CultureInfo.CurrentCulture, out result))
                        valueString = result.ToString((IFormatProvider)CultureInfo.InvariantCulture);
                    fieldType = "Text";
                }
            }
            else
                fieldType = "Text";
        }
    }

    internal static class SPDataSourceView
    {
        private static bool FieldSkippedForMultFilter(SPField field)
        {
            Guid guid1 = new Guid("{23F27201-BEE3-471e-B2E7-B64FD8B7CA38}");
            Guid guid2 = new Guid("333B1BC2-0532-4872-96F1-BBBDEAD35A56");
            if ((field.TypeAsString == "TaxonomyFieldType" || field.TypeAsString == "TaxonomyFieldTypeMulti") && field.Id != guid1)
                return field.Id != guid2;
            else
                return false;
        }

        internal static XmlElement
         ModifyQueryForMultipleFilters(SPList list, string query, Dictionary<string, SPWebParts.FilterOperation> fos)
        {
            bool flag1 = false;
            // added by atarutin to support single-predicated filter
            bool singleFilter = false;
            int num = 0;
            foreach (string strName in fos.Keys)
            {
                if (!SPDataSourceView.FieldSkippedForMultFilter(list.Fields.GetField(strName)))
                {
                    ++num;
                    // commented by atarutin because of FilterOperation.IsMultipleFilter is inaccessible outside
                    /*if (fos[strName].IsMultipleFilter)
                        flag1 = true;*/
                }
            }
            // commented by atarutin because of FilterOperation.IsMultipleFilter is inaccessible outside
            /*if (!flag1)
                return (XmlElement)null;*/
            XmlDocument xmlDocument = new XmlDocument();
            using (XmlTextReader xmlTextReader = new XmlTextReader((TextReader)new StringReader(query)))
            {
                xmlTextReader.DtdProcessing = DtdProcessing.Prohibit;
                xmlDocument.Load((XmlReader)xmlTextReader);
            }
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            XmlElement documentElement = xmlDocument.DocumentElement;
            XmlNode xmlNode1 = documentElement.SelectSingleNode("Query", nsmgr);
            if (xmlNode1 == null)
            {
                XmlNode node = xmlDocument.CreateNode(XmlNodeType.Element, "", "Query", "");
                xmlNode1 = documentElement.AppendChild(node);
            }
            XmlNode xmlNode2 = xmlNode1.SelectSingleNode("Where", nsmgr);
            if (xmlNode2 == null)
            {
                XmlNode node = xmlDocument.CreateNode(XmlNodeType.Element, "", "Where", "");
                xmlNode2 = xmlNode1.AppendChild(node);
            }
            bool flag2 = num > 1;
            XmlNode firstChild = xmlNode2.FirstChild;
            XmlNode newChild1;
            if (firstChild != null)
            {
                XmlNode newChild2 = firstChild;
                newChild1 = xmlDocument.CreateNode(XmlNodeType.Element, "", "And", "");
                newChild1.AppendChild(newChild2);
                flag2 = true;
            }
            else
            {
                newChild1 = xmlDocument.CreateNode(XmlNodeType.Element, "", "And", "");
                // added by atarutin to support one-predicated filter
                singleFilter = (fos.Count == 1);
                
            }
            foreach (string strName in fos.Keys)
            {
                SPWebParts.FilterOperation op = fos[strName];
                string filterValue = op.FilterValue;
                string fieldType = op.IsFieldLookupId ? "Integer" : (string)null;
                // edited by atarutin because of FilterOperation.IsMultipleFilter is inaccessible outside
                // old code:
                // bool isMultipleFilter = op.IsMultipleFilter;
                bool isMultipleFilter = false;
                SPField field = list.Fields.GetField(strName);
                if (!SPDataSourceView.FieldSkippedForMultFilter(field))
                {
                    string fieldXpath = strName;
                    // commented by atarutin because of SPField.DisplayNameSrcField inaccessible outside
                    /*if (!list.HasExternalDataSource)
                        fieldXpath = field.DisplayNameSrcField == null ? strName : field.DisplayNameSrcField;*/
                        
                    DataFormWebPart.GetValueStringAndFieldType(list, fieldXpath, op, ref filterValue, ref fieldType);
                    if (newChild1.ChildNodes.Count == 2)
                    {
                        XmlNode newChild2 = newChild1;
                        newChild1 = xmlDocument.CreateNode(XmlNodeType.Element, "", "And", "");
                        newChild1.AppendChild(newChild2);
                    }
                    XmlNode newChild3 = xmlDocument.CreateNode(XmlNodeType.Element, "", "Or", "");
                    if (!isMultipleFilter)
                        newChild3 = newChild1;
                    List<string> list1 = SPFieldMultiColumnValue.ParseMultiColumnValue(filterValue, SPGlobal.DelimiterType.Internal, true);
                    if (list1.Count == 0 && string.IsNullOrEmpty(filterValue))
                        list1.Add(filterValue);
                    foreach (string text in list1)
                    {
                        if (newChild3.ChildNodes.Count == 2)
                        {
                            XmlNode newChild2 = newChild3;
                            newChild3 = xmlDocument.CreateNode(XmlNodeType.Element, "", "Or", "");
                            newChild3.AppendChild(newChild2);
                        }
                        if (string.IsNullOrEmpty(text))
                        {
                            XmlNode node = xmlDocument.CreateNode(XmlNodeType.Element, "", "IsNull", "");
                            XmlNode xmlNode3 = newChild3.AppendChild(node).AppendChild(xmlDocument.CreateNode(XmlNodeType.Element, "", "FieldRef", ""));
                            XmlAttribute attribute = xmlDocument.CreateAttribute("Name");
                            attribute.Value = fieldXpath;
                            xmlNode3.Attributes.SetNamedItem((XmlNode)attribute);
                        }
                        else
                        {
                            XmlNode node1 = xmlDocument.CreateNode(XmlNodeType.Element, "", "Eq", "");
                            XmlNode xmlNode3 = newChild3.AppendChild(node1);
                            XmlNode node2 = xmlDocument.CreateNode(XmlNodeType.Element, "", "FieldRef", "");
                            XmlNode xmlNode4 = xmlNode3.AppendChild(node2);
                            XmlAttribute attribute1 = xmlDocument.CreateAttribute("Name");
                            attribute1.Value = fieldXpath;
                            xmlNode4.Attributes.SetNamedItem((XmlNode)attribute1);
                            XmlNode node3 = xmlDocument.CreateNode(XmlNodeType.Element, "", "Value", "");
                            XmlNode xmlNode5 = xmlNode3.AppendChild(node3);
                            XmlAttribute attribute2 = xmlDocument.CreateAttribute("Type");
                            attribute2.Value = fieldType;
                            xmlNode5.Attributes.SetNamedItem((XmlNode)attribute2);
                            XmlNode newChild2 = (XmlNode)xmlDocument.CreateTextNode(text);
                            xmlNode5.AppendChild(newChild2);
                        }
                    }
                    if (isMultipleFilter && !flag2)
                        newChild1 = newChild3;
                    else if (isMultipleFilter && flag2)
                    {
                        newChild1.AppendChild(newChild3);
                        flag2 = true;
                    }
                }
            }
            // edited by atarutin to support one-predicated filter
            if (singleFilter)
            {
                xmlNode2.AppendChild(newChild1.FirstChild);
            }
            else
            {
                xmlNode2.AppendChild(newChild1);
            }
            return documentElement;
        }
    }

    internal class ExportContext
    {
        public string listId { get; set; }
        public string webpartId { get; set; }
    }

    [System.Runtime.InteropServices.GuidAttribute("7963FBA8-56F9-4375-A5C2-1C7AEFB69307")]
    public class ExportToSpreadsheetControl : System.Web.UI.Control
    {
        protected override void OnLoad(EventArgs e)
        {
            if (this.Page.Request["__EVENTTARGET"] == Constants.ExportToSpreadsheet) 
            {
                var spContext = SPContext.Current;
                ExportContext exCtx = new JavaScriptSerializer().Deserialize<ExportContext>(this.Page.Request["__EVENTARGUMENT"]);
                SPList list;
                SPView view;

                if (spContext.ViewContext.View != null)
                {
                    list = spContext.List;
                    view = spContext.ViewContext.View;
                }
                else
                {
                    var listId = new Guid(exCtx.listId);
                    var web = spContext.Web;
                    list = web.Lists[listId];
                    view = list.DefaultView;
                }

                
                WebPartManager wpm = WebPartManager.GetCurrentWebPartManager(this.Page);
                SPWebParts.WebPart curWebPart = null;
                foreach (Object item in wpm.WebParts)
                {
                    SPWebParts.WebPart wp = item as SPWebParts.WebPart;
                    if (wp == null) continue;

                    if (wp.StorageKey.Equals(new Guid(exCtx.webpartId)))
                    {
                        curWebPart = wp;
                        break;
                    }
                    
                }
                var filters = new Dictionary<string, SPWebParts.FilterOperation>();
                if (curWebPart is SPWebParts.DataFormWebPart)
                {
                    filters = getConnectedFilters((SPWebParts.DataFormWebPart)curWebPart)
                        .Concat(getOtherFilters((SPWebParts.DataFormWebPart)curWebPart))
                        .GroupBy(d => d.Key)
                        .ToDictionary(d => d.Key, d => d.First().Value);
                }

                ExportData(list.Title + " - " + view.Title, GetDataTable(list, view, filters));
            }
        }

        private Dictionary<string, SPWebParts.FilterOperation> getConnectedFilters(SPWebParts.DataFormWebPart dwp)
        {
            Dictionary<string, SPWebParts.FilterOperation> res = new Dictionary<string, SPWebParts.FilterOperation>();

            foreach (var item in dwp.ConnectionFilters)
            {
                var fo = new SPWebParts.FilterOperation()
                {
                    FilterData = null,
                    FilterValue = item.Value,
                    IsFieldLookupId = false,
                    OperationType = null
                };
                res.Add(item.Key, fo);
            }

            return res;
        }

        private Dictionary<string, SPWebParts.FilterOperation> getOtherFilters(SPWebParts.DataFormWebPart dwp)
        {
            return dwp.FilterOperations;
        }

        private void ExportData(string title, System.Data.DataTable table)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(title.Length > 31 ? title.Substring(0, 31) : title);
            ws.Cell(1, 1).InsertTable(table);

            var response = this.Page.Response;
            response.Clear();

            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var filename = title + ".xlsx";
            response.AddHeader("content-disposition", GetContentDisposition(filename));

            // Flush the workbook to the Response.OutputStream
            using (var memoryStream = new MemoryStream())
            {
                wb.SaveAs(memoryStream);
                memoryStream.WriteTo(response.OutputStream);
            }
            response.End();
        }

        //http://stackoverflow.com/questions/93551/how-to-encode-the-filename-parameter-of-content-disposition-header-in-http
        private string GetContentDisposition(string filename)
        {
            var request = this.Page.Request;
            string contentDisposition;
            if (request.Browser.Browser == "IE" && (request.Browser.Version == "7.0" || request.Browser.Version == "8.0"))
                contentDisposition = "attachment; filename=" + Uri.EscapeDataString(filename);
            else if (request.UserAgent != null && request.UserAgent.ToLowerInvariant().Contains("android")) // android built-in download manager (all browsers on android)
                contentDisposition = "attachment; filename=\"" + MakeAndroidSafeFileName(filename) + "\"";
            else
                contentDisposition = "attachment; filename=\"" + filename + "\"; filename*=UTF-8''" + Uri.EscapeDataString(filename);
            return contentDisposition;
        }

        private static readonly Dictionary<char, char> AndroidAllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ._-+,@£$€!½§~'=()[]{}0123456789".ToDictionary(c => c);

        private string MakeAndroidSafeFileName(string fileName)
        {
            char[] newFileName = fileName.ToCharArray();
            for (int i = 0; i < newFileName.Length; i++)
            {
                if (!AndroidAllowedChars.ContainsKey(newFileName[i]))
                    newFileName[i] = '_';
            }
            return new string(newFileName);
        }

        private static System.Data.DataTable GetDataTable(SPList list, SPView view, Dictionary<string, SPWebParts.FilterOperation> filters)
        {
            var query = new SPQuery(view);
            if (filters.Count > 0)
            {
                XmlElement xmlElement = SPDataSourceView.ModifyQueryForMultipleFilters(list, "<View><Query>" + query.Query + "</Query></View>", filters);
                if (xmlElement != null)
                {
                    query.Query = xmlElement.FirstChild.InnerXml;
                }
            }

            SPListItemCollectionPosition position;
            var flags = SPListGetDataTableOptions.UseBooleanDataType | SPListGetDataTableOptions.UseCalculatedDataType;
            var result = list.GetDataTable(query, flags, out position);
            while (position != null)
            {
                query.ListItemCollectionPosition = position;
                list.AppendDataTable(query, flags, result, out position);
            }
            return result;
        }



    }
}
