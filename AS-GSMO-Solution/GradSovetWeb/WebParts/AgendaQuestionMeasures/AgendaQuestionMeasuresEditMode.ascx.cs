using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace GradSovetWeb.WebParts.AgendaQuestionMeasures
{
    public partial class AgendaQuestionMeasuresEditMode : UserControl
    {
        protected void Page_Load(object sender, EventArgs args)
        {
            if (ScriptManager.GetCurrent(Page).IsInAsyncPostBack)
                return;

            if (!IsPostBack)
                Load();
            else
                Save();
        }

        private void Load()
        {
            if (SPContext.Current == null)
                return;

            var web = SPContext.Current.Web;

            lookupQuestionType.DataSource = web.Lists["Категории вопросов"].Items.OfType<SPListItem>()
                .Select(item => new {Id = item.ID, Name = item["Категория вопроса"]});
            lookupQuestionType.DataBind();

            var categoryLookup = SPContext.Current.Item["Категория вопроса"] as string;
            if (string.IsNullOrEmpty(categoryLookup))
                return;

            var currentQuestionCategory = new SPFieldLookupValue(categoryLookup);

            lookupQuestionType.SelectedValue = Convert.ToString(currentQuestionCategory.LookupId);

            var measureValues = from item in web.Lists["Показатели по вопросу"].Items.OfType<SPListItem>()
                let question = new SPFieldLookupValue(item["Вопрос"] as string)
                where question.LookupId == SPContext.Current.ItemId
                select new DataItem
                {
                    Measure = new SPFieldLookupValue(item["Показатель"] as string),
                    Value = item["Значение"] as string
                };

            BindRepeater(currentQuestionCategory.LookupId, measureValues);
        }

        private void Save()
        {
            var newQuestionCategory = !string.IsNullOrEmpty(lookupQuestionType.SelectedValue) ? new SPFieldLookupValue(lookupQuestionType.SelectedValue).LookupId : 0;

            var web = SPContext.Current.Web;
            var list = web.Lists["Показатели по вопросу"];

            //if (oldQuestionCategory != newQuestionCategory)
            {
                SPContext.Current.Item["Категория вопроса"] = newQuestionCategory > 0
                    ? new SPFieldLookupValue(newQuestionCategory, lookupQuestionType.SelectedItem.Text)
                    : null;

                //if (oldQuestionCategory > 0)
                {
                    foreach (var item in list.Items.OfType<SPListItem>()
                        .Where(item => new SPFieldLookupValue(item["Вопрос"] as string).LookupId == SPContext.Current.ItemId)
                        .ToArray())
                    {
                        item.Delete();
                    }
                }

                var allMeasures = from item in web.Lists["Градостроительные показатели"].Items.OfType<SPListItem>()
                    let questionCategory = new SPFieldLookupValue(item["Категория вопроса"] as string)
                    where questionCategory.LookupId == newQuestionCategory
                    select new
                    {
                        Id = item.ID,
                        Measure = new SPFieldLookupValue(item["Показатель"] as string),
                        Unit = new SPFieldLookupValue(item["Единица измерения"] as string)
                    };

                allMeasures = allMeasures.ToArray();

                foreach (RepeaterItem item in repeaterMeasures.Items)
                {
                    var value = ((TextBox) item.FindControl("textValue")).Text;
                    if (string.IsNullOrEmpty(value))
                        continue;

                    int questionMeasureId;

                    var measureId = Convert.ToInt32(((HiddenField)item.FindControl("hiddenMeasure")).Value);
                    var measures = allMeasures.Where(m => m.Measure.LookupId == measureId).ToArray();

                    if (measures.Length > 1)
                    {
                        var lookupUnit = (DropDownList) item.FindControl("lookupUnit");
                        questionMeasureId = measures
                            .Where(m => m.Unit.LookupId == Convert.ToInt32(lookupUnit.SelectedValue))
                            .Select(m => m.Id)
                            .First();
                    }
                    else
                        questionMeasureId = measures[0].Id;

                    var newItem = list.AddItem();
                    newItem["Вопрос"] = SPContext.Current.ItemId;
                    newItem["Показатель"] = questionMeasureId;
                    newItem["Значение"] = value;
                    newItem.Update();
                }
            }

            list.Update();
        }

        protected void OnQuestionTypeChanged(object sender, EventArgs args)
        {
            BindRepeater(Convert.ToInt32(lookupQuestionType.SelectedValue), new DataItem[0]);
        }

        private void BindRepeater(int categoryId, IEnumerable<DataItem> measureValues)
        {
            var web = SPContext.Current.Web;

            repeaterMeasures.DataSource =
                from item in web.Lists["Градостроительные показатели"].Items.OfType<SPListItem>()
                let questionCategory = new SPFieldLookupValue(item["Категория вопроса"] as string)
                where questionCategory.LookupId == categoryId
                group item by item["Показатель"] as string into g
                let measure = new SPFieldLookupValue(g.Key) 
                let measuringUnits = g.Select(item => new SPFieldLookupValue(item["Единица измерения"] as string))
                let measureValue = measureValues.FirstOrDefault(v => g.Any(item => v.Measure.LookupId == item.ID))
                let currentMeasureUnit = measureValue != null
                    ? g.Where(item => item.ID == measureValue.Measure.LookupId).Select(item => new SPFieldLookupValue(item["Единица измерения"] as string)).FirstOrDefault()
                    : null
                select new DataItem
                {
                    Measure = measure,
                    Value = measureValue != null ? measureValue.Value : null,
                    MeasuringUnits = measuringUnits.ToArray(),
                    CurrentMeasureUnit = currentMeasureUnit
                };

            repeaterMeasures.DataBind();
        }

        protected void OnItemDataBound(object sender, RepeaterItemEventArgs args)
        {
            var dataItem = ((DataItem) args.Item.DataItem);

            ((HiddenField)args.Item.FindControl("hiddenMeasure")).Value = dataItem.Measure.LookupId.ToString();

            ((TextBox)args.Item.FindControl("textValue")).Text = dataItem.Value;
            if (dataItem.MeasuringUnits.Length == 1)
            {
                args.Item.FindControl("placeMeasuringUnit").Controls.Add(new Label {Text = dataItem.MeasuringUnits[0].LookupValue});
            }
            else
            {
                var lookupUnit = (DropDownList) args.Item.FindControl("lookupUnit");
                lookupUnit.Visible = true;

                lookupUnit.DataSource = dataItem.MeasuringUnits;
                lookupUnit.DataBind();

                if (dataItem.CurrentMeasureUnit != null)
                    lookupUnit.SelectedValue = dataItem.CurrentMeasureUnit.LookupId.ToString();
            }
        }
    }

    class DataItem
    {
        public SPFieldLookupValue Measure { get; set; }
        public string Value { get; set; }
        public SPFieldLookupValue[] MeasuringUnits { get; set; }
        public SPFieldLookupValue CurrentMeasureUnit { get; set; }
    }
}
