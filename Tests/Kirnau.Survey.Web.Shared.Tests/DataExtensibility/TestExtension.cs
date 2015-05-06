namespace Kirnau.Survey.Web.Shared.Tests.DataExtensibility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Kirnau.Survey.Web.Shared.DataExtensibility;

    public class TestExtension : IUDFModel
    {
        private IList<UDFItem> items;

        public TestExtension()
        {
            this.items = new List<UDFItem> { new UDFItem { DefaultValue = "defaultvalue", Name = "name", Value = "value", Display = "display" } };
        }

        IList<UDFItem> IUDFModel.UserDefinedFields
        {
            get
            {
                return this.items;
            }
            set
            {
               this.items = value;
            }
        }
    }
}
