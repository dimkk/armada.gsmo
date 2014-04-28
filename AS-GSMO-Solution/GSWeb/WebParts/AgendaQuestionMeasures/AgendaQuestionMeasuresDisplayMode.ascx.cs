using System;
using System.Linq;
using System.Web.UI;
using Microsoft.SharePoint;

namespace GSWeb.WebParts.AgendaQuestionMeasures
{
    public partial class AgendaQuestionMeasuresDisplayMode : UserControl
    {
        protected void Page_Load(object sender, EventArgs args)
        {
            var web = SPContext.Current.Web;

            var categoryLookup = SPContext.Current.Item["Категория вопроса"] as string;
            if (string.IsNullOrEmpty(categoryLookup))
                return;

            var currentQuestionCategory = new SPFieldLookupValue(categoryLookup);

            var measureValues = web.Lists["Показатели по вопросу"]
                .Items.OfType<SPListItem>()
                .Select(item => new
                {
                    Question = new SPFieldLookupValue(item["Вопрос"] as string),
                    Measure = new SPFieldLookupValue(item["Показатель"] as string),
                    Value = item["Значение"] as string
                })
                .Where(item => item.Question.LookupId == SPContext.Current.ItemId)
                .ToArray();

            repeaterMeasures.DataSource =
                from item in web.Lists["Градостроительные показатели"].Items.OfType<SPListItem>()
                let questionCategory = new SPFieldLookupValue(item["Категория вопроса"] as string)
                where questionCategory.LookupId == currentQuestionCategory.LookupId
                let measure = new SPFieldLookupValue(item["Показатель"] as string)
                let value = measureValues.FirstOrDefault(v => v.Measure.LookupId == item.ID)
                where value != null
                let measuringUnit = new SPFieldLookupValue(item["Единица измерения"] as string)
                select new { Measure = measure.LookupValue, Value = value.Value, MeasuringUnit = measuringUnit.LookupValue };

            repeaterMeasures.DataBind();
        }
    }
}
