namespace Kirnau.Survey.Admin.Models
{
    public class TenantUdfPageViewData<T> : TenantPageViewData<T> 
    {
        public TenantUdfPageViewData(string model, T contentModel) : base(contentModel)
        {
            this.ModelName = model;
        }

        public string ModelName { get; set; }
    }
}