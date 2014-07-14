using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace GradSovetPages.Layouts.GradSovetPages.Pages
{
    public partial class MeetingMvk : LayoutsPageBase
    {
        public MeetingMvk()
        {
            _meeting = new MeetingMvkList();
            _question = new QuestionMvkList();
        }

        private readonly ViewModelList _meeting;
        private readonly ViewModelList _question;

        public ViewModelList Meeting { get { return _meeting; } }

        public ViewModelList Question { get { return _question; } }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }

    public class MeetingMvkList : ViewModelList
    {
        public override string ListName
        {
            get { return "MVK Meeting"; }
        }

        public override string ItemContentId
        {
            get { return "0x0100C274D4A47E4742EEABF2D5FDF35DFBC900564F722C76FF6748AE39FEEA68F2EC1C"; }
        }
    }

    public class QuestionMvkList : ViewModelList
    {
        public override string ListName
        {
            get { return "MVK Issue"; }
        }

        public override string ItemContentId
        {
            get { return "0x01009435E8C0C3E94107BD6F56DE758815D600C914B89ED365644284FC3A7A7EB36B71"; }
        }
    }

    public abstract class ViewModelList
    {
        public abstract string ListName { get; }
        public abstract string ItemContentId { get; }
    }
}
